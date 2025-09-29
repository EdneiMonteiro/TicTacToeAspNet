using TicTacToe.Domain;
using TicTacToe.Domain.AI;

namespace TicTacToe.Tests;

public class BoardConverterTests
{
    [Fact]
    public void ToCharArray_DeveConverterTabuleiroVazio()
    {
        // Arrange
        var board = new Board();

        // Act
        var result = BoardConverter.ToCharArray(board);

        // Assert
        Assert.Equal(9, result.Length);
        Assert.All(result, c => Assert.Equal('\0', c));
    }

    [Fact]
    public void ToCharArray_DeveConverterTabuleiroComJogadas()
    {
        // Arrange
        var board = new Board();
        board.TryPlace(new Move(0, 0), Player.X);
        board.TryPlace(new Move(1, 1), Player.O);
        board.TryPlace(new Move(2, 2), Player.X);

        // Act
        var result = BoardConverter.ToCharArray(board);

        // Assert
        Assert.Equal('X', result[0]); // Posição (0,0)
        Assert.Equal('O', result[4]); // Posição (1,1)
        Assert.Equal('X', result[8]); // Posição (2,2)
        Assert.Equal('\0', result[1]); // Posição vazia
    }

    [Fact]
    public void ToMoveCoords_DeveConverterIndiceParaCoordenadas()
    {
        // Arrange & Act & Assert
        Assert.Equal(new Move(0, 0), BoardConverter.ToMoveCoords(0));
        Assert.Equal(new Move(0, 1), BoardConverter.ToMoveCoords(1));
        Assert.Equal(new Move(0, 2), BoardConverter.ToMoveCoords(2));
        Assert.Equal(new Move(1, 0), BoardConverter.ToMoveCoords(3));
        Assert.Equal(new Move(1, 1), BoardConverter.ToMoveCoords(4));
        Assert.Equal(new Move(1, 2), BoardConverter.ToMoveCoords(5));
        Assert.Equal(new Move(2, 0), BoardConverter.ToMoveCoords(6));
        Assert.Equal(new Move(2, 1), BoardConverter.ToMoveCoords(7));
        Assert.Equal(new Move(2, 2), BoardConverter.ToMoveCoords(8));
    }

    [Fact]
    public void ToChar_DeveConverterPlayerParaChar()
    {
        // Arrange & Act & Assert
        Assert.Equal('X', BoardConverter.ToChar(Player.X));
        Assert.Equal('O', BoardConverter.ToChar(Player.O));
        Assert.Equal('\0', BoardConverter.ToChar(null));
    }

    [Fact]
    public void ToCharArray_DeveRespeitarOrdemCorreta()
    {
        // Arrange - Tabuleiro 3x3 preenchido em ordem
        var board = new Board();
        var expectedPlayers = new Player?[] 
        { 
            Player.X, Player.O, Player.X, 
            Player.O, Player.X, Player.O, 
            Player.X, Player.O, null 
        };

        for (int r = 0; r < Board.Size; r++)
        {
            for (int c = 0; c < Board.Size; c++)
            {
                var index = r * Board.Size + c;
                if (expectedPlayers[index] != null)
                {
                    board.TryPlace(new Move(r, c), expectedPlayers[index]!.Value);
                }
            }
        }

        // Act
        var result = BoardConverter.ToCharArray(board);

        // Assert
        for (int i = 0; i < 9; i++)
        {
            var expected = expectedPlayers[i] switch
            {
                Player.X => 'X',
                Player.O => 'O',
                null => '\0',
                _ => '\0'
            };
            Assert.Equal(expected, result[i]);
        }
    }
}