using System;
using System.Linq;
using Scriptables.Shapes;
using Systems.Actions.Movement;
using Systems.Player;
using TMPro;
using UnityEngine;

namespace Debugging
{
    public class ActionShapeSetter : MonoBehaviour
    {
        [SerializeField] private GameObject player;

        [Header("Action Dropdowns")]
        [SerializeField] private TMP_Dropdown jumpDropdown;
        [SerializeField] private TMP_Dropdown doubleJumpDropdown;
        [SerializeField] private TMP_Dropdown wallJumpDropdown;
        [SerializeField] private TMP_Dropdown dashDropdown;
        [SerializeField] private TMP_Dropdown diveDropdown;
        [SerializeField] private TMP_Dropdown airDropdown;
        [SerializeField] private TMP_Dropdown rollDropdown;
        [SerializeField] private TMP_Dropdown leftSpecialDropdown;
        [SerializeField] private TMP_Dropdown rightSpecialDropdown;
        [SerializeField] private TMP_Dropdown wallSlideDropdown;

        private PlayerController _playerController;
        private string[] _shapeNames;

        private void Awake()
        {
            _playerController = player.GetComponent<PlayerController>();

            // Populate dropdowns with ShapeType enum names (skip None)
            _shapeNames = Enum.GetNames(typeof(ShapeType)).Skip(1).ToArray();

            SetupDropdown(jumpDropdown, MovementActionType.JumpAction);
            SetupDropdown(doubleJumpDropdown, MovementActionType.DoubleJumpAction);
            SetupDropdown(wallJumpDropdown, MovementActionType.WallJumpAction);
            SetupDropdown(dashDropdown, MovementActionType.DashAction);
            SetupDropdown(diveDropdown, MovementActionType.DiveAction);
            SetupDropdown(airDropdown, MovementActionType.AirAction);
            //SetupDropdown(rollDropdown, MovementActionType.RollAction);
            //SetupDropdown(leftSpecialDropdown, MovementActionType.LeftSpecialAction);
            //SetupDropdown(rightSpecialDropdown, MovementActionType.RightSpecialAction);
            //SetupDropdown(wallSlideDropdown, MovementActionType.WallSlideAction);
        }

        private void SetupDropdown(TMP_Dropdown dropdown, MovementActionType actionType)
        {
            dropdown.ClearOptions();
            dropdown.AddOptions(_shapeNames.ToList());

            // Sync UI with the player's current action shape
            var currentShape = _playerController
                .MovementActionShapes
                .FirstOrDefault(m => m.MovementActionType == actionType)
                .ShapeType;

            // Dropdown value = shapeType - 1 (since we skipped None)
            dropdown.value = Mathf.Max(0, (int)currentShape - 1);

            // Add listener
            dropdown.onValueChanged.AddListener(value => DropdownOnValueChanged(actionType, value));
        }

        private void DropdownOnValueChanged(MovementActionType movementActionType, int value)
        {
            // offset by +1 since "None" is skipped
            var shapeType = (ShapeType)(value + 1);
            _playerController.SetMovementActionShape(movementActionType, shapeType);
        
            _playerController.ResetMovementActions();
        }
    }
}
