using System;
using System.ComponentModel.DataAnnotations.Schema;
using Infra.PostgreSQL.Commons.Repository;

namespace Infra.PostgreSQL.Domains.Models;

[Table("change_requests")]
public sealed class ChangeRequestEntity : Entity
{
    [Column("configuration_id")]
    public long ConfigurationId { get; set; }
    
    [Column("initial_config")]
    public string InitialConfig { get; set; }
    
    [Column("final_config")]
    public string FinalConfig { get; set; }
    
    [Column("release_name")]
    public string ReleaseName { get; set; }
    
    [Column("status")]
    public string Status { get; set; }
    
    [Column("created_by")]
    public string CreatedBy { get; set; }
    
    [Column("updated_by")]
    public string UpdatedBy { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public override string ToString() => $"ChangeRequestEntity: {{ Id: {Id}, ConfigurationId: {ConfigurationId}, " +
        $"InitialConfig: {InitialConfig}, FinalConfig: {FinalConfig}, ReleaseName: {ReleaseName}, Status: {Status}, " +
        $"CreatedBy: {CreatedBy}, UpdatedBy: {UpdatedBy}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt} }}";
}