namespace NokPortalAPI.Domains.Validation
{
    using FluentValidation;
    using NokPortalAPI.Domains.Models;

    public class RequestTokenValidation : AbstractValidator<RequestMicrosoftToken>
    {
        public RequestTokenValidation()
        {
            this.RuleFor(a => a.Token).NotEmpty();
        }
    }
}
