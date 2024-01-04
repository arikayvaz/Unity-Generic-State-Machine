namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.TextCore.Text;

    public class StateIdle : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.Idle;

        public StateIdle(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }

        private StateInfo info;

        public override void OnEnter(StateInfo info)
        {
            base.OnEnter(info);

            info.stateDisplay.UpdateStateDisplay(StateId.ToString());

            this.info = info;

            SubscribeInputs(info);
        }

        public override void OnExit(StateInfo info)
        {
            base.OnExit(info);

            UnsubscribeInputs(info);

            this.info = null;
        }

        public override void SubscribeInputs(StateInfo info)
        {
            base.SubscribeInputs(info);

            info.characterInput.Player.Move.started += MoveInputStarted;
            info.characterInput.Player.Jump.started += JumpInputStarted;
        }

        public override void UnsubscribeInputs(StateInfo info)
        {
            base.UnsubscribeInputs(info);

            info.characterInput.Player.Move.started -= MoveInputStarted;
            info.characterInput.Player.Jump.started -= JumpInputStarted;
        }

        private void MoveInputStarted(InputAction.CallbackContext callback)
        {
            stateMachine.ChangeState(StateIds.Move);
        }

        private void JumpInputStarted(InputAction.CallbackContext callback) 
        {
            if (!info.Grounded)
                return;

            stateMachine.ChangeState(StateIds.Jump);
        }
    }
}
