namespace NokPortal.Domians.Services
{
    public interface IManagePayloadService
    {
        public string GetJsonPropertyName<T>(string propertyName);

        int GetUserIdFromJwtDecode(HttpContext payload);
    }
}
