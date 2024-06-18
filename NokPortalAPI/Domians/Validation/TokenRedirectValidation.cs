namespace NokPortal.Domians.Validation
{
    using FluentValidation;
    using NokPortal.Domians.Models;

    public class TokenRedirectValidation : AbstractValidator<RequestToken>
    {
        public TokenRedirectValidation()
        {
            this.RuleFor(a => a.Token).NotEmpty();
        }
    }
}
