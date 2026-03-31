using Scriptables.User_Interface;
using UnityEngine;
using UnityEngine.UI;

namespace User_Interface
{
    public class ActionOption : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private Image actionIcon;
        
        private Color _defaultBackgroundColor;
        private Color _defaultIconColor;

        private void Awake()
        {
            _defaultBackgroundColor = background.color;
            _defaultIconColor = actionIcon.color;
        }

        public void Initialize(MovementActionShapesDatum movementActionShapesDatum)
        {
            actionIcon.sprite = movementActionShapesDatum.MovementActionIcon;
            ResetColors();
        }

        public void Initialize(Sprite defaultIcon)
        {
            ResetColors();
            actionIcon.sprite = defaultIcon;
        }

        private void ResetColors()
        {
            background.color = _defaultBackgroundColor;
            actionIcon.color = _defaultIconColor;
        }

        public void SetIsNew(Color newActionBackgroundColor, Color newActionIconColor)
        {
            newActionBackgroundColor.a = _defaultBackgroundColor.a;
            newActionIconColor.a = _defaultIconColor.a;
            
            background.color = newActionBackgroundColor;
            actionIcon.color = newActionIconColor;
        }
    }
}
