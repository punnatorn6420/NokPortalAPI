namespace NokPortalAPI.Validation
{
    using FluentValidation;
    using NokPortalAPI.Models;

    public class RequestTokenValidation : AbstractValidator<RequestMicrosoftToken>
    {
        public RequestTokenValidation()
        {
            RuleFor(a => a.Token).NotEmpty();
        }
    }
}
