namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    using UnityEngine.Windows;

    public class StateJump : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.Jump;

        public StateJump(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }

        bool jump = false;
        float jumpTargetPosY;
        bool isJumpSuccess = false;

        public override void OnEnter(StateInfo info)
        {
            base.OnEnter(info);

            Debug.Log("State Jump");

            jump = true;
            isJumpSuccess = false;
            jumpTargetPosY = info.character.transform.position.y + info.JumpHeight;
            info._verticalVelocity = 0f;
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
            HandleMovement(info);

            if (info.character.transform.position.y >= jumpTargetPosY)
            {
                Debug.LogWarning("Jump Success");
                isJumpSuccess = true;
            }

            if (isJumpSuccess && info.Grounded && info.character.transform.position.y <= 0f)
            {
                Debug.LogWarning("Jump Complete");
                stateMachine.ChangeState(StateIds.Idle);
            }
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
                if (jump && info._jumpTimeoutDelta <= 0.0f)
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
                jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (info._verticalVelocity < info._terminalVelocity)
            {
                info._verticalVelocity += info.Gravity * Time.deltaTime;
            }
        }

        private void HandleMovement(StateInfo info) 
        {
            Vector3 motion = new Vector3(0f, info._verticalVelocity * Time.deltaTime, 0f);
            info._controller.Move(motion);
        }
    }
}
