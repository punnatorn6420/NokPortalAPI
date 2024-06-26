namespace NokPortal.Domians.Validation
{
    using FluentValidation;
    using NokPortal.Domians.Models;

    public class RequestTokenValidation : AbstractValidator<RequestMicrosoftToken>
    {
        public RequestTokenValidation()
        {
            this.RuleFor(a => a.Token).NotEmpty();
        }
    }
}
