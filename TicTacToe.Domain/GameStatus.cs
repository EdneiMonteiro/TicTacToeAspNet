namespace TicTacToe.Domain;

/// <summary>
/// Representa os possíveis estados do jogo (em andamento, vitória de X, vitória de O ou empate).
/// </summary>
public enum GameStatus
{
    InProgress,
    XWon,
    OWon,
    Draw
}
