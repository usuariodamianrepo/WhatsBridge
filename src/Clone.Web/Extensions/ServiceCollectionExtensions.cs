using Clone.DataAccess.Repositories;
using Clone.DataAccess.Repositories.IRepository;

namespace Clone.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDataAccessServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IRepositoryAsync<>), typeof(RepositoryAsync<>));

            services.AddScoped<ICompanyRepository, CompanyRepository>();
        }
    }
}
