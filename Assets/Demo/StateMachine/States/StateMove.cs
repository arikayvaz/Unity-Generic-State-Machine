namespace ayvazarik.Demo.DemoStateMachine
{
    using ayvazarik.GenericStateMachine;
    using UnityEngine;
    using UnityEngine.InputSystem;
    using UnityEngine.InputSystem.XR;
    using UnityEngine.Windows;

    public class StateMove : StateBase<StateIds, StateInfo>
    {
        public override StateIds StateId => StateIds.Move;

        public StateMove(GenericStateMachine<StateIds, StateInfo> stateMachine) : base(stateMachine) { }

        private Vector2 moveInput = Vector2.zero;
        private bool isSprint;

        public override void OnEnter(StateInfo info)
        {
            base.OnEnter(info);

            Debug.Log("State Move");

            moveInput = info.characterInput.Player.Move.ReadValue<Vector2>();
            isSprint = info.characterInput.Player.Sprint.IsPressed();

            SubscribeInputs(info);
        }

        public override void OnExit(StateInfo info)
        {
            base.OnExit(info);

            UnsubscribeInputs(info);

            info._animator.SetFloat(info._animIDSpeed, 0f);
            info._animator.SetFloat(info._animIDMotionSpeed, 0f);
        }

        public override void OnUpdate(StateInfo info)
        {
            base.OnUpdate(info);
            Move(info);
        }

        public override void SubscribeInputs(StateInfo info)
        {
            base.SubscribeInputs(info);

            info.characterInput.Player.Move.performed += MoveInputPerformed;
            info.characterInput.Player.Move.canceled += MoveInputCanceled;
            info.characterInput.Player.Sprint.performed += SprintInputPerformed;
        }

        public override void UnsubscribeInputs(StateInfo info)
        {
            base.UnsubscribeInputs(info);

            info.characterInput.Player.Move.performed -= MoveInputPerformed;
            info.characterInput.Player.Move.canceled -= MoveInputCanceled;
            info.characterInput.Player.Sprint.performed -= SprintInputPerformed;
        }

        private void MoveInputPerformed(InputAction.CallbackContext context)
        {
            moveInput = context.ReadValue<Vector2>();
        }

        private void MoveInputCanceled(InputAction.CallbackContext context) 
        {
            moveInput = Vector2.zero;
        }

        private void SprintInputPerformed(InputAction.CallbackContext context)
        {
            isSprint = context.ReadValueAsButton();
        }

        private void Move(StateInfo info) 
        {
            float targetSpeed = isSprint ? info.SprintSpeed : info.MoveSpeed;

            if (moveInput == Vector2.zero)
                targetSpeed = 0f;

            float currentHorizontalSpeed = new Vector3(info._controller.velocity.x, 0f, info._controller.velocity.z).magnitude;

            if (targetSpeed == 0f && currentHorizontalSpeed <= 0.01f)
            {
                stateMachine.ChangeState(StateIds.Idle);
                return;
            }

            float speedOffset = 0.1f;
            float inputMagnitude = moveInput.magnitude;
            float speed = 0f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * info.SpeedChangeRate);
            }
            else
            {
                speed = targetSpeed;
            }

            info._animationBlend = Mathf.Lerp(info._animationBlend, targetSpeed, Time.deltaTime * info.SpeedChangeRate);

            if (info._animationBlend < 0.01f)
                info._animationBlend = 0f;

            Vector3 inputDirection = new Vector3(moveInput.x, 0.0f, moveInput.y).normalized;

            if (moveInput != Vector2.zero)
            {
                info._targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  info._mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(info.character.transform.eulerAngles.y, info._targetRotation, ref info._rotationVelocity,
                    info.RotationSmoothTime);

                // rotate to face input direction relative to camera position
                info.character.transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, info._targetRotation, 0.0f) * Vector3.forward;

            Vector3 motion = targetDirection.normalized * (speed * Time.deltaTime) +
                             new Vector3(0.0f, info._verticalVelocity, 0.0f) * Time.deltaTime;

            info._controller.Move(motion);

            info._animator.SetFloat(info._animIDSpeed, info._animationBlend);
            info._animator.SetFloat(info._animIDMotionSpeed, inputMagnitude);
        }
        
    }
}
