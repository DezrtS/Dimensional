using UnityEngine;

namespace Scriptables.Save
{
    [CreateAssetMenu(fileName = "StringVariable", menuName = "Scriptable Objects/Save/StringVariable")]
    public class StringVariable : SaveVariable
    {
        [Space]
        [SerializeField] private string defaultValue;
        private string _runtimeValue;

        public string Value
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
            return _runtimeValue;
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = data;
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = defaultValue;
        }
    }
}
