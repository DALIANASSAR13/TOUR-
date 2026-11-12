using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TourGuide.BLL.DTOs.Account;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.DAL.Entities;
using TourGuide.PL.Extensions;
using TourGuide.PL.ViewModels;


namespace TourGuide.PL.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;

    private readonly IAccountService _accountService;

    private readonly ICountryProvider _countryProvider;

    public AccountController(
    SignInManager<ApplicationUser> signInManager,
    IAccountService accountService,
    ICountryProvider countryProvider)
    {
        _signInManager = signInManager;
        _accountService = accountService;
        _countryProvider = countryProvider;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl
        });
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) &&
                Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(
            string.Empty,
            "Invalid email or password."
        );

        return View(model);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login", "Account");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Register()
    {
        ViewBag.Countries = _countryProvider.GetCountries();

        return View(new RegisterTouristViewModel());
    }
  
    [AllowAnonymous]
    [HttpGet]
    public IActionResult RegisterGuide()
    {
        ViewBag.Countries = _countryProvider.GetCountries();

        return View(new RegisterGuideViewModel());
    }
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Register(RegisterTouristViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = _countryProvider.GetCountries();
            return View(model);
        }

        var request = new RegisterTouristRequest
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            CountryCode = model.CountryCode
        };

        var result = await _accountService.RegisterTouristAsync(request, ct);
        if (!result.Succeeded)
        {
            ModelState.AddResultErrors(result);
            ViewBag.Countries = _countryProvider.GetCountries();
            return View(model);
        }

        TempData["AccountMessage"] = "Your account is ready. Log in to continue.";
        return RedirectToAction(nameof(Login));
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RegisterGuide(RegisterGuideViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Countries = _countryProvider.GetCountries();
            return View(model);
        }

        using var residenceStream = model.ProofOfResidence!.OpenReadStream();
        using var governmentIdStream = model.GovernmentId!.OpenReadStream();
        using var criminalRecordStream = model.CriminalRecord!.OpenReadStream();

        var request = new RegisterGuideRequest
        {
            FullName = model.FullName,
            Email = model.Email,
            Password = model.Password,
            CountryCode = model.CountryCode,
            ProofOfResidence = new FileUpload { Content = residenceStream, FileName = model.ProofOfResidence.FileName, ContentType = model.ProofOfResidence.ContentType, SizeBytes = model.ProofOfResidence.Length },
            GovernmentId = new FileUpload { Content = governmentIdStream, FileName = model.GovernmentId.FileName, ContentType = model.GovernmentId.ContentType, SizeBytes = model.GovernmentId.Length },
            CriminalRecord = new FileUpload { Content = criminalRecordStream, FileName = model.CriminalRecord.FileName, ContentType = model.CriminalRecord.ContentType, SizeBytes = model.CriminalRecord.Length }
        };

        var result = await _accountService.RegisterGuideAsync(request, ct);
        if (!result.Succeeded)
        {
            ModelState.AddResultErrors(result);
            ViewBag.Countries = _countryProvider.GetCountries();
            return View(model);
        }

        TempData["AccountMessage"] = "Your guide account is created and waiting for review. Log in to follow its status.";
        return RedirectToAction(nameof(Login));
    }
}