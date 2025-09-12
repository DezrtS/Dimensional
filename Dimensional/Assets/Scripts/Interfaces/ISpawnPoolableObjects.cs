namespace Interfaces
{
    public interface ISpawnPoolableObjects<out T>
    {
        public T Spawn();
    }
}
