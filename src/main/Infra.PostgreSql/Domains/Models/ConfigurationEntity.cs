using System;
using System.ComponentModel.DataAnnotations.Schema;
using Infra.PostgreSQL.Commons.Repository;

namespace Infra.PostgreSQL.Domains.Models;

[Table("configurations")]
public sealed class ConfigurationEntity : Entity
{
    [Column("repository_id")]
    public long RepositoryId { get; set; }
    
    [Column("payload")]
    public string Payload { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}