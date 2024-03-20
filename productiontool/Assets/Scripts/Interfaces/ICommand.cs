public interface ICommand
{
    public void Execute();
    public virtual void Undo() { }
}