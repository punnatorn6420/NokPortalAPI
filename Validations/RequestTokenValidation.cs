namespace NokPortalAPI.Validations
{
    using FluentValidation;
    using NokPortalAPI.Dtos;

    /// <summary>
    /// Validator for MicrosoftTokenDto.
    /// </summary>
    public class RequestTokenValidation : AbstractValidator<MicrosoftTokenDto>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestTokenValidation"/> class.
        /// </summary>
        public RequestTokenValidation()
        {
            RuleFor(a => a.Token).NotEmpty();
        }
    }
}