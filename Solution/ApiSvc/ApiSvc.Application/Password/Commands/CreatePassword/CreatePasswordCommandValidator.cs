using FluentValidation;

namespace ApiSvc.Application.Password.Commands.CreatePassword
{
    public class CreatePasswordCommandValidator : AbstractValidator<CreatePasswordCommand>
    {
        public CreatePasswordCommandValidator()
        {
            RuleFor(item => item.CreatePasswordItemDto.Password)
                .MinimumLength(1)
                .MaximumLength(50)
                .NotEmpty();
            RuleFor(item => item.CreatePasswordItemDto.Title)
                .MinimumLength(1)
                .MaximumLength(200)
                .NotEmpty();
        }
    }
}
