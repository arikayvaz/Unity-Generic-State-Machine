namespace ayvazarik.GenericStateMachine
{
    using UnityEngine;

    public abstract class GenericStateBase<TStateIds, TStateInfo>
        where TStateIds : System.Enum
        where TStateInfo : GenericStateInfo
    {
        protected GenericStateMachine<TStateIds, TStateInfo> stateMachine = null;

        public abstract TStateIds StateId { get; }

        public GenericStateBase(GenericStateMachine<TStateIds, TStateInfo> stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        public virtual void OnEnter(TStateInfo info) { }
        public virtual void OnExit(TStateInfo info) { }
        public virtual void OnUpdate(TStateInfo info) { }
        public virtual void OnFixedUpdate(TStateInfo info) { }
        public virtual void OnLateUpdate(TStateInfo info) { }
    }
}