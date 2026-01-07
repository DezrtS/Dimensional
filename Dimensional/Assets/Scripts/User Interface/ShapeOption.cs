using System;
using Managers;
using Scriptables.Shapes;
using Scriptables.User_Interface;
using Systems.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface
{
    public class ShapeOption : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI shapeText;
        [SerializeField] private Image shapeImage;
        
        private ShapeType _shapeType;
        private bool _isSelected;
        private bool _canSelect;
        private bool _isDisabled;
        private Animator _animator;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void Initialize(ShapeDatum shapeData)
        {
            _canSelect = true;
            _shapeType = shapeData.ShapeType;
            shapeText.text = shapeData.ShapeName;
            shapeImage.sprite = shapeData.ShapeIcon;
        }
        
        public void Initialize()
        {
            _canSelect = false;
            _shapeType = ShapeType.None;
            shapeText.text = "???";
            shapeImage.sprite = null;
            _isDisabled = true;
        }

        private void Default() => _animator.SetTrigger("Default");

        public void Select()
        {
            if (_isSelected || !_canSelect || _isDisabled) return;
            _isSelected = true;
            _animator.SetTrigger("Select");
            PlayerController.Instance.SetMovementActionShape(UIManager.Instance.SelectedMovementActionType, _shapeType);
            PlayerController.Instance.ResetMovementActions();
        }

        public void Deselect()
        {
            if (!_isSelected) return;
            _isSelected = false;
            if (!_isDisabled) Default();
        }

        public void Enable()
        {
            if (!_isDisabled) return;
            _isDisabled = false;
            if (!_isSelected) Default();
        }

        public void Disable()
        {
            if (_isDisabled) return;
            _isDisabled = true;
            _animator.SetTrigger("Disable");
        }

        public void Hide() => _animator.SetTrigger("Hide");

        public void Show()
        {
            if (_isSelected) _animator.SetTrigger("Select");
            else if (_isDisabled) _animator.SetTrigger("Disable");
            else Default();
        }
        
    }
}
