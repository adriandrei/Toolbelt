using Toolbelt.Cosmos;
using Toolbelt.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddShared(options =>
{
    var secretKey = builder.Configuration.GetValue<string>("SecretKey");
    options.SomeKey = secretKey;
});

builder.Services.AddCosmos(options =>
{
    options.CosmosEndpoint = builder.Configuration.GetValue<string>("Cosmos:Endpoint");
    options.CosmosKey = builder.Configuration.GetValue<string>("Cosmos:CosmosKey");
    options.DatabaseId = builder.Configuration.GetValue<string>("Cosmos:DatabaseId");
    options.ContainerName = builder.Configuration.GetValue<string>("Cosmos:ContainerName");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
