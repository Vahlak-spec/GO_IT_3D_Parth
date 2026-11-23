namespace RaycastAdvanced
{
    /// <summary>
    /// Спільний інтерфейс демок уроку «Raycast — розширений рівень»,
    /// щоб DemoSwitcher міг показати назву, підказку і поточний стан у HUD.
    /// </summary>
    public interface IRaycastDemo
    {
        string Title { get; }
        string Hint { get; }
        string Status { get; }
    }
}
