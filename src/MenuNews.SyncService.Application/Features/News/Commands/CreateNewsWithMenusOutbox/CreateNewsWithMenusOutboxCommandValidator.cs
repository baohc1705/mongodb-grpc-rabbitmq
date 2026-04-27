using FluentValidation;

namespace MenuNews.SyncService.Application.Features.News.Commands.CreateNewsWithMenusOutbox;

public class CreateNewsWithMenusOutboxCommandValidator : AbstractValidator<CreateNewsWithMenusOutboxCommand>
{
    public CreateNewsWithMenusOutboxCommandValidator()
    {
        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("News Title is required.")
            .MaximumLength(200).WithMessage("News Title must not exceed 200 characters.");

        RuleFor(v => v.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(200).WithMessage("Slug must not exceed 200 characters.");

        RuleFor(v => v.Summary)
            .NotEmpty().WithMessage("Summary is required.")
            .MaximumLength(500).WithMessage("Summary must not exceed 500 characters.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.");

        RuleForEach(v => v.MenuItems).SetValidator(new MenusItemRequestValidator());
    }
}

public class MenusItemRequestValidator : AbstractValidator<MenusItemRequest>
{
    public MenusItemRequestValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Menu Name is required.")
            .MaximumLength(100).WithMessage("Menu Name must not exceed 100 characters.");

        RuleFor(v => v.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(100).WithMessage("Slug must not exceed 100 characters.");

        RuleFor(v => v.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display Order must be greater than or equal to 0.");

        RuleFor(v => v.NewsMenuDisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("News Menu Display Order must be greater than or equal to 0.");
    }
}
