namespace TicTacToe.Domain.AI;

/// <summary>
/// Classe utilitária para conversão entre representações de tabuleiro e coordenadas.
/// </summary>
public static class BoardConverter
{
    public static char[] ToCharArray(Board board)
    {
        var result = new char[9];
        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                var index = r * Board.Size + c;
                var player = board[r, c];
                result[index] = player switch
                {
                    Player.X => 'X',
                    Player.O => 'O',
                    null => '\0'
                };
            }
        }
        return result;
    }

    public static int ToMove(int boardIndex)
    {
        // Converte índice do array (0-8) para posição (row, col)
        return boardIndex;
    }

    public static Move ToMoveCoords(int boardIndex)
    {
        var row = boardIndex / Board.Size;
        var col = boardIndex % Board.Size;
        return new Move(row, col);
    }

    public static char ToChar(Player? player)
    {
        return player switch
        {
            Player.X => 'X',
            Player.O => 'O',
            null => '\0',
            _ => '\0'
        };
    }
}