using Managers;
using UnityEngine;

namespace Scriptables.Save
{
    [CreateAssetMenu(fileName = "BoolVariable", menuName = "Scriptable Objects/Save/BoolVariable")]
    public class BoolVariable : SaveVariable
    {
        [Space]
        [SerializeField] private bool defaultValue;
        private bool _runtimeValue;

        public bool Value
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
            _runtimeValue = bool.Parse(data);
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = defaultValue;
        }
    }
}
