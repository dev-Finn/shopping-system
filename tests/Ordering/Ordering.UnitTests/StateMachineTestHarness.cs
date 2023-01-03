using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ordering.Components.StateMachines;

namespace Ordering.UnitTests;

public abstract class StateMachineTestHarness<TState, TMachine> where TState : class, SagaStateMachineInstance where TMachine : class, SagaStateMachine<TState>, new()
{
    protected ITestHarness TestHarness { get; }
    protected ISagaStateMachineTestHarness<TMachine, TState> SagaHarness { get; }

    protected StateMachineTestHarness()
    {
        LogContext.ConfigureCurrentLogContext(new TestLogger());

        var provider = new ServiceCollection()
            .AddMassTransitTestHarness(cfg =>
            {
                cfg.AddSagaStateMachine<OrderStateMachine, OrderState>();
            })
            .BuildServiceProvider(true);
        TestHarness =  provider.GetRequiredService<ITestHarness>();
        SagaHarness = TestHarness.GetSagaStateMachineHarness<TMachine, TState>();
    }

    [SetUp]
    public async Task BaseSetup()
    {
        await TestHarness.Start();
    }

    [TearDown]
    public async Task BaseTeardown()
    {
        await TestHarness.Stop();
    }
}
