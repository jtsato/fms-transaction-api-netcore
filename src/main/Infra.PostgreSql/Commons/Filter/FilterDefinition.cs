using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Infra.PostgreSql.Commons.Filter;

public sealed class FilterDefinition
{
    public readonly string Property;
    public readonly SqlOperator SqlOperator;
    public readonly object Value;

    public FilterDefinition(string property, SqlOperator sqlOperator, object value)
    {
        Property = property;
        SqlOperator = sqlOperator;
        Value = value;
    }
    
    private bool Equals(FilterDefinition other)
    {
        return Property == other.Property 
               && SqlOperator == other.SqlOperator 
               && Equals(Value, other.Value);
    }
    
    [ExcludeFromCodeCoverage]
    public override bool Equals(object obj)
    {
        return obj is FilterDefinition other && Equals(other);
    }
    
    [ExcludeFromCodeCoverage]    
    public override int GetHashCode()
    {
        return HashCode.Combine(Property, SqlOperator.Id, Value);
    }
    
    [ExcludeFromCodeCoverage]    
    public override string ToString()
    {
        return new StringBuilder()
            .AppendLine($"Property: {Property} ")
            .AppendLine($"SqlOperator: {SqlOperator} ")
            .AppendLine($"Value: {Value} ")
            .ToString();
    }
}