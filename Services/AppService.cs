using NokAir.Core.Exceptions;
using NokPortalAPI.Dtos;
using NokPortalAPI.Extensions;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="AppService"/> class.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="appRepository"></param>
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
                var app = await appRepository.FindAppByIdAsync(id);
                if (app == null)
                {
                    return false;
                }
                await appRepository.RemoveAppByIdAsync(id);
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
            var app = await appRepository.FindAppByIdAsync(id);
            return app?.ToDto();
        }

        /// <inheritdoc />
        public async Task<(IList<AppDto> Items, int TotalRecords)> GetAppsByCriteriaAsync(AppSearchDto searchCriteria)
        {
            var (apps, total) = await appRepository.FindAppsByCriteriaAsync(searchCriteria);
            return (apps.Select(a => a.ToDto()).ToList(), total);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateAppAsync(AppDto appDto)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var existingApp = await appRepository.FindAppByIdAsync(appDto.Id);
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