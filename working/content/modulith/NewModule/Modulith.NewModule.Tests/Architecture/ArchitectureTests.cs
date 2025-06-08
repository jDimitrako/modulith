using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using System.Reflection;

namespace Modulith.NewModule.Tests.Architecture;

public class ArchitectureTests
{
    private static readonly ArchUnitNET.Domain.Architecture Architecture =
        new ArchLoader().LoadAssemblies(Assembly.LoadFrom("Modulith.NewModule.Api.dll"),
                                         Assembly.LoadFrom("Modulith.NewModule.Application.dll"),
                                         Assembly.LoadFrom("Modulith.NewModule.Infrastructure.dll"),
                                         Assembly.LoadFrom("Modulith.NewModule.dll"), // Domain project
                                         Assembly.LoadFrom("Modulith.SharedKernel.dll"))
                          .Build();

    private readonly IArchRule _domainLayerRule = Classes().That().ResideInNamespace("Modulith.NewModule.Domain..")
        .Should().NotDependOnAny(Classes().That().ResideInNamespace("Modulith.NewModule.Infrastructure.."))
        .Because("Domain layer should be independent of infrastructure concerns.");

    private readonly IArchRule _applicationLayerRule = Classes().That().ResideInNamespace("Modulith.NewModule.Application..")
        .Should().NotDependOnAny(Classes().That().ResideInNamespace("Modulith.NewModule.Api.."))
        .Because("Application layer should not depend on API layer.");

    private readonly IArchRule _infrastructureLayerRule = Classes().That().ResideInNamespace("Modulith.NewModule.Infrastructure..")
        .Should().DependOnAny(Classes().That().ResideInNamespace("Modulith.NewModule.Application.."))
        .Because("Infrastructure layer implements application interfaces.");

    private readonly IArchRule _apiLayerRule = Classes().That().ResideInNamespace("Modulith.NewModule.Api..")
        .Should().DependOnAny(Classes().That().ResideInNamespace("Modulith.NewModule.Application.."))
        .Because("API layer depends on application layer to send commands/queries.");

    private readonly IArchRule _noCircularDependenciesRule = CycleDetectionOption.NoCycles().Should().BeTrue();


    [ArchTest]
    public void DomainLayer_Should_Not_Depend_On_Infrastructure(IArchRule rule)
    {
        _domainLayerRule.Check(Architecture);
    }

    [ArchTest]
    public void ApplicationLayer_Should_Not_Depend_On_Api(IArchRule rule)
    {
        _applicationLayerRule.Check(Architecture);
    }

    [ArchTest]
    public void InfrastructureLayer_Should_Depend_On_Application(IArchRule rule)
    {
        _infrastructureLayerRule.Check(Architecture);
    }

    [ArchTest]
    public void ApiLayer_Should_Depend_On_Application(IArchRule rule)
    {
        _apiLayerRule.Check(Architecture);
    }

    [ArchTest]
    public void NoCircularDependencies(IArchRule rule)
    {
        _noCircularDependenciesRule.Check(Architecture);
    }
} 