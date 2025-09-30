namespace TicTacToe.Domain;

/// <summary>
/// Representa uma jogada no tabuleiro, contendo linha e coluna.
/// </summary>
public readonly record struct Move(int Row, int Col)
{
    public bool InBounds(int size = 3) => Row >= 0 && Row < size && Col >= 0 && Col < size;
}
