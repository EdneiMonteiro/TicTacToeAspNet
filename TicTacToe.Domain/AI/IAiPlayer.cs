namespace TicTacToe.Domain.AI;

public interface IAiPlayer
{
    int GetBestMove(char[] board, char aiMark);
}