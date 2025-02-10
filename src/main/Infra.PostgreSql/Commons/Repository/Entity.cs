using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Infra.PostgreSql.Commons.Repository;

public abstract class Entity : IEntity
{
    [Key]
    [Column("id")]
    public long Id { get; set; }
}