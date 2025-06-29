using System;
using Interfaces;
using Scriptables.Entities;
using Systems.Entities;
using UnityEngine;

namespace Systems.Enemies
{
    public abstract class Enemy : MonoBehaviour, IEntity
    {
        [SerializeField] private EntityDatum entityDatum;
        
        
        private Health _health;
        
        public EntityDatum EntityDatum => entityDatum;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }
    }
}
