using FluentValidation;

namespace MenuNews.SyncService.Application.Features.Menus.Commands.UpdateMenu;

public class UpdateMenuCommandValidator : AbstractValidator<UpdateMenuCommand>
{
    public UpdateMenuCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("Menu ID is required.");

        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Menu Name is required.")
            .MaximumLength(100).WithMessage("Menu Name must not exceed 100 characters.");

        RuleFor(v => v.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");

        RuleFor(v => v.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display Order must be greater than or equal to 0.");
    }
}
