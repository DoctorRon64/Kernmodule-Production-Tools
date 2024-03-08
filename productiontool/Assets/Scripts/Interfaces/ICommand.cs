public interface ICommand
{
    public abstract void Execute();
    public abstract void Undo();
    public abstract void Redo();
}