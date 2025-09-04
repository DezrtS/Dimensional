using Scriptables.Shapes;
using Systems.Visual_Effects;
using UnityEngine;

namespace Scriptables.Visual_Effects
{
    [CreateAssetMenu(fileName = "ShapeActionVisualEffectDatum", menuName = "Scriptable Objects/Visual Effects/Actions/ShapeActionVisualEffectDatum")]
    public class ShapeActionVisualEffectDatum : ActionVisualEffectDatum
    {
        [SerializeField] private ShapeType shapeType;
        
        public ShapeType ShapeType => shapeType;

        public override ActionVisualEffect AttachActionVisualEffect(Transform parent)
        {
            var actionVisualEffectObject = Instantiate(ActionVisualEffectPrefab, parent);
            var actionVisualEffect = actionVisualEffectObject.GetComponent<ShapeActionVisualEffect>();
            actionVisualEffect.Initialize(this);
            return actionVisualEffect;
        }
    }
}
