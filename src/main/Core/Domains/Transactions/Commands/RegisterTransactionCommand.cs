using System;
using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace Core.Domains.Transactions.Commands;

[ExcludeFromCodeCoverage]
public sealed class RegisterTransactionCommand
{
    private static readonly RegisterTransactionCommandValidator CommandValidator = new RegisterTransactionCommandValidator();
    
    public string Description { get; set; }
    public string Amount { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
    public string Date { get; set; }
    
    public RegisterTransactionCommand(string description, string amount, string type, string status, string date)
    {
        Description = description;
        Amount = amount;
        Type = type;
        Status = status;
        Date = date;
        CommandValidator.ValidateAndThrow(this);
    }
    
    private bool Equals(RegisterTransactionCommand other)
    {
        return Description == other.Description 
               && Amount == other.Amount 
               && Type == other.Type 
               && Status == other.Status 
               && Date == other.Date;
    }
    
    public override bool Equals(object obj)
    {
        return ReferenceEquals(this, obj) || obj is RegisterTransactionCommand other && Equals(other);
    }
    
    public override int GetHashCode()
    {
        return HashCode.Combine(Description, Amount, Type, Status, Date);
    }

    public override string ToString()
    {
        return $"{nameof(Description)}: {Description}, {nameof(Amount)}: {Amount}, {nameof(Type)}: {Type}, {nameof(Status)}: {Status}, {nameof(Date)}: {Date}";
    }
}