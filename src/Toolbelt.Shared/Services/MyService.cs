namespace Toolbelt.Shared.Services;

public interface IMyService
{
    Task<string> DoThat();
}

public class MyService : IMyService
{
    private readonly string _key;
    public MyService(Options options)
    {
        this._key = options.SomeKey;
    }

    public async Task<string> DoThat()
    {
        await Task.Delay(100);
        return _key;
    }
}