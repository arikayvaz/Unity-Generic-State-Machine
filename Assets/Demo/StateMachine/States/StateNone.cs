namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    public class StateNone : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.None;

        public StateNone(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }
    }
}