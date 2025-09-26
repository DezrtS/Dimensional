using Managers;
using Systems.Actions.Movement;
using UnityEngine;
using User_Interface.Selection_Wheels;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "ActionWheelSectionDatum", menuName = "Scriptable Objects/Selection Wheels/ActionWheelSectionDatum")]
    public class ActionWheelSectionDatum : WheelSectionDatum
    {
        [SerializeField] private MovementActionType movementActionType;
        [SerializeField] private SelectionWheelDatum selectionWheelDatum;

        public override void Select(SelectionWheel selectionWheel, WheelSection wheelSection)
        {
            selectionWheel.DeactivateSelectionWheel(false);
            UIManager.Instance.ActivateShapeSelectionWheel(movementActionType, selectionWheelDatum);
        }
    }
}
