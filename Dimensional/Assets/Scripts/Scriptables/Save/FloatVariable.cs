using System.Globalization;
using Managers;
using UnityEngine;

namespace Scriptables.Save
{
    [CreateAssetMenu(fileName = "FloatVariable", menuName = "Scriptable Objects/Save/FloatVariable")]
    public class FloatVariable : SaveVariable
    {
        [Space]
        [SerializeField] private float defaultValue;
        private float _runtimeValue;

        public float Value
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
            return _runtimeValue.ToString(CultureInfo.InvariantCulture);
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = float.Parse(data, CultureInfo.InvariantCulture);
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = defaultValue;
        }
    }
}
