public interface IInitializeBehavior<in T>
{
    void Initialize(T data);
}