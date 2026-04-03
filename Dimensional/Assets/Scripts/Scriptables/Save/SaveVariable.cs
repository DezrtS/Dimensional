using System;
using Managers;
using UnityEngine;

namespace Scriptables.Save
{
    public abstract class SaveVariable : ScriptableObject
    {
        public event Action ValueChanged;
        
        [SerializeField] private string id;
        [Space]
        [SerializeField] private bool save = true;
        [SerializeField] private bool load = true;
        
        public string Id => id;
        public bool Save => save;
        public bool Load => load;

        private bool _isDirty;

        public virtual string Capture()
        {
            _isDirty = false;
            return null;
        }
        public virtual void Restore(string data) => _isDirty = false;
        public virtual void Reset() => _isDirty = false;

        protected void SetIsDirty()
        {
            ValueChanged?.Invoke();
            if (_isDirty) return;
            _isDirty = true;
            SaveManager.Instance.SetDirty(this);
        }
    }
}
