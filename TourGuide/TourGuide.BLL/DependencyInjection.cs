using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourGuide.BLL.Domain;
using TourGuide.BLL.Domain.Interfaces;
using TourGuide.BLL.Infrastructure;
using TourGuide.BLL.Options;
using TourGuide.BLL.Services;
using TourGuide.BLL.Services.Interfaces;
using TourGuide.BLL.Services.Account;
using TourGuide.BLL.Validators;


namespace TourGuide.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<FileStorageOptions>(config.GetSection(FileStorageOptions.SectionName));
        services.Configure<BookingJobOptions>(config.GetSection(BookingJobOptions.SectionName));

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IPriceCalculator, PriceCalculator>();
        services.AddSingleton<IBookingStateMachine, BookingStateMachine>();
        services.AddSingleton<ICountryProvider, CountryProvider>();

        // Every AbstractValidator<T> in this assembly is registered automatically.
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        // Services are added here by later cycles, each as: services.AddScoped<IXService, XService>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IGuideProfileService, GuideProfileService>();
        services.AddScoped<IGuideVerificationService, GuideVerificationService>();
        services.AddScoped<ISearchService, SearchService>();
        services.AddScoped<IGuideProfileService, GuideProfileService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReviewService, ReviewService>();

        return services;
    }
}
