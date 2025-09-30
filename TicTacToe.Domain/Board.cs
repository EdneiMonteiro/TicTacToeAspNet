namespace TicTacToe.Domain;

/// <summary>
/// Representa o tabuleiro 3x3 do jogo da velha, gerenciando as células e verificando vitórias.
/// </summary>
public class Board
{
    public const int Size = 3;
    private readonly Player?[,] _cells = new Player?[Size, Size];

    public Player? this[int row, int col]
    {
        get => _cells[row, col];
    }

    public bool IsEmpty(Move move) => _cells[move.Row, move.Col] is null;

    public bool TryPlace(Move move, Player player)
    {
        if (!move.InBounds(Size)) return false;
        if (!IsEmpty(move)) return false;
        _cells[move.Row, move.Col] = player;
        return true;
    }

    public bool IsFull()
    {
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (_cells[r, c] is null) return false;
        return true;
    }

    public bool HasWinningLine(Player p)
    {
        // rows
        for (int r = 0; r < Size; r++)
        {
            var win = true;
            for (int c = 0; c < Size; c++) win &= _cells[r, c] == p;
            if (win) return true;
        }
        // cols
        for (int c = 0; c < Size; c++)
        {
            var win = true;
            for (int r = 0; r < Size; r++) win &= _cells[r, c] == p;
            if (win) return true;
        }
        // diag
        var d1 = true; for (int i = 0; i < Size; i++) d1 &= _cells[i, i] == p;
        if (d1) return true;
        // anti-diag
        var d2 = true; for (int i = 0; i < Size; i++) d2 &= _cells[i, Size - 1 - i] == p;
        if (d2) return true;
        return false;
    }

    public void Clear()
    {
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                _cells[r, c] = null;
    }

    public void ResetBoard()
    {
        _engine.Reset();
        _repo.Save(_engine.State);
    }
}
