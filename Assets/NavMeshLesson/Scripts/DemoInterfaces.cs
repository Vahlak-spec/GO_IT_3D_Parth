namespace NavMeshLesson
{
    /// <summary>Демка для HUD перемикача: назва, підказка, стан.</summary>
    public interface IDemo
    {
        string Title { get; }
        string Hint { get; }
        string Status { get; }
    }

    /// <summary>Скрипт, який уміє почати свою поведінку заново (при вході в демку або по [R]).</summary>
    public interface IRestartable
    {
        void Restart();
    }

    /// <summary>Скрипт, що віддає рядок стану для HUD.</summary>
    public interface IStatusSource
    {
        string Status { get; }
    }
}
