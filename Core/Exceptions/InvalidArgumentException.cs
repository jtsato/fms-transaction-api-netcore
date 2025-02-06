using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Core.Commons.Models;

namespace Core.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class InvalidArgumentException(string message, IList<FieldError> fieldErrors, params object[] args) : CoreException(message, args)
{
    public IList<FieldError> FieldErrors { get; } = fieldErrors ?? new List<FieldError>();

    public override string ToString()
    {
        return $"{base.ToString()}, FieldErrors: {string.Join(", ", FieldErrors)}";
    }

    public string Serialize()
    {
        return JsonSerializer.Serialize(this);
    }

    public static ParentConstraintException Deserialize(string json)
    {
        return JsonSerializer.Deserialize<ParentConstraintException>(json);
    }
}