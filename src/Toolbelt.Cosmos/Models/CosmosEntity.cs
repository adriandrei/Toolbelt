using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Toolbelt.Abstractions.Entities;

namespace Toolbelt.Cosmos.Models;

public abstract class CosmosEntity : Entity
{
    public CosmosEntity(string id = null) : base(id) { }

    [Key]
    [JsonProperty("id")]
    public override string Id { get => base.Id; protected set => base.Id = value; }
    [JsonProperty("partitionKey")]
    public string PartitionKey { get; set; }
    [JsonProperty("_etag")]
    public string? Etag { get; set; }
    public abstract string GetPartitionKey();
}
