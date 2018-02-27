using UltimateTicTacToe.Web.Models;

namespace UltimateTicTacToe.Web.Logic
{
    public interface IGameManager
    {
        GameState GetGameState(string connectionId);
        string CreateGame(string name, string connectionId);
        Game GetGame(string gameCode);
    }
}