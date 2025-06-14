namespace Interfaces
{
    public interface ISingleton<T>
    {
        public static T Instance { get; } = default(T);
        public void InitializeSingleton();
    }
}