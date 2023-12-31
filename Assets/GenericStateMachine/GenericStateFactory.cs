namespace ayvazarik.GenericStateMachine
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public abstract class GenericStateFactory<TStateIds, TStateInfo, TStateMachine>
        where TStateIds : System.Enum
        where TStateInfo : GenericStateInfo
        where TStateMachine : GenericStateMachine<TStateIds, TStateInfo>
    {
        [SerializeField] TStateIds[] initialStates = null;

        public abstract GenericStateBase<TStateIds, TStateInfo> CreateState(TStateIds stateId, TStateMachine stateMachine);

        public virtual IEnumerable<GenericStateBase<TStateIds, TStateInfo>> GetStates(TStateMachine stateMachine) 
        {
            if (initialStates == null || initialStates.Length <= 0)
                yield return null;

            foreach (TStateIds stateId in initialStates)
                yield return CreateState(stateId, stateMachine);
        }
    }
}