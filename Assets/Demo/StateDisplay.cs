namespace ayvazarik.Demo
{
    using TMPro;
    using UnityEngine;

    public class StateDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text textDisplay = null;

        public void UpdateStateDisplay(string strState) 
        {
            textDisplay.text = $"State {strState}";
        }
    }
}