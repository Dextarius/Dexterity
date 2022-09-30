using Core.Tools;

namespace Core.States
{
    public interface IPausable
    {
        PauseToken Pause();
        void       Unpause();
    }
}