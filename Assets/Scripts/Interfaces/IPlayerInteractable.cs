namespace NeonBlack.Interfaces
{
    public interface IPlayerInteractable
    {
        bool CanBeInteracted { get; }
        void Activate();
    }
}
