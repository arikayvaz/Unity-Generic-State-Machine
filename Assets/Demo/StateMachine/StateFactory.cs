namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;

    [System.Serializable]
    public class StateFactory : GenericStateFactory<StateIds, StateInfo, StateMachine>
    {
        public override GenericStateBase<StateIds, StateInfo> CreateState(StateIds stateId, StateMachine stateMachine)
        {
            switch (stateId)
            {
                case StateIds.None:
                    return new StateNone(stateMachine);
                case StateIds.Idle:
                    return new StateIdle(stateMachine);
                case StateIds.Move:
                    return new StateMove(stateMachine);
                case StateIds.Jump:
                    return new StateJump(stateMachine);
                default:
                    return null;
            }
        }
    }
}