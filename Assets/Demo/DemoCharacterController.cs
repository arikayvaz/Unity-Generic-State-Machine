namespace ayvazarik.Demo
{
    using ayvazarik.Demo.DemoStateMachine;
    using UnityEngine;
    using UnityEngine.InputSystem.XR;
    using UnityEngine.Windows;

    public class DemoCharacterController : MonoBehaviour
    {
        [SerializeField] StateInfo stateInfo = null;
        [SerializeField] StateMachine stateMachine = null;
        [SerializeField] StateFactory stateFactory = null;

        private void Awake()
        {
            stateInfo.InitInfo();
            InitStateMachine();
            stateMachine.ChangeState(StateIds.Idle);
        }

        private void InitStateMachine() 
        {
            stateMachine.InitStateMachine(stateInfo, stateFactory.GetStates(stateMachine));
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (stateInfo.FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, stateInfo.FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(stateInfo.FootstepAudioClips[index], transform.TransformPoint(stateInfo._controller.center), stateInfo.FootstepAudioVolume);
                }
            }
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(stateInfo.LandingAudioClip, transform.TransformPoint(stateInfo._controller.center), stateInfo.FootstepAudioVolume);
            }
        }
    }
}