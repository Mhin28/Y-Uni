using FluentValidation;
using API.DTOs.Ai;

namespace API.Validators;

public class ChatCreateRequestValidator : AbstractValidator<ChatCreateRequest>
{
    public ChatCreateRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");
    }
}