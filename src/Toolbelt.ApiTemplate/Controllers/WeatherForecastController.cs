using Microsoft.AspNetCore.Mvc;
using Toolbelt.Cosmos.Interfaces;
using Toolbelt.Cosmos.Models;
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
    private readonly ICosmosRepository<SampleModel, CosmosRequest<SampleModel>> repo;
    private readonly ILogger<WeatherForecastController> logger;

    public WeatherForecastController(
        IMyService myService,
        ICosmosRepository<SampleModel, CosmosRequest<SampleModel>> repo,
        ILogger<WeatherForecastController> logger)
    {
        this.myService = myService;
        this.repo = repo;
        this.logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> GetAsync()
    {
        var details = await this.myService.DoThat();

        var rnd = new Random();
        var randomNunmber = rnd.Next(100);
        var toAdd = new SampleModel("Title", randomNunmber);

        await repo.CreateAsync(toAdd);
        logger.LogInformation($"Successfully added new {nameof(SampleModel)}: {randomNunmber}");

        var added = await repo.GetAsync(new CosmosRequest<SampleModel>(toAdd.Id, toAdd.PartitionKey));
        logger.LogInformation($"Successfully retrieved added item {toAdd.Id} with number {added.SomeNumber}");

        var newRandom = rnd.Next(100);
        await repo.UpdateAsync(
            new CosmosRequest<SampleModel>(toAdd.Id, toAdd.PartitionKey),
            existing =>
            {
                existing.SomeNumber = newRandom;
            });
        logger.LogInformation($"Succesfully upserted item {toAdd.Id} with number {newRandom}");

        var allUnderFifty = await this.repo
            .ListAsync("sample", entry => entry.SomeNumber < 50);
        logger.LogInformation($"Successfully retrieved {allUnderFifty.Count} entries under fifty");

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
