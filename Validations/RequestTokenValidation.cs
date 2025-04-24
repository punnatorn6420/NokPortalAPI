namespace NokPortalAPI.Validations
{
    using FluentValidation;
    using NokPortalAPI.Dtos;

    public class RequestTokenValidation : AbstractValidator<MicrosoftTokenDto>
    {
        public RequestTokenValidation()
        {
            RuleFor(a => a.Token).NotEmpty();
        }
    }
}
