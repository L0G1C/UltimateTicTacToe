namespace UltimateTicTacToe.Web.Logic
{
    public interface IGameManager
    {
        GameState GetGameState(string contextConnectionId);
    }
}