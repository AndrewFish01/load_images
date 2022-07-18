using Cysharp.Threading.Tasks;

namespace Content.Scripts.Commands
{
    public interface ICommand
    {
        UniTask ExecuteAsync();
        void Undo();
    }
}