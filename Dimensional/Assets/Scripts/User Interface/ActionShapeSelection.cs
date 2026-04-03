using System;
using System.Collections.Generic;
using Managers;
using Scriptables.Save;
using Scriptables.Shapes;
using Scriptables.User_Interface;
using Systems.Actions.Movement;
using Systems.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace User_Interface
{
    public class ActionShapeSelection : MonoBehaviour
    {
        private static readonly int IsRevealedHash = Animator.StringToHash("IsRevealed");
        private const float DpadThreshold = 0.5f;
        
        [Header("Save Data")]
        [SerializeField] private PlayerShapes playerShapesSaveData;
        [SerializeField] private IntListVariable unlockedActionsSaveData;
        [SerializeField] private IntListVariable unlockedShapesSaveData;
        [SerializeField] private NewShapes newShapesSaveData;
        [Space]
        [SerializeField] private float revealDuration;
        [Space]
        [SerializeField] private MovementActionShapesDatum[] movementActionShapesData;
        [Space]
        [SerializeField] private TextMeshProUGUI selectedAction;
        [SerializeField] private ActionOption leftActionOption;
        [SerializeField] private ActionOption rightActionOption;
        [Space]
        [SerializeField] private Image leftDPadImage;
        [SerializeField] private Image rightDPadImage;
        [SerializeField] private Image upDownDPadImage;
        [Space]
        [SerializeField] private ShapeOption[] upperShapes;
        [SerializeField] private ShapeOption selectedShape;
        [SerializeField] private ShapeOption[] lowerShapes;
        [Space]
        [SerializeField] private GameObject newShapesNotifierGameObject;
        [Space]
        [SerializeField] private Sprite defaultIconSprite;
        [SerializeField] private Color newShapeBackgroundColor;
        [SerializeField] private Color newShapeTextColor;
        [SerializeField] private Color newShapeIconColor;
        
        private InputActionMap _inputActionMap;
        private Animator _animator;

        private List<MovementActionShapesDatum> _unlockedActions;
        private Dictionary<MovementActionType, List<ShapeDatum>> _actionShapes;
        private Dictionary<MovementActionType, ShapeType> _selectedShapes;
        
        private int _currentActionIndex;

        private bool _canSwitch = true;
        private bool _isRevealed;
        private float _revealTimer;
        
        private void OnEnable()
        {
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            unlockedActionsSaveData.ValueChanged += UnlockedActionsSaveDataOnValueChanged;
            unlockedShapesSaveData.ValueChanged += UnlockedShapesSaveDataOnValueChanged;
            newShapesSaveData.ValueChanged += NewShapesSaveDataOnValueChanged;
        }

        private void OnDisable()
        {
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
            unlockedActionsSaveData.ValueChanged -= UnlockedActionsSaveDataOnValueChanged;
            unlockedShapesSaveData.ValueChanged -= UnlockedShapesSaveDataOnValueChanged;
            newShapesSaveData.ValueChanged -= NewShapesSaveDataOnValueChanged;
            UnassignControls();
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            if (newValue != GameState.Preparing) return;
            foreach (var movementActionShape in playerShapesSaveData.Value.movementActionShapes)
            {
                _selectedShapes.Add(movementActionShape.MovementActionType, movementActionShape.ShapeType);
            }

            DefineUnlockedActions();
            DefineUnlockedShapes();
            CheckNewShapes();
            UpdateUI();
        }
        
        private void UnlockedActionsSaveDataOnValueChanged()
        {
            DefineUnlockedActions();
            CheckNewShapes();
            UpdateUI();
        }

        private void UnlockedShapesSaveDataOnValueChanged()
        {
            DefineUnlockedShapes();
            CheckNewShapes();
            UpdateUI();
        }
        
        private void NewShapesSaveDataOnValueChanged()
        {
            CheckNewShapes();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _selectedShapes = new Dictionary<MovementActionType, ShapeType>();
        }

        private void Start()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Action Selection");
            AssignControls();
        }

        private void FixedUpdate()
        {
            if (_revealTimer <= 0f) return;

            _revealTimer -= Time.fixedDeltaTime;
            if (_revealTimer <= 0f)
            {
                SetIsRevealed(false);
            }
        }

        private void DefineUnlockedActions()
        {
            _unlockedActions = new List<MovementActionShapesDatum>();
            foreach (var movementActionShapeDatum in movementActionShapesData)
            {
                if (unlockedActionsSaveData.Value.list.Contains((int)movementActionShapeDatum.MovementActionType)) _unlockedActions.Add(movementActionShapeDatum);
            }
            
            _currentActionIndex = 0;
        }

        private void DefineUnlockedShapes()
        {
            _actionShapes = new Dictionary<MovementActionType, List<ShapeDatum>>();
            foreach (var movementActionShapeDatum in movementActionShapesData)
            {
                _actionShapes.Add(movementActionShapeDatum.MovementActionType, new List<ShapeDatum>());
                foreach (var shapeDatum in movementActionShapeDatum.ShapeData)
                {
                    if (!unlockedShapesSaveData.Value.list.Contains((int)shapeDatum.ShapeType)) continue;
                    _actionShapes[movementActionShapeDatum.MovementActionType].Add(shapeDatum);
                }
            }
        }
        
        private void CheckNewShapes()
        {
            var hasNewShapes = false;
            foreach (var movementActionShapesDatum in _unlockedActions)
            {
                hasNewShapes = newShapesSaveData.GetNewActionShapes(movementActionShapesDatum.MovementActionType).Count > 0;
                if (hasNewShapes) break;
            }
            
            if (hasNewShapes) _animator.SetTrigger("New Shapes");
            newShapesNotifierGameObject.SetActive(hasNewShapes);
            leftDPadImage.color = hasNewShapes ? newShapeIconColor : Color.white;
            rightDPadImage.color = hasNewShapes ? newShapeIconColor : Color.white;
        }

        public void EnableCanSwitch() => _canSwitch = true;
        
        public void DisableCanSwitch() => _canSwitch = false;

        public void UpdateUI()
        {
            if (_unlockedActions.Count <= 0 || _actionShapes.Count <= 0) return;
            
            var datum = _unlockedActions[_currentActionIndex];
            var actionCount = _unlockedActions.Count;
            var rightActionIndex = (_currentActionIndex + 1 + actionCount) % actionCount;
            var leftActionIndex = (_currentActionIndex - 1 + actionCount) % actionCount;
            
            selectedAction.text = datum.MovementActionName;
            if (rightActionIndex == _currentActionIndex)
            {
                rightActionOption.Initialize(defaultIconSprite);
            }
            else
            {
                rightActionOption.Initialize(_unlockedActions[rightActionIndex]);
                if (newShapesSaveData.GetNewActionShapes(_unlockedActions[rightActionIndex].MovementActionType).Count > 0) rightActionOption.SetIsNew(newShapeBackgroundColor, newShapeTextColor);   
            }

            if (leftActionIndex == _currentActionIndex)
            {
                leftActionOption.Initialize(defaultIconSprite);
            }
            else
            {
                leftActionOption.Initialize(_unlockedActions[leftActionIndex]);
                if (newShapesSaveData.GetNewActionShapes(_unlockedActions[leftActionIndex].MovementActionType).Count > 0) leftActionOption.SetIsNew(newShapeBackgroundColor, newShapeTextColor);
            }
            
            var shapeData = _actionShapes[datum.MovementActionType];
            var shapeCount = shapeData.Count;
            switch (shapeCount)
            {
                case 0:
                    SetAllUnknown();
                    return;
            }

            var startIndex = FindCurrentShapeIndex();
            if (startIndex == -1) startIndex = 0;
            var index = startIndex;
            var selectedShapeDatum = shapeData[index];
            selectedShape.Initialize(selectedShapeDatum);
            var newActionShapes = newShapesSaveData.GetNewActionShapes(datum.MovementActionType);
            if (newActionShapes.Contains(selectedShapeDatum.ShapeType))
            {
                newShapesSaveData.RemoveNewShape(datum.MovementActionType, selectedShapeDatum.ShapeType);
            }
            
            upDownDPadImage.color = newActionShapes.Count <= 0 ? Color.white : newShapeIconColor;
            foreach (var shape in lowerShapes)
            {
                index = (index + 1 + shapeCount) % shapeCount;
                if (shapeCount == 1)
                {
                    shape.Initialize();
                    continue;
                }
                
                var shapeDatum = shapeData[index];
                shape.Initialize(shapeDatum);
                if (newShapesSaveData.GetNewActionShapes(datum.MovementActionType).Contains(shapeDatum.ShapeType)) shape.SetIsNew(newShapeBackgroundColor, newShapeTextColor, newShapeIconColor);
            }
            
            index = startIndex;
            foreach (var shape in upperShapes)
            {
                index = (index - 1 + shapeCount) % shapeCount;
                if (shapeCount == 1)
                {
                    shape.Initialize();
                    continue;
                }
                
                var shapeDatum = shapeData[index];
                shape.Initialize(shapeDatum);
                if (newShapesSaveData.GetNewActionShapes(datum.MovementActionType).Contains(shapeDatum.ShapeType)) shape.SetIsNew(newShapeBackgroundColor, newShapeTextColor, newShapeIconColor);
            }
        }

        private void SetIsRevealed(bool isRevealed)
        {
            _isRevealed = isRevealed;
            if (_isRevealed) _revealTimer = revealDuration;
            _animator.SetBool(IsRevealedHash, _isRevealed);
        }

        private void SetAllUnknown()
        {
            selectedShape.Initialize();
            
            foreach (var shape in lowerShapes)
            {
                shape.Initialize();
            }

            foreach (var shape in upperShapes)
            {
                shape.Initialize();
            }
        }

        private void SwitchAction(int direction)
        {
            if (!_canSwitch) return;
            
            if (_unlockedActions.Count <= 1) return;
            var actionCount = _unlockedActions.Count;
            _currentActionIndex = (_currentActionIndex + direction + actionCount) % actionCount;
            _animator.SetTrigger(direction > 0 ? "Right" : "Left");
        }

        private void SwitchShape(int direction)
        {
            if (!_canSwitch) return;
            var movementActionType = _unlockedActions[_currentActionIndex].MovementActionType;
            var shapes = _actionShapes[movementActionType];

            if (shapes.Count <= 1) return;
            var index = FindCurrentShapeIndex();
            if (index == -1) index = 0;
            index = (index + direction + shapes.Count) % shapes.Count;
            _selectedShapes[movementActionType] = shapes[index].ShapeType;
            _animator.SetTrigger(direction > 0 ? "Up" : "Down");
            
            playerShapesSaveData.SetMovementActionShapesPreset(movementActionType, shapes[index].ShapeType);
            PlayerController.Instance.ResetMovementActions();
        }

        private int FindCurrentShapeIndex()
        {
            var movementActionType = _unlockedActions[_currentActionIndex].MovementActionType;
            var selectedShapeType = _selectedShapes[movementActionType];
            var shapes = _actionShapes[movementActionType];
            
            return shapes.FindIndex(shape => shape.ShapeType == selectedShapeType);
        }
        
        private void OnDpadInput(InputAction.CallbackContext context)
        {
            if (_unlockedActions.Count <= 0) return;
            
            if (!_isRevealed)
            {
                SetIsRevealed(true);
                return;
            }

            _revealTimer = revealDuration;

            var input = context.ReadValue<Vector2>();

            // Horizontal = Action
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > DpadThreshold)
                    SwitchAction(1);
                else if (input.x < -DpadThreshold)
                    SwitchAction(-1);

                return;
            }

            // Vertical = Shape
            if (input.y > DpadThreshold)
                SwitchShape(-1); // UP
            else if (input.y < -DpadThreshold)
                SwitchShape(1); // DOWN
        }
        
        private void AssignControls()
        {
            var dpadAction = _inputActionMap.FindAction("Select Shape");
            dpadAction.performed += OnDpadInput;
        }

        private void UnassignControls()
        {
            var dpadAction = _inputActionMap.FindAction("Select Shape");
            dpadAction.performed -= OnDpadInput;
        }
    }
}
