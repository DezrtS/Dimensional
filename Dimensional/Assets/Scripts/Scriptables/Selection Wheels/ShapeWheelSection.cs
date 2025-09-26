using Managers;
using Scriptables.Shapes;
using Systems.Player;
using UnityEngine;
using User_Interface.Selection_Wheels;

namespace Scriptables.Selection_Wheels
{
    [CreateAssetMenu(fileName = "ShapeWheelSection", menuName = "Scriptable Objects/Selection Wheels/ShapeWheelSection")]
    public class ShapeWheelSection : WheelSectionDatum
    {
        [SerializeField] private ShapeType shapeType;
        
        public override void Select(SelectionWheel selectionWheel, WheelSection wheelSection)
        {
            var instance = PlayerController.Instance;
            instance.SetMovementActionShape(UIManager.Instance.SelectedMovementActionType, shapeType);
            instance.ResetMovementActions();
            selectionWheel.DeactivateSelectionWheel();
        }
    }
}
