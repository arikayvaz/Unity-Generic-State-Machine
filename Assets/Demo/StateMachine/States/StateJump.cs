namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    using UnityEngine.InputSystem.XR;
    using UnityEngine.Windows;

    public class StateJump : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.Jump;

        public StateJump(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }

        public override void OnEnter(StateInfo info)
        {
            base.OnEnter(info);

            Debug.Log("State Jump");
        }

        public override void OnExit(StateInfo info)
        {
            base.OnExit(info);

            // reset the fall timeout timer
            info._fallTimeoutDelta = info.FallTimeout;

            // update animator if using character
            info._animator.SetBool(info._animIDJump, false);
            info._animator.SetBool(info._animIDFreeFall, false);
        }

        public override void OnUpdate(StateInfo info)
        {
            base.OnUpdate(info);

            JumpAndGravity(info);
            info.GroundedCheck();

            if (info._verticalVelocity <= 0f && info.Grounded)
                stateMachine.ChangeState(StateIds.Idle);
        }

        private void JumpAndGravity(StateInfo info)
        {
            if (info.Grounded)
            {
                // reset the fall timeout timer
                info._fallTimeoutDelta = info.FallTimeout;

                // update animator if using character
                info._animator.SetBool(info._animIDJump, false);
                info._animator.SetBool(info._animIDFreeFall, false);

                // stop our velocity dropping infinitely when grounded
                if (info._verticalVelocity < 0.0f)
                {
                    info._verticalVelocity = -2f;
                }

                // Jump
                if (info.input.jump && info._jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    info._verticalVelocity = Mathf.Sqrt(info.JumpHeight * -2f * info.Gravity);

                    // update animator if using character
                    info._animator.SetBool(info._animIDJump, true);
                }

                // jump timeout
                if (info._jumpTimeoutDelta >= 0.0f)
                {
                    info._jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                info._jumpTimeoutDelta = info.JumpTimeout;

                // fall timeout
                if (info._fallTimeoutDelta >= 0.0f)
                {
                    info._fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    info._animator.SetBool(info._animIDFreeFall, true);
                }

                // if we are not grounded, do not jump
                info.input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (info._verticalVelocity < info._terminalVelocity)
            {
                info._verticalVelocity += info.Gravity * Time.deltaTime;
            }

            info._controller.Move(new Vector3(0.0f, info._verticalVelocity, 0.0f) * Time.deltaTime);
        }
    }
}
