namespace Toolbelt.Cosmos.Models;

public class SampleModel : CosmosEntity
{
    public SampleModel(
        string title,
        int someNumber)
    {
        this.Title = title;
        this.SomeNumber = someNumber;
    }

    public string Title { get; set; }
    public int SomeNumber { get; set; }
    public override string GetPartitionKey()
    {
        return "sample";
    }
}
