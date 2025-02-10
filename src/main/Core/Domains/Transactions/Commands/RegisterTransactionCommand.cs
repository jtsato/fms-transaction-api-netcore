using System;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Core.Domains.Transactions.Commands;

[ExcludeFromCodeCoverage]
public sealed class RegisterTransactionCommand
{
    private static readonly RegisterTransactionCommandValidator CommandValidator = new RegisterTransactionCommandValidator();
    
    public string Description { get; }
    public string Amount { get; }
    public string Type { get; }
    public string Date { get; }
    
    public RegisterTransactionCommand(string description, string amount, string type, string date)
    {
        Description = description;
        Amount = amount;
        Type = type;
        Date = date;
        CommandValidator.ValidateAndThrow(this);
    }
    
    private bool Equals(RegisterTransactionCommand other)
    {
        return Description == other.Description 
               && Amount == other.Amount 
               && Type == other.Type 
               && Date == other.Date;
    }
    
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is RegisterTransactionCommand other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Description, Amount, Type, Date);
    }

    public override string ToString()
    {
        return $"{nameof(Description)}: {Description}, {nameof(Amount)}: {Amount}, {nameof(Type)}: {Type}, {nameof(Date)}: {Date}";
    }
}
