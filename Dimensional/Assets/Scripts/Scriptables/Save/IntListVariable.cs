using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scriptables.Save
{
    [Serializable]
    public class IntList
    {
        public List<int> list = new();
    }
    
    [CreateAssetMenu(fileName = "IntListVariable", menuName = "Scriptable Objects/Save/IntListVariable")]
    public class IntListVariable : SaveVariable
    {
        [Space]
        [SerializeField] private IntList defaultValue = new();
        private IntList _runtimeValue;
        
        public IntList Value {
            get
            {
                EnsureRuntime();
                return _runtimeValue;
            }
        }

        public void SetValue(int index, int value)
        {
            EnsureRuntime();
            _runtimeValue.list[index] = value;
            SetIsDirty();
        }

        public void AddValue(int value)
        {
            EnsureRuntime();
            _runtimeValue.list.Add(value);
            SetIsDirty();
        }

        public void RemoveValue(int value)
        {
            EnsureRuntime();
            _runtimeValue.list.Remove(value);
            SetIsDirty();
        }
        
        private void EnsureRuntime()
        {
            _runtimeValue ??= new IntList();
            _runtimeValue.list ??= new List<int>();
        }

        public override string Capture()
        {
            base.Capture();
            return JsonUtility.ToJson(_runtimeValue);
        }

        public override void Restore(string data)
        {
            base.Restore(data);
            _runtimeValue = JsonUtility.FromJson<IntList>(data);
        }

        public override void Reset()
        {
            base.Reset();
            _runtimeValue = new IntList() { list = new List<int>(defaultValue.list) };
        }
    }
}
