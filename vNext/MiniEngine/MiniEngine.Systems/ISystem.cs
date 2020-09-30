namespace MiniEngine.Systems
{
    /// <summary>
    /// Marker interface, every class implementing ISystem should have a method called Process with (n + m) arguments
    /// the first n arguments should be of type AComponent, the last m arguments should be injectable services
    /// </summary>
    public interface ISystem { }
}
