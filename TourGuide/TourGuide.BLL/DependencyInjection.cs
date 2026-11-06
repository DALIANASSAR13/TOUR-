using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourGuide.BLL.Options;

namespace TourGuide.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogic(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<FileStorageOptions>(config.GetSection(FileStorageOptions.SectionName));
        services.Configure<BookingJobOptions>(config.GetSection(BookingJobOptions.SectionName));

        services.AddSingleton(TimeProvider.System);

        // Every AbstractValidator<T> in this assembly is registered automatically.
        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        // Services are added here by later cycles, each as: services.AddScoped<IXService, XService>();

        return services;
    }
}
