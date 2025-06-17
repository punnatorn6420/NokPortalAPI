using NokAir.Core.Exceptions;
using NokPortalAPI.Extensions;
using NokPortalAPI.Dtos;
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
        public async Task<AppDto> AddAppAsync(AppDto appDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var app = appDto.ToEntity();
                await appRepository.AddAppAsync(app);
                await transaction.CommitAsync();
                return app.ToDto();
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
        public async Task<AppDto?> GetAppByIdAsync(int id)
        {
            var app = await appRepository.GetAppByIdAsync(id);
            return app?.ToDto();
        }

        /// <inheritdoc />
        public async Task<IList<AppDto>> GetAppsByCriteriaAsync(AppSearchDto searchCriteria)
        {
            var apps = await appRepository.GetAppsByCriteriaAsync(searchCriteria);
            return apps.Select(app => app.ToDto()).ToList();
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAppAsync(AppDto appDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingApp = await appRepository.GetAppByIdAsync(appDto.Id);
                if (existingApp == null)
                {
                    return false;
                }

                existingApp.Name = appDto.Name;
                existingApp.Header = appDto.Header;
                existingApp.Subheader = appDto.Subheader;
                existingApp.EnvironmentType = appDto.EnvironmentType;
                existingApp.ClientUrl = appDto.ClientUrl;
                existingApp.BackendUrl = appDto.BackendUrl;
                existingApp.ImageUrl = appDto.ImageUrl;
                existingApp.SecretKey = appDto.SecretKey;
                existingApp.JwtExpiryHours = appDto.JwtExpiryHours;
                existingApp.Remark = appDto.Remark;
                existingApp.Active = appDto.Active;
                existingApp.ModifiedAt = DateTime.Now;
                await appRepository.UpdateAppAsync(existingApp);
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
