namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    using UnityEngine.InputSystem.XR;
    using UnityEngine.Windows;

    public class StateMove : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.Move;

        public StateMove(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }

        public override void OnEnter(StateInfo info)
        {
            base.OnEnter(info);

            Debug.Log("State Move");
        }

        public override void OnUpdate(StateInfo info)
        {
            base.OnUpdate(info);

            if (CheckMovement(info))
                return;

            Move(info);
        }

        public override void OnLateUpdate(StateInfo info)
        {
            base.OnLateUpdate(info);

            info.CameraRotation();
        }

        private bool CheckMovement(StateInfo info)
        {
            float vel = info.input.move.magnitude;

            if (vel < 0.01f)
            {
                stateMachine.ChangeState(StateIds.Idle);
                return true;
            }

            return false;
        }
        private void Move(StateInfo info)
        {
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed = info.input.sprint ? info.SprintSpeed : info.MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (info.input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(info._controller.velocity.x, 0.0f, info._controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = info.input.analogMovement ? info.input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                info._speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * info.SpeedChangeRate);

                // round speed to 3 decimal places
                info._speed = Mathf.Round(info._speed * 1000f) / 1000f;
            }
            else
            {
                info._speed = targetSpeed;
            }

            info._animationBlend = Mathf.Lerp(info._animationBlend, targetSpeed, Time.deltaTime * info.SpeedChangeRate);
            if (info._animationBlend < 0.01f) info._animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(info.input.move.x, 0.0f, info.input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (info.input.move != Vector2.zero)
            {
                info._targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  info._mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(info.character.transform.eulerAngles.y, info._targetRotation, ref info._rotationVelocity,
                    info.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                info.character.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, info._targetRotation, 0.0f) * Vector3.forward;

            // move the player
            info._controller.Move(targetDirection.normalized * (info._speed * Time.deltaTime) +
                             new Vector3(0.0f, info._verticalVelocity, 0.0f) * Time.deltaTime);

            info._animator.SetFloat(info._animIDSpeed, info._animationBlend);
            info._animator.SetFloat(info._animIDMotionSpeed, inputMagnitude);
        }
    }
}
