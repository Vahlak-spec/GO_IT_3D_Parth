namespace RaycastLesson
{
    /// <summary>
    /// Спільний інтерфейс для демо-скриптів уроку — щоб RaycastDemoSwitcher
    /// міг показати назву, підказку і поточний стан у HUD.
    /// </summary>
    public interface IRaycastDemo
    {
        string Title { get; }
        string Hint { get; }
        string Status { get; }
    }
}
