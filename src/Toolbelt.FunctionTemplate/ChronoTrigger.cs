using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Toolbelt.Shared.Services;

namespace Toolbelt.FunctionTemplate;

public class ChronoTrigger
{
    private readonly IMyOtherService _service;

    public ChronoTrigger(
        IMyOtherService service)
    {
        _service = service;
    }
    
    [FunctionName("ChronoTrigger")]
    public async Task RunAsync([TimerTrigger("*/3 * * * * *")] TimerInfo myTimer)
    {
        await _service.DoTheOtherThing();
    }
}