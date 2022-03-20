using Microsoft.Extensions.Logging;

namespace Toolbelt.Shared.Services;

public interface IMyOtherService
{
    Task DoTheOtherThing();
}

public class MyOtherService : IMyOtherService
{
    private readonly IMyService _myService;
    private readonly ILogger<MyOtherService> _logger;

    public MyOtherService(
        IMyService myService,
        ILogger<MyOtherService> logger)
    {
        _myService = myService;
        _logger = logger;
    }

    public async Task DoTheOtherThing()
    {
        try
        {
            var result = await _myService.DoThat();
            _logger.LogInformation(result);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Fucked up");
            throw;
        }
    }
}