using System;
using System.Collections.Generic;
using Managers;
using Scriptables.User_Interface;
using Systems.Player;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace User_Interface
{
    public class ActionSelection : MonoBehaviour
    {
        [SerializeField] private MovementActionShapesDatum[] movementActionShapesData;
        [SerializeField] private ShapeOption[] shapeOptions;
        [SerializeField] private TextMeshProUGUI actionText;
        [Space]
        [SerializeField] private float showDuration = 2f;

        private InputActionMap _inputActionMap;
        private Animator _animator;

        private int _currentActionIndex;
        private ShapeOption _currentShapeOption;

        // Per-action selected shape index
        private Dictionary<int, int> _selectedShapeIndexDictionary;

        private float _showTimer;
        private bool _isHidden = true;

        private const float DpadThreshold = 0.5f;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            GameManager.GameStateChanged += GameManagerOnGameStateChanged;
            SaveManager.Loaded += SaveManagerOnLoaded;
            SaveManager.Saving += SaveManagerOnSaving;
        }

        private void SaveManagerOnLoaded(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Action)) return;
            foreach (var actionShape in saveData.actionData.actionShapes)
            {
                var data = actionShape.Split(":");
                var action = int.Parse(data[0]);
                var shape = int.Parse(data[1]);
                _selectedShapeIndexDictionary[action] = shape;
            }
        }

        private void SaveManagerOnSaving(SaveData saveData, List<DataType> dataTypes)
        {
            if (!dataTypes.Contains(DataType.Action)) return;
            for (var i = 0; i < movementActionShapesData.Length; i++)
            {
                saveData.actionData.actionShapes.Add($"{i}:{_selectedShapeIndexDictionary[i]}");
            }
        }

        private void GameManagerOnGameStateChanged(GameState oldValue, GameState newValue)
        {
            switch (newValue)
            {
                case GameState.Initializing:
                    _selectedShapeIndexDictionary = new Dictionary<int, int>();
                    for (var i = 0; i < movementActionShapesData.Length; i++)
                    {
                        _selectedShapeIndexDictionary[i] = 0;
                    }

                    break;
                case GameState.Preparing:
                    SaveManager.Instance.RequestLoad(new List<DataType>() { DataType.Action });

                    for (var i = 0; i < movementActionShapesData.Length; i++)
                    {
                        var actionType = movementActionShapesData[i].MovementActionType;
                        var shapeType = movementActionShapesData[i].ShapeData[_selectedShapeIndexDictionary[i]].ShapeType;
                        PlayerController.Instance.SetMovementActionShape(actionType, shapeType);
                    }
                    PlayerController.Instance.ResetMovementActions();
            
                    var datum = movementActionShapesData[_currentActionIndex];
                    for (var i = 0; i < shapeOptions.Length; i++)
                    {
                        var option = shapeOptions[i];

                        if (i >= datum.ShapeData.Length)
                        {
                            option.Initialize();
                        }
                        else
                        {
                            option.Initialize(datum.ShapeData[i]);
                        }
                    }
            
                    UIManager.Instance.SelectedMovementActionType = datum.MovementActionType;
                    break;
            }
        }

        private void Start()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Action Selection");
            AssignControls();
        }

        private void OnDisable()
        {
            UnassignControls();
            GameManager.GameStateChanged -= GameManagerOnGameStateChanged;
        }

        private void FixedUpdate()
        {
            if (_showTimer <= 0f) return;

            _showTimer -= Time.fixedDeltaTime;
            if (_showTimer <= 0f)
            {
                HideOptions();
            }
        }

        #region Input

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

        private void OnDpadInput(InputAction.CallbackContext context)
        {
            if (_isHidden)
            {
                ShowOptions();
                return;
            }

            _showTimer = showDuration;

            var input = context.ReadValue<Vector2>();

            // Horizontal = Action
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > DpadThreshold)
                    SwitchActionRelative(+1);
                else if (input.x < -DpadThreshold)
                    SwitchActionRelative(-1);

                return;
            }

            // Vertical = Shape
            if (input.y > DpadThreshold)
                SwitchShapeRelative(-1); // UP
            else if (input.y < -DpadThreshold)
                SwitchShapeRelative(+1); // DOWN
        }

        #endregion

        #region Actions

        private void SwitchActionRelative(int direction)
        {
            int newIndex =
                (movementActionShapesData.Length + _currentActionIndex + direction)
                % movementActionShapesData.Length;

            SwitchAction(newIndex);
        }

        private void SwitchAction(int newActionIndex)
        {
            _currentActionIndex = newActionIndex;

            var datum = movementActionShapesData[_currentActionIndex];
            actionText.text = datum.MovementActionName;

            UIManager.Instance.SelectedMovementActionType = datum.MovementActionType;

            SetShapeOptions(datum);

            SelectShape(_selectedShapeIndexDictionary[_currentActionIndex]);
        }

        #endregion

        #region Shapes

        private void SwitchShapeRelative(int direction)
        {
            int max = movementActionShapesData[_currentActionIndex].ShapeData.Length;
            int current = _selectedShapeIndexDictionary[_currentActionIndex];

            int newIndex = Mathf.Clamp(current + direction, 0, max - 1);
            SelectShape(newIndex);
        }

        private void SelectShape(int index)
        {
            if (index < 0 || index >= shapeOptions.Length)
                return;

            var previous = _currentShapeOption;
            _currentShapeOption = shapeOptions[index];

            _selectedShapeIndexDictionary[_currentActionIndex] = index;

            if (previous == _currentShapeOption)
                return;

            if (previous)
                previous.Deselect();

            _currentShapeOption.Select();
        }

        private void SetShapeOptions(MovementActionShapesDatum datum)
        {
            for (int i = 0; i < shapeOptions.Length; i++)
            {
                var option = shapeOptions[i];

                if (i >= datum.ShapeData.Length)
                {
                    option.Initialize();
                    option.Disable();
                }
                else
                {
                    option.Initialize(datum.ShapeData[i]);
                    option.Enable();
                }
            }
        }

        #endregion

        #region UI Visibility

        private void ShowOptions()
        {
            _isHidden = false;
            _showTimer = showDuration;
            _animator.SetTrigger("Show");

            foreach (var option in shapeOptions)
                option.Show();
        }

        private void HideOptions()
        {
            _isHidden = true;
            _animator.SetTrigger("Hide");

            foreach (var option in shapeOptions)
                option.Hide();
        }

        #endregion
    }
}
