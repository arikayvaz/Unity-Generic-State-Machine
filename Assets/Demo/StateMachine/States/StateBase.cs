namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;

    public abstract class StateBase<TStateIds, TStateInfo> : GenericStateBase<TStateIds, TStateInfo>
        where TStateIds : System.Enum
        where TStateInfo : GenericStateInfo
    {
        protected StateBase(GenericStateMachine<TStateIds, TStateInfo> stateMachine) : base(stateMachine) { }
    }
}