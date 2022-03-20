namespace Toolbelt.Abstractions.Entities;

public abstract class EntityRequest
{
    public EntityRequest(string? id = null)
    {
        Id = id;
    }
    public string? Id { get; set; }
}
