namespace SOPractice
{
    /// <summary>Демка для HUD перемикача: назва, підказка, стан.</summary>
    public interface IDemo
    {
        string Title { get; }
        string Hint { get; }
        string Status { get; }
    }

    /// <summary>Скрипт, що віддає рядок стану для HUD.</summary>
    public interface IStatusSource
    {
        string Status { get; }
    }
}
