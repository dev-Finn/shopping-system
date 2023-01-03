using MassTransit;
using MassTransit.Testing;

namespace Shared.NUnit;

public abstract class StateMachineTestHarness<TMachine, TState> : MassTransitTestHarness where TState : class, SagaStateMachineInstance where TMachine : class, SagaStateMachine<TState>, new()
{
    protected ISagaStateMachineTestHarness<TMachine, TState> SagaHarness { get; }

    protected StateMachineTestHarness()
    {
        SagaHarness = TestHarness.GetSagaStateMachineHarness<TMachine, TState>();
    }

    protected override void ConfigureTestHarness(IBusRegistrationConfigurator busRegistrationConfigurator)
    {
        busRegistrationConfigurator.AddSagaStateMachine<TMachine, TState>();
    }
}
