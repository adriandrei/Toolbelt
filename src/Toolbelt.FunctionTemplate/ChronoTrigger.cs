using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Toolbelt.Cosmos.Interfaces;
using Toolbelt.Cosmos.Models;
using Toolbelt.Shared.Services;

namespace Toolbelt.FunctionTemplate;

public class ChronoTrigger
{
    private readonly IMyOtherService service;
    private readonly ICosmosRepository<SampleModel, CosmosRequest<SampleModel>> repo;
    private readonly ILogger<ChronoTrigger> logger;

    public ChronoTrigger(
        IMyOtherService service,
        ICosmosRepository<SampleModel, CosmosRequest<SampleModel>> repo,
        ILogger<ChronoTrigger> logger)
    {
        this.service = service;
        this.repo = repo;
        this.logger = logger;
    }

    [FunctionName("ChronoTrigger")]
    public async Task RunAsync([TimerTrigger("*/3 * * * * *")] TimerInfo myTimer)
    {
        await service.DoTheOtherThing();

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
            .CountAsync("sample", entry => entry.SomeNumber < 50);
        logger.LogInformation($"Successfully retrieved {allUnderFifty} entries under fifty");
    }
}