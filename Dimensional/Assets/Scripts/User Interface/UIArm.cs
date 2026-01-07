using UnityEngine;
using UnityEngine.UI;

namespace User_Interface
{
    public class UIArm : MonoBehaviour
    {
        [SerializeField] private Image armImage;

        public void UpdateArm(Vector3 elementPosition, Vector3 targetPosition)
        {
            Vector2 direction = targetPosition - elementPosition;
            var distance = direction.magnitude;
            direction = direction.normalized;

            var size = distance / 150f;
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            
            armImage.transform.localScale = new Vector3(1, size, 1);
            armImage.transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
