using TicTacToe.Domain;

namespace TicTacToe.Tests;

public class GameServiceTests
{
    [Fact]
    public void Vitoria_X_na_primeira_linha()
    {
        var game = new GameService();
        string? err;

        Assert.Equal(GameStatus.InProgress, game.Status);
        Assert.Equal(Player.X, game.CurrentPlayer);

        Assert.True(game.MakeMove(new Move(0, 0), out err)); // X
        Assert.Null(err);
        Assert.True(game.MakeMove(new Move(1, 0), out err)); // O
        Assert.True(game.MakeMove(new Move(0, 1), out err)); // X
        Assert.True(game.MakeMove(new Move(1, 1), out err)); // O
        Assert.True(game.MakeMove(new Move(0, 2), out err)); // X wins

        Assert.Equal(GameStatus.XWon, game.Status);
    }

    [Fact]
    public void Jogada_invalida_em_casa_ocupada()
    {
        var game = new GameService();
        string? err;

        Assert.True(game.MakeMove(new Move(0, 0), out err)); // X
        Assert.Null(err);

        var ok = game.MakeMove(new Move(0, 0), out err); // O tenta jogar na mesma
        Assert.False(ok);
        Assert.Equal("Casa ocupada", err);
        Assert.Equal(GameStatus.InProgress, game.Status);
    }

    [Fact]
    public void Empate_sem_vencedor()
    {
        var g = new GameService();
        string? err;
        // Sequência que leva a empate
        g.MakeMove(new Move(0, 0), out err); // X
        g.MakeMove(new Move(0, 1), out err); // O
        g.MakeMove(new Move(0, 2), out err); // X
        g.MakeMove(new Move(1, 1), out err); // O
        g.MakeMove(new Move(1, 0), out err); // X
        g.MakeMove(new Move(1, 2), out err); // O
        g.MakeMove(new Move(2, 1), out err); // X
        g.MakeMove(new Move(2, 0), out err); // O
        g.MakeMove(new Move(2, 2), out err); // X

        Assert.Equal(GameStatus.Draw, g.Status);
    }
}
