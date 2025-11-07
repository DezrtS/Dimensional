using System;
using System.Collections.Generic;
using Managers;
using Scriptables.User_Interface;
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
        [SerializeField] private float showDuration;

        private InputActionMap _inputActionMap;
        private Animator _animator;
        
        private int _currentActionIndex = -1;
        private ShapeOption _currentShapeOption;
        private Dictionary<int, int> _selectedShapeIndexDictionary;

        private float _showTimer;
        private bool _isHidden;

        private void OnDisable()
        {
            UnassignControls();
        }
        
        private void AssignControls()
        {
            var switchActionInputAction = _inputActionMap.FindAction("Switch Action");
            switchActionInputAction.performed += OnSwitchAction;
            
            var selectShapeInputAction = _inputActionMap.FindAction("Select Shape");
            selectShapeInputAction.performed += OnSelectShape;
        }

        private void UnassignControls()
        {
            var switchActionInputAction = _inputActionMap.FindAction("Switch Action");
            switchActionInputAction.performed -= OnSwitchAction;
            
            var selectShapeInputAction = _inputActionMap.FindAction("Select Shape");
            selectShapeInputAction.performed -= OnSelectShape;
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        private void Start()
        {
            _inputActionMap = GameManager.Instance.InputActionAsset.FindActionMap("Action Selection");
            AssignControls();

            _selectedShapeIndexDictionary = new Dictionary<int, int>();
            for (var i = 0; i < movementActionShapesData.Length; i++)
            {
                _selectedShapeIndexDictionary.Add(i, 0);
            }
            SwitchAction(0);
            _showTimer = showDuration;
        }

        private void FixedUpdate()
        {
            var fixedDeltaTime = Time.fixedDeltaTime;
            if (_showTimer <= 0) return;
            _showTimer -= fixedDeltaTime;
            if (_showTimer > 0) return;
            HideOptions();
        }

        private void OnSwitchAction(InputAction.CallbackContext context)
        {
            if (_isHidden)
            {
                ShowOptions();
                return;
            }
            
            _showTimer = showDuration;
            var input = (int)context.ReadValue<float>();
            var newActionIndex = (movementActionShapesData.Length + _currentActionIndex + input) % movementActionShapesData.Length;
            SwitchAction(newActionIndex);
        }
        
        private void OnSelectShape(InputAction.CallbackContext context)
        {
            if (_isHidden)
            {
                ShowOptions();
                return;
            }
            
            _showTimer = showDuration;
            var input = context.ReadValue<Vector2>();
            var indexMap = new Dictionary<Vector2, int>()
            {
                { Vector2.up, 0 },
                { Vector2.right, 1 },
                { Vector2.down, 2 },
                { Vector2.left, 3 }
            };  

            var index = indexMap[input];
            SelectShape(index);
        }

        private void SelectShape(int index)
        {
            var previousShapeOption = _currentShapeOption;
            
            _currentShapeOption = shapeOptions[index];
            _selectedShapeIndexDictionary[_currentActionIndex] = index;
            if (previousShapeOption == _currentShapeOption) return;
            if (previousShapeOption) previousShapeOption.Deselect();
            _currentShapeOption.Select();
        }

        private void SwitchAction(int newActionIndex)
        {
            _currentActionIndex = newActionIndex;
            var movementActionShapesDatum = movementActionShapesData[_currentActionIndex];
            actionText.text = movementActionShapesDatum.MovementActionName;
            
            UIManager.Instance.SelectedMovementActionType = movementActionShapesDatum.MovementActionType;
            for (var i = 0; i < shapeOptions.Length; i++)
            {
                var shapeOption = shapeOptions[i];
                if (i >= movementActionShapesDatum.ShapeData.Length)
                {
                    shapeOption.Initialize();
                    shapeOption.Disable();
                    continue;
                }
                
                shapeOption.Initialize(movementActionShapesDatum.ShapeData[i]);
                shapeOption.Enable();
            }
            
            SelectShape(_selectedShapeIndexDictionary[_currentActionIndex]);
        }

        private void ShowOptions()
        {
            _isHidden = false;
            _showTimer = showDuration;
            _animator.SetTrigger("Show");
            foreach (var shapeOption in shapeOptions)
            {
                shapeOption.Show();
            }
        }

        private void HideOptions()
        {
            _isHidden = true;
            _animator.SetTrigger("Hide");
            foreach (var shapeOption in shapeOptions)
            {
                shapeOption.Hide();
            }
        }
    }
}
