using System;
using System.Diagnostics.CodeAnalysis;

namespace Core.Exceptions;

[ExcludeFromCodeCoverage]
public sealed class AccessDeniedException(string message, params object[] args) : CoreException(message, args)
{
    public override string ToString()
    {
        return $"{base.ToString()}, Parameters: {string.Join(", ", Parameters ?? Array.Empty<object>())}";
    }

}