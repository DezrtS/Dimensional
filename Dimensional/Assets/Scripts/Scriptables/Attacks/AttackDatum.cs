using UnityEngine;

namespace Scriptables.Attacks
{
    public enum AttackType
    {
        None,
        Light,
        Medium,
        Heavy,
    }
    
    [CreateAssetMenu(fileName = "AttackDatum", menuName = "Scriptable Objects/Attacks/AttackDatum")]
    public class AttackDatum : ScriptableObject
    {
        [SerializeField] private float damage;
    }
}
