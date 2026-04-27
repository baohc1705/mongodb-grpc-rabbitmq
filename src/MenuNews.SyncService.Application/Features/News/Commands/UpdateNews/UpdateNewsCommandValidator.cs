using FluentValidation;

namespace MenuNews.SyncService.Application.Features.News.Commands.UpdateNews;

public class UpdateNewsCommandValidator : AbstractValidator<UpdateNewsCommand>
{
    public UpdateNewsCommandValidator()
    {
        RuleFor(v => v.Id)
            .NotEmpty().WithMessage("News ID is required.");

        RuleFor(v => v.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(v => v.Slug)
            .NotEmpty().WithMessage("Slug is required.")
            .MaximumLength(200).WithMessage("Slug must not exceed 200 characters.");

        RuleFor(v => v.Summary)
            .NotEmpty().WithMessage("Summary is required.")
            .MaximumLength(500).WithMessage("Summary must not exceed 500 characters.");

        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Content is required.");
    }
}
