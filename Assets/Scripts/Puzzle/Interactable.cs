namespace Puzzle
{
    public interface Interactable
    {
        public bool TryInteract(Player.PlayerState sate);
        public void SetToDefaultState();
    }
}