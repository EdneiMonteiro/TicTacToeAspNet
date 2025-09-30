namespace TicTacToe.Domain.AI;

/// <summary>
/// Implementação de IA usando o algoritmo Minimax para jogar de forma ótima no jogo da velha.
/// </summary>
public class MinimaxAi : IAiPlayer
{
    public int GetBestMove(char[] board, char aiMark)
    {
        if (board == null || board.Length != 9)
            return -1;

        var humanMark = aiMark == 'X' ? 'O' : 'X';
        var bestMove = -1;
        var bestScore = int.MinValue;

        // Try cada jogada possível
        for (int i = 0; i < 9; i++)
        {
            if (IsEmpty(board, i))
            {
                // Fazer jogada
                board[i] = aiMark;
                
                // Calcular score - próximo é o humano (minimizing player)
                var score = Minimax(board, 0, false, aiMark, humanMark);
                
                // Desfazer jogada
                board[i] = '\0';
                
                // Verificar se é melhor
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = i;
                }
                else if (score == bestScore && IsBetterPosition(i, bestMove))
                {
                    bestMove = i;
                }
            }
        }

        return bestMove;
    }

    private int Minimax(char[] board, int depth, bool isMaximizing, char aiMark, char humanMark)
    {
        // Verificar game over
        var winner = GetWinner(board);
        if (winner == aiMark) return 10 - depth;
        if (winner == humanMark) return depth - 10;
        if (IsBoardFull(board)) return 0;

        if (isMaximizing) // AI turn
        {
            var bestScore = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (IsEmpty(board, i))
                {
                    board[i] = aiMark;
                    var score = Minimax(board, depth + 1, false, aiMark, humanMark);
                    board[i] = '\0';
                    bestScore = Math.Max(score, bestScore);
                }
            }
            return bestScore;
        }
        else // Human turn
        {
            var bestScore = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (IsEmpty(board, i))
                {
                    board[i] = humanMark;
                    var score = Minimax(board, depth + 1, true, aiMark, humanMark);
                    board[i] = '\0';
                    bestScore = Math.Min(score, bestScore);
                }
            }
            return bestScore;
        }
    }

    private char? GetWinner(char[] board)
    {
        var lines = new int[][]
        {
            new[] {0, 1, 2}, new[] {3, 4, 5}, new[] {6, 7, 8}, // rows
            new[] {0, 3, 6}, new[] {1, 4, 7}, new[] {2, 5, 8}, // cols
            new[] {0, 4, 8}, new[] {2, 4, 6}                   // diagonals
        };

        foreach (var line in lines)
        {
            if (board[line[0]] != '\0' && board[line[0]] != ' ' &&
                board[line[0]] == board[line[1]] &&
                board[line[1]] == board[line[2]])
            {
                return board[line[0]];
            }
        }
        return null;
    }

    private bool IsEmpty(char[] board, int index)
    {
        return board[index] == '\0' || board[index] == ' ';
    }

    private bool IsBoardFull(char[] board)
    {
        for (int i = 0; i < 9; i++)
        {
            if (IsEmpty(board, i))
                return false;
        }
        return true;
    }

    // Determinismo: centro > cantos > laterais
    private bool IsBetterPosition(int newPos, int currentBest)
    {
        if (currentBest == -1) return true;
        
        var newPriority = GetPositionPriority(newPos);
        var currentPriority = GetPositionPriority(currentBest);
        
        return newPriority > currentPriority;
    }

    private int GetPositionPriority(int position)
    {
        if (position == 4) return 3; // centro
        if (position == 0 || position == 2 || position == 6 || position == 8) return 2; // cantos
        return 1; // laterais
    }
}