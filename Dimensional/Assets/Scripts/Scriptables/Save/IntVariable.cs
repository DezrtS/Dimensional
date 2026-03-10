using UnityEngine;

namespace Scriptables.Save
{
    [CreateAssetMenu(fileName = "IntVariable", menuName = "Scriptable Objects/Save/IntVariable")]
    public class IntVariable : SaveVariable
    {
        [Space]
        [SerializeField] private int defaultValue;
        private int _runtimeValue;

        public int Value
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
            return _runtimeValue.ToString();
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = int.Parse(data);
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = defaultValue;
        }
    }
}
