// Marker interface used for strong typing
public interface IBtNodeKey<TFactory> where TFactory : IBtNodeFactory
{
    // This interface lets the compiler verify each key is tied to a factory
}