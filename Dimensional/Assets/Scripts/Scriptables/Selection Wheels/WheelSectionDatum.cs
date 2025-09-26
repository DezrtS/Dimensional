using UnityEngine;
using User_Interface.Selection_Wheels;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "WheelSectionDatum", menuName = "Scriptable Objects/Selection Wheels/WheelSectionDatum")]
    public class WheelSectionDatum : ScriptableObject
    {
        [SerializeField] private GameObject wheelSectionPrefab;
        [Space]
        [SerializeField] private string sectionName;
        [SerializeField] private Sprite sectionIcon;
        
        public string SectionName => sectionName;
        public Sprite SectionIcon => sectionIcon;

        public virtual void Select(SelectionWheel selectionWheel, WheelSection wheelSection)
        {
            Debug.Log($"Selected {sectionName}");
        }

        public WheelSection AttachWheelSection(Transform parent, SelectionWheel selectionWheel)
        {
            var instance = Instantiate(wheelSectionPrefab, parent);
            var wheelSection = instance.GetComponent<WheelSection>();
            wheelSection.Initialize(this, selectionWheel);
            return wheelSection;
        }
    } 
}
