namespace TicTacToe.Domain;

/// <summary>
/// Serviço principal que gerencia o estado e as regras do jogo da velha.
/// </summary>
public class GameService
{
    public Board Board { get; } = new();
    public Player CurrentPlayer { get; private set; } = Player.X; // X starts
    public GameStatus Status { get; private set; } = GameStatus.InProgress;

    public bool MakeMove(Move move, out string? error)
    {
        error = null;
        
        if (Status != GameStatus.InProgress)
        {
            error = "Jogo já finalizado";
            return false;
        }
        if (!move.InBounds(Board.Size))
        {
            error = "Posição inválida";
            return false;
        }
        if (!Board.IsEmpty(move))
        {
            error = "Casa ocupada";
            return false;
        }

        Board.TryPlace(move, CurrentPlayer);

        if (Board.HasWinningLine(CurrentPlayer))
        {
            Status = CurrentPlayer == Player.X ? GameStatus.XWon : GameStatus.OWon;
            return true;
        }

        if (Board.IsFull())
        {
            Status = GameStatus.Draw;
            return true;
        }

        CurrentPlayer = CurrentPlayer == Player.X ? Player.O : Player.X;
        return true;
    }

    public void Reset()
    {
        Board.Clear();
        CurrentPlayer = Player.X;
        Status = GameStatus.InProgress;
    }
}
