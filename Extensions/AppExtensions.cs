using NokPortalAPI.Dtos;
using NokPortalAPI.Entities;

namespace NokPortalAPI.Extensions
{
    public static class AppExtensions
    {
        /// <summary>
        /// Converts an AppDto to an App entity.
        /// </summary>
        /// <param name="appDto">AppDto object to convert.</param>
        public static App ToEntity(this AppDto appDto)
        {
            return new App
            {
                Id = appDto.Id,
                Name = appDto.Name,
                Header = appDto.Header,
                Subheader = appDto.Subheader,
                EnvironmentType = appDto.EnvironmentType,
                BaseUrl = appDto.BaseUrl,
                SecretKey = appDto.SecretKey,
                JwtExpiryHours = appDto.JwtExpiryHours,
                Remark = appDto.Remark,
                Active = appDto.Active
            };
        }

        public static AppDto ToDto(this App app)
        {
            return new AppDto
            {
                Id = app.Id,
                Name = app.Name,
                Header = app.Header,
                Subheader = app.Subheader,
                EnvironmentType = app.EnvironmentType,
                BaseUrl = app.BaseUrl,
                SecretKey = app.SecretKey,
                JwtExpiryHours = app.JwtExpiryHours,
                Remark = app.Remark,
                Active = app.Active
            };
        }
    }
}
