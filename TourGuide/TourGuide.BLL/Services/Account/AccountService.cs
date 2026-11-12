using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;
using TourGuide.BLL.Common;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Constants;
using TourGuide.DAL.Entities;
using TourGuide.DAL.Enums;
using TourGuide.DAL.UnitOfWork;

namespace TourGuide.BLL.Services.Account;

// FR-01, FR-02, BR-01, NFR-02, NFR-03.
public class AccountService : IAccountService
{
    private const string EmailTakenMessage = "An account with this email already exists.";
    private const int MaxOriginalFileNameLength = 260; // GuideDocument.OriginalFileName

    private readonly UserManager<ApplicationUser> _users;
    private readonly IUnitOfWork _uow;
    private readonly IFileStorage _files;
    private readonly IMapper _mapper;
    private readonly IValidator<RegisterTouristRequest> _touristValidator;
    private readonly IValidator<RegisterGuideRequest> _guideValidator;
    private readonly TimeProvider _time;

    public AccountService(
        UserManager<ApplicationUser> users,
        IUnitOfWork uow,
        IFileStorage files,
        IMapper mapper,
        IValidator<RegisterTouristRequest> touristValidator,
        IValidator<RegisterGuideRequest> guideValidator,
        TimeProvider time)
    {
        _users = users;
        _uow = uow;
        _files = files;
        _mapper = mapper;
        _touristValidator = touristValidator;
        _guideValidator = guideValidator;
        _time = time;
    }

    // FR-01, NFR-03: the password is hashed by Identity, never stored as text.
    public async Task<Result<string>> RegisterTouristAsync(RegisterTouristRequest request, CancellationToken ct = default)
    {
        request.FullName = (request.FullName ?? string.Empty).Trim();
        request.Email = (request.Email ?? string.Empty).Trim();
        request.CountryCode = (request.CountryCode ?? string.Empty).Trim().ToUpperInvariant();

        var validation = await _touristValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<string>.Invalid(ToErrors(validation));

        if (await _users.FindByEmailAsync(request.Email) is not null)
            return EmailTaken();

        await using var tx = await _uow.BeginTransactionAsync(ct);

        var created = await CreateUserAsync(_mapper.Map<ApplicationUser>(request), request.Password, Roles.Tourist);
        if (!created.Succeeded)
            return Result<string>.Invalid(created.Errors);

        await tx.CommitAsync(ct);
        return Result<string>.Ok(created.Value!.Id);
    }

    // FR-02, BR-01 (starts Pending), NFR-02 (files outside wwwroot).
    // The files are saved first. If anything after that fails (weak password, database error), they are deleted again.
    public async Task<Result<string>> RegisterGuideAsync(RegisterGuideRequest request, CancellationToken ct = default)
    {
        request.FullName = (request.FullName ?? string.Empty).Trim();
        request.Email = (request.Email ?? string.Empty).Trim();
        request.CountryCode = (request.CountryCode ?? string.Empty).Trim().ToUpperInvariant();

        var validation = await _guideValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return Result<string>.Invalid(ToErrors(validation));

        if (await _users.FindByEmailAsync(request.Email) is not null)
            return EmailTaken();

        var now = _time.GetUtcNow().UtcDateTime;
        var savedFiles = new List<string>();
        var committed = false;

        try
        {
            var documents = new List<GuideDocument>();
            foreach (var (type, file) in DocumentsOf(request))
            {
                var contentType = FileSignatureDetector.Detect(file.Content)
                    ?? throw new InvalidOperationException("The file content is not a supported type.");

                var storedName = await _files.SaveAsync(file, FileFolders.GuideDocuments, ct);
                savedFiles.Add(storedName);

                documents.Add(new GuideDocument
                {
                    DocumentType = type,
                    StoredFileName = storedName,
                    OriginalFileName = SafeOriginalName(file.FileName),
                    ContentType = contentType,
                    SizeBytes = file.SizeBytes,
                    UploadedAtUtc = now,
                });
            }

            await using var tx = await _uow.BeginTransactionAsync(ct);

            var created = await CreateUserAsync(_mapper.Map<ApplicationUser>(request), request.Password, Roles.Guide);
            if (!created.Succeeded)
                return Result<string>.Invalid(created.Errors);

            var profile = new GuideProfile
            {
                UserId = created.Value!.Id,
                VerificationStatus = VerificationStatus.Pending,
                SubmittedAtUtc = now,
            };
            foreach (var document in documents)
                profile.Documents.Add(document);

            await _uow.GuideProfiles.AddAsync(profile, ct);
            await _uow.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            committed = true;
            return Result<string>.Ok(created.Value.Id);
        }
        finally
        {
            if (!committed)
            {
                foreach (var storedName in savedFiles)
                    await _files.DeleteAsync(storedName, FileFolders.GuideDocuments, CancellationToken.None);
            }
        }
    }

    // Creates the user and adds the role. Must run inside the caller's transaction.
    private async Task<Result<ApplicationUser>> CreateUserAsync(ApplicationUser user, string password, string role)
    {
        user.CreatedAtUtc = _time.GetUtcNow().UtcDateTime;

        var created = await _users.CreateAsync(user, password);
        if (!created.Succeeded)
            return Result<ApplicationUser>.Invalid(FromIdentity(created.Errors));

        var roleResult = await _users.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
            throw new InvalidOperationException(
                $"Could not add role {role}: {string.Join("; ", roleResult.Errors.Select(e => e.Description))}");

        return Result<ApplicationUser>.Ok(user);
    }

    private static IEnumerable<(DocumentType Type, FileUpload File)> DocumentsOf(RegisterGuideRequest request)
    {
        yield return (DocumentType.ProofOfResidence, request.ProofOfResidence);
        yield return (DocumentType.GovernmentId, request.GovernmentId);
        yield return (DocumentType.CriminalRecord, request.CriminalRecord);
    }

    // The user's file name is kept for display only, never for storage.
    private static string SafeOriginalName(string fileName)
    {
        var name = Path.GetFileName(fileName ?? string.Empty).Trim();
        if (name.Length == 0)
            name = "document";
        return name.Length <= MaxOriginalFileNameLength ? name : name[^MaxOriginalFileNameLength..];
    }

    private static Result<string> EmailTaken() =>
        Result<string>.Invalid(new List<ValidationError> { new("Email", EmailTakenMessage) });

    // "ProofOfResidence.SizeBytes" -> "ProofOfResidence", so the message shows under the right input.
    private static List<ValidationError> ToErrors(ValidationResult validation) =>
        [.. validation.Errors
            .Select(e => new ValidationError(e.PropertyName.Split('.')[0], e.ErrorMessage))
            .DistinctBy(e => (e.Field, e.Message))];

    // Identity's own rules (password strength, duplicate email) become field errors.
    private static List<ValidationError> FromIdentity(IEnumerable<IdentityError> errors) =>
        [.. errors
            .Select(e => e.Code switch
            {
                var code when code.StartsWith("Password", StringComparison.Ordinal)
                    => new ValidationError("Password", e.Description),
                "DuplicateEmail" or "DuplicateUserName"
                    => new ValidationError("Email", EmailTakenMessage),
                "InvalidEmail" or "InvalidUserName"
                    => new ValidationError("Email", "Enter a valid email."),
                _ => new ValidationError(string.Empty, e.Description),
            })
            .DistinctBy(e => (e.Field, e.Message))];
}
