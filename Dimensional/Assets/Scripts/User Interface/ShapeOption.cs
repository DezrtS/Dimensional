using System;
using System.Linq;
using Managers;
using Scriptables.Save;
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
        [SerializeField] private Image background;
        [SerializeField] private TextMeshProUGUI shapeText;
        [SerializeField] private Image shapeIcon;
        
        private Color _defaultBackgroundColor;
        private Color _defaultTextColor;
        private Color _defaultIconColor;

        private void Awake()
        {
            _defaultBackgroundColor = background.color;
            _defaultTextColor = shapeText.color;
            _defaultIconColor = shapeIcon.color;
        }

        public void Initialize(ShapeDatum shapeData)
        {
            shapeText.text = shapeData.ShapeName;
            shapeIcon.sprite = shapeData.ShapeIcon;
            shapeIcon.enabled = true;
            ResetColors();
        }

        public void Initialize()
        {
            shapeText.text = "???";
            shapeIcon.enabled = false;
            ResetColors();
        }

        private void ResetColors()
        {
            background.color = _defaultBackgroundColor;
            shapeText.color = _defaultTextColor;
            shapeIcon.color = _defaultIconColor;
        }

        public void SetIsNew(Color newShapeBackgroundColor, Color newShapeTextColor, Color newShapeIconColor)
        {
            newShapeBackgroundColor.a = _defaultBackgroundColor.a;
            newShapeTextColor.a = _defaultTextColor.a;
            newShapeIconColor.a = _defaultIconColor.a;
            
            background.color = newShapeBackgroundColor;
            shapeText.color = newShapeTextColor;
            shapeIcon.color = newShapeIconColor;
        }
    }
}
