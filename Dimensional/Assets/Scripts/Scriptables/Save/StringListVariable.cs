using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptables.Save
{
    [Serializable]
    public class StringList
    {
        public List<string> list;
    }
    
    [CreateAssetMenu(fileName = "StringListVariable", menuName = "Scriptable Objects/Save/StringListVariable")]
    public class StringListVariable : SaveVariable
    {
        [Space]
        [SerializeField] private StringList defaultValue = new();
        private StringList _runtimeValue;
        
        public StringList Value => _runtimeValue;

        public void SetValue(int index, string value)
        {
            _runtimeValue.list[index] = value;
            SetIsDirty();
        }

        public void AddValue(string value)
        {
            _runtimeValue.list.Add(value);
            SetIsDirty();
        }

        public override string Capture()
        {
            base.Capture();
            return JsonUtility.ToJson(_runtimeValue);
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = JsonUtility.FromJson<StringList>(data);
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = new StringList() { list = new List<string>(defaultValue.list) };
        }
    }
}
