// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Global

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Infra.PostgreSQL.Commons.Repository;

namespace IntegrationTest.Infra.PostgreSql.Commons;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[Table("Employees")]
public sealed class RegularEntity : Entity
{
    [Required, MinLength(1), MaxLength(50)]
    [Column("name")]
    public string Name { get; init; }

    [Required, MinLength(1), MaxLength(50)]
    [Column("surname")]
    public string Surname { get; init; }

    [Required]
    [Column("birth_date", TypeName = "date")]
    public DateTime BirthDate { get; init; }

    public int Age => DateTime.Now.Year - BirthDate.Year;

    [Required]
    [Column("number_of_children")]
    public int NumberOfChildren { get; init; }

    [Required]
    [Column("salary", TypeName = "decimal(10,2)")]
    public decimal Salary { get; init; }
}

public sealed class NoTableEntity : Entity
{
    public string Name { get; init; }
}

public sealed class NoKeyEntity
{
    [Column("name")] public string Name { get; init; }
}

[Table("Empty")]
public sealed class EmptyEntity : Entity
{
}