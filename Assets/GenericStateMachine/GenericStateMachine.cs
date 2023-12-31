namespace ayvazarik.GenericStateMachine
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GenericStateMachine<TStateIds, TStateInfo> : MonoBehaviour
        where TStateIds : System.Enum
        where TStateInfo : GenericStateInfo
    {
        private Dictionary<TStateIds, GenericStateBase<TStateIds, TStateInfo>> dictStates = null;

        public GenericStateBase<TStateIds, TStateInfo> State => state;
        private GenericStateBase<TStateIds, TStateInfo>state = null;
        private TStateInfo stateInfo = null;

        public void InitStateMachine(TStateInfo stateInfo, IEnumerable<GenericStateBase<TStateIds, TStateInfo>> initStates)
        {
            this.stateInfo = stateInfo;

            if (dictStates == null)
                dictStates = new Dictionary<TStateIds, GenericStateBase<TStateIds, TStateInfo>>();
            else if (dictStates.Count > 0)
                dictStates.Clear();

            foreach (GenericStateBase<TStateIds, TStateInfo> state in initStates)
            {
                dictStates.Add(state.StateId, state);
            }//foreach (TStateIds stateId in stateIds)
        }

        private bool isChangeStateLocked = false;
        private List<TStateIds> changeStateIds = new List<TStateIds>();

        public void ChangeState(TStateIds stateId)
        {
            if (isChangeStateLocked)
            {
                // ChangeState called inside change state funcion(OnExit,OnEnter etc)
                // wait for other ChangeState function execution
                changeStateIds.Add(stateId);
                return;
            }//if (isChangeStateLocked)

            isChangeStateLocked = true;

            GenericStateBase<TStateIds, TStateInfo> stateNew = GetStateBase(stateId);

            if (stateNew == null)
                return;

            State?.OnExit(stateInfo);

            stateNew.OnEnter(stateInfo);

            state = stateNew;

            isChangeStateLocked = false;

            if (changeStateIds.Count > 0)
            {
                //we have new ChangeState request during execution -> handle it
                TStateIds changeStateId = changeStateIds[0];
                changeStateIds.RemoveAt(0);
                ChangeState(changeStateId);
            }
        }

        private GenericStateBase<TStateIds, TStateInfo> GetStateBase(TStateIds stateId) 
        {
            GenericStateBase<TStateIds, TStateInfo> state = null;

            if (dictStates == null || dictStates.Count <= 0)
            {
                Debug.LogError("StateMachine: state dictionary is null or empty " + this.name);
                return state;
            }

            dictStates.TryGetValue(stateId, out state);

            if (state == null) 
            {
                Debug.LogError("StateMachine:StateBase missing state for state id : " + stateId.ToString() + " " + this.name);
            }

            return state;
        }

        private void Update()
        {
            if (stateInfo == null)
                return;

            State?.OnUpdate(stateInfo);
        }

        private void FixedUpdate()
        {
            if (stateInfo == null)
                return;

            State?.OnFixedUpdate(stateInfo);
        }

        private void LateUpdate()
        {
            if (stateInfo == null)
                return;

            State?.OnLateUpdate(stateInfo);
        }
    }
}

