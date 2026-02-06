using UnityEngine;

namespace Systems.Grass
{
    public class GrassMask : MonoBehaviour
    {
        [SerializeField] private int priority;
        [SerializeField] private Texture2D maskTexture;
        
        public int Priority => priority;
        public Texture2D MaskTexture => maskTexture;
    }
}
