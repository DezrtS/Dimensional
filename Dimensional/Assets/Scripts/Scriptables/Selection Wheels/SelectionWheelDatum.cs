using UnityEngine;
using User_Interface.Selection_Wheels;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "SelectionWheelDatum", menuName = "Scriptable Objects/Selection Wheels/SelectionWheelDatum")]
    public class SelectionWheelDatum : ScriptableObject
    {
        [SerializeField] private bool hasSelectionWheelPrefab;
        [SerializeField] private GameObject selectionWheelPrefab;
        [SerializeField] private SelectionWheelSettingsDatum selectionWheelSettingsDatum;
        [SerializeField] private WheelSectionDatum[] wheelSectionData;
        
        public bool HasSelectionWheelPrefab => hasSelectionWheelPrefab;
        public GameObject SelectionWheelPrefab => selectionWheelPrefab;
        public SelectionWheelSettingsDatum SelectionWheelSettingsDatum => selectionWheelSettingsDatum;
        public WheelSectionDatum[] WheelSectionData => wheelSectionData;
        
        public SelectionWheel AttachSelectionWheel(GameObject selectionWheelHolder)
        {
            var selectionWheel = selectionWheelHolder.AddComponent<SelectionWheel>();
            selectionWheel.Initialize(this);
            return selectionWheel;
        }
    }
}
