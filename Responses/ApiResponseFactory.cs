using Microsoft.Extensions.Localization;
using NokCore.Api.Responses.Web;
using NokCore.Resources.ServiceLocalize;
using NokPortalAPI.Resources;

namespace NokPortalAPI.Responses
{
    public class ApiResponseFactory : BaseApiResponseFactory<ApiResponseLocalize>
    {
        public ApiResponseFactory(
            IStringLocalizer<ApiResponseLocalize> localizer,
            IStringLocalizer<ServiceLocalize> systemLocalizer)
            : base(localizer, systemLocalizer)
        {
        }
    }
}
