using System;

namespace Interfaces
{
    public interface IObjectPoolable<out T>
    {
        public event Action<T> Returned;

        public void ReturnToPool();
    }
}
