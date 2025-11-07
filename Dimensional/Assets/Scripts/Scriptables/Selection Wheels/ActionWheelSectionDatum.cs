using Managers;
using Systems.Actions.Movement;
using UnityEngine;
using User_Interface.Selection_Wheels;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "ActionWheelSectionDatum", menuName = "Scriptable Objects/Selection Wheels/ActionWheelSectionDatum")]
    public class ActionWheelSectionDatum : WheelSectionDatum
    {
        [SerializeField] private bool enableShapeSelectionWheelOnSelect;
        [SerializeField] private bool cancelSelectionWheelOnSelect;
        [SerializeField] private bool useOtherMethod;
        [SerializeField] private MovementActionType movementActionType;
        [SerializeField] private SelectionWheelDatum selectionWheelDatum;
        
        public bool EnableShapeSelectionWheelOnSelect => enableShapeSelectionWheelOnSelect;
        public bool UseOtherMethod => useOtherMethod;
        public SelectionWheelDatum SelectionWheelDatum => selectionWheelDatum;

        public override void Select(SelectionWheel selectionWheel, WheelSection wheelSection)
        {
            selectionWheel.Deactivate();
            UIManager.Instance.SelectedMovementActionType = movementActionType;
            if (cancelSelectionWheelOnSelect) selectionWheel.Cancel();
        }
    }
}
