using System.IO;
using Core.Commons.Extensions;
using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Microsoft.Extensions.Configuration;

namespace IntegrationTest.Infra.PostgreSql.Commons;

public sealed class DockerKeeper
{
    private static readonly string CurrentProjectName = typeof(DockerKeeper).Assembly.GetName().Name;

    private readonly ICompositeService _compositeService;
    private readonly bool _shouldStartContainerOnTest;

    public DockerKeeper(IConfiguration configuration)
    {
        _shouldStartContainerOnTest = configuration["SHOULD_START_CONTAINER_ON_TEST"] == "true";

        if (!_shouldStartContainerOnTest) return;
        
        string dockerComposePath = GetDockerComposeLocation(configuration["DOCKER_COMPOSE_FILE"]);
        
        _compositeService = new Builder()
            .UseContainer()
            .UseCompose()
            .FromFile(dockerComposePath)
            .RemoveOrphans()
            .NoBuild()
            .NoRecreate()
            .Build();
    }

    public void DockerComposeUp()
    {
        if (!_shouldStartContainerOnTest) return;
        _compositeService.Start();
    }

    private static string GetDockerComposeLocation(string dockerComposeFileName)
    {
        string projectRootFolder = Directory.GetCurrentDirectory().SubstringBefore(CurrentProjectName);
        return Path.Combine($"{projectRootFolder}{CurrentProjectName}", dockerComposeFileName);
    }
}