#pragma warning disable ASPIRECERTIFICATES001
IDistributedApplicationBuilder builder = DistributedApplication.CreateBuilder(args);

IResourceBuilder<PostgresServerResource> postgres = builder
    .AddPostgres("Postgres")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(cfg => cfg.WithLifetime(ContainerLifetime.Persistent))
    ;

IResourceBuilder<RedisResource> redis = builder
    .AddRedis("Redis")
    .WithoutHttpsCertificate()
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(cfg => cfg.WithLifetime(ContainerLifetime.Persistent))
    ;

IResourceBuilder<RabbitMQServerResource> rabbitMq = builder
    .AddRabbitMQ("RabbitMq")
    .WithDataVolume()
    .WithLifetime(ContainerLifetime.Persistent)
    .WithManagementPlugin()
    ;

IResourceBuilder<ExternalServiceResource> kayaposoft = builder
    .AddExternalService("Kayaposoft", "https://kayaposoft.com/enrico/json/v2.0/")
    ;

builder.AddProject<Projects.CalendarApp_Background>("calendarapp-background")
    .WithReference(postgres).WaitFor(postgres)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    .WithReference(kayaposoft)
    ;

builder.AddProject<Projects.CalendarApp_Api>("calendarapp-api")
    .WithReference(redis).WaitFor(redis)
    .WithReference(rabbitMq).WaitFor(rabbitMq)
    ;

builder.Build().Run();
