using Microsoft.AspNetCore.Mvc;
using Toolbelt.Shared.Services;

namespace Toolbelt.ApiTemplate.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    private readonly IMyService myService;
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(
        IMyService myService,
        ILogger<WeatherForecastController> logger)
    {
        this.myService = myService;
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        var details = await this.myService.DoThat();
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateTime.Now.AddDays(index),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)],
            Details = details
        })
        .ToArray();
    }
}
