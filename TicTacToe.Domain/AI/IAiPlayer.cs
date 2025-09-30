namespace TicTacToe.Domain.AI;

/// <summary>
/// Interface para implementações de jogadores de IA que podem escolher a melhor jogada.
/// </summary>
public interface IAiPlayer
{
    int GetBestMove(char[] board, char aiMark);
}