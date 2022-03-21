namespace Toolbelt.Abstractions.Entities;

public abstract class Entity
{
    public Entity(
        string? id = null)
    {
        if (id == null)
            id = Guid.NewGuid().ToString();

        this.Id = id;
    }

    public virtual string Id { get; protected set; }
}
