using FluentValidation;
using TourGuide.BLL.Domain;
using TourGuide.BLL.DTOs.Review;

namespace TourGuide.BLL.Validators;

// FR-19: rating is a whole number from MinRating to MaxRating; the comment is optional, at most MaxCommentLength.
// Ownership, status and "one review per booking" are rules of ReviewService, not of this validator.
public class CreateReviewValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.BookingId).GreaterThan(0).WithMessage("Choose a booking to review.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(BookingRules.MinRating, BookingRules.MaxRating)
            .WithMessage($"Choose {BookingRules.MinRating} to {BookingRules.MaxRating} stars.");

        RuleFor(x => x.Comment)
            .MaximumLength(BookingRules.MaxCommentLength)
            .WithMessage($"Write at most {BookingRules.MaxCommentLength} characters.");
    }
}
