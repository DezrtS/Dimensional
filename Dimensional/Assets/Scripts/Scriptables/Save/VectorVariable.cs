using UnityEngine;

namespace Scriptables.Save
{
    [CreateAssetMenu(fileName = "VectorVariable", menuName = "Scriptable Objects/Save/VectorVariable")]
    public class VectorVariable : SaveVariable
    {
        [Space]
        [SerializeField] private Vector3 defaultValue;
        private Vector3 _runtimeValue;

        public Vector3 Value
        {
            get => _runtimeValue;
            set
            {
                _runtimeValue = value;
                SetIsDirty();
            }
        }

        public override string Capture()
        {
            base.Capture();
            return JsonUtility.ToJson(_runtimeValue);
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = JsonUtility.FromJson<Vector3>(data);
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = defaultValue;
        }
    }
}