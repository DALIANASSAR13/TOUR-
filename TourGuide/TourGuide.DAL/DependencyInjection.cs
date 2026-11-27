using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourGuide.DAL.Data;
using TourGuide.DAL.Repositories;
using TourGuide.DAL.Repositories.Interfaces;

namespace TourGuide.DAL;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("Default")));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IGuideProfileRepository, GuideProfileRepository>();
        services.AddScoped<IGuideDocumentRepository, GuideDocumentRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<UnitOfWork.IUnitOfWork, UnitOfWork.UnitOfWork>();

        return services;
    }
}
