using Core.Commons;
using Core.Domains.Transactions.Models;
using FluentValidation;

namespace Core.Domains.Transactions.Commands;

internal sealed class RegisterTransactionCommandValidator : AbstractValidator<RegisterTransactionCommand>
{
    public RegisterTransactionCommandValidator()
    {
        RuleFor(command => command.Description)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("ValidationTransactionDescriptionIsNullOrEmpty");
        
        RuleFor(command => command.Amount)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("ValidationTransactionAmountIsNullOrEmpty")
            .Must(ArgumentChecker.IsDouble)
            .WithMessage("ValidationTransactionAmountIsNotDouble");
        
        RuleFor(command => command.Type)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("ValidationTransactionTypeIsNullOrEmpty")
            .Must(ArgumentChecker.IsValidEnumOf<Type>)
            .WithMessage("ValidationTransactionTypeIsInvalid");
        
        RuleFor(command => command.Status)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("ValidationTransactionStatusIsNullOrEmpty")
            .Must(ArgumentChecker.IsValidEnumOf<Status>)
            .WithMessage("ValidationTransactionStatusIsInvalid");
        
        RuleFor(command => command.Date)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("ValidationTransactionDateIsNullOrEmpty")
            .Must(ArgumentChecker.BeEmptyOrAValidDate)
            .WithMessage("ValidationTransactionDateIsInvalid");
    }
}