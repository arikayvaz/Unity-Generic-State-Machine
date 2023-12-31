namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    using UnityEngine.Windows;

    public class StateIdle : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.Idle;

        public StateIdle(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }

        public override void OnEnter(StateInfo info)
        {
            base.OnEnter(info);

            Debug.Log("State Idle");

            //info._animator.SetFloat(info._animIDSpeed, 0f);
            info._animator.SetFloat(info._animIDMotionSpeed, 0f);
        }

        public override void OnUpdate(StateInfo info)
        {
            base.OnUpdate(info);

            HandleAnimationBlend(info);
            info.GroundedCheck();

            if (CheckJump(info))
                return;

            CheckMovement(info);
        }

        public override void OnLateUpdate(StateInfo info)
        {
            base.OnLateUpdate(info);

            info.CameraRotation();
        }

        private void CheckMovement(StateInfo info) 
        {
            float vel = info.input.move.magnitude;

            if (vel > 0.001f)
            {
                stateMachine.ChangeState(StateIds.Move);
            }
        }

        private void HandleAnimationBlend(StateInfo info) 
        {
            if (info._animationBlend <= 0f) 
            {
                return;
            }

            info._animationBlend = Mathf.Lerp(info._animationBlend, 0f, 10f * Time.deltaTime);
            info._animator.SetFloat(info._animIDSpeed, info._animationBlend);
            //info._animator.SetFloat(info._animIDMotionSpeed, inputMagnitude);
        }

        private bool CheckJump(StateInfo info) 
        {
            if (!info.Grounded)
                return false;

            if (info._jumpTimeoutDelta >= 0.0f)
            {
                info._jumpTimeoutDelta -= Time.deltaTime;
            }

            if (info.input.jump && info._jumpTimeoutDelta <= 0.0f)
            {
                stateMachine.ChangeState(StateIds.Jump);
                return true;
            }

            return false;
        }
    }
}
