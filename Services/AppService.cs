using NokAir.Core.Exceptions;
using NokPortalAPI.Models;
using NokPortalAPI.Repositories;

namespace NokPortalAPI.Services
{
    /// <summary>
    /// App service.
    /// </summary>
    public class AppService : IAppService
    {
        private readonly AppDbContext context;
        private readonly IAppRepository appRepository;

        public AppService(AppDbContext context, IAppRepository appRepository)
        {
            this.context = context;
            this.appRepository = appRepository;
        }

        /// <inheritdoc />
        public async Task<App> AddAppAsync(App app)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                await appRepository.AddAppAsync(app);
                await transaction.CommitAsync();
                return app;
            }
            catch (DataValidationException)
            {
                await transaction.RollbackAsync();
                throw;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> DeleteAppByIdAsync(int id)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var app = await appRepository.GetAppByIdAsync(id);
                if (app == null)
                {
                    return false;
                }
                await appRepository.DeleteAppByIdAsync(id);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<App?> GetAppByIdAsync(int id)
        {
           return await appRepository.GetAppByIdAsync(id);
        }

        /// <inheritdoc />
        public async Task<IList<App>> GetAppsByCriteriaAsync(AppSearchCriteria searchCriteria)
        {
            return await appRepository.GetAppsByCriteriaAsync(searchCriteria);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAppAsync(App app)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingApp = await appRepository.GetAppByIdAsync(app.Id);
                if (existingApp == null)
                {
                    return false;
                }
                await appRepository.UpdateAppAsync(app);
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
