namespace NokPortalAPI.Models.Validation
{
    using FluentValidation;
    using NokPortalAPI.Models;

    public class RequestTokenValidation : AbstractValidator<MicrosoftTokenRequest>
    {
        public RequestTokenValidation()
        {
            RuleFor(a => a.Token).NotEmpty();
        }
    }
}
