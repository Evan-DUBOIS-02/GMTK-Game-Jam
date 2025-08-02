namespace Puzzle
{
    public interface Interactable
    {
        public int Interact(Player.PlayerState sate); //1 = Lever / 2 = Bomb
        public void SetToDefaultState();
    }
}