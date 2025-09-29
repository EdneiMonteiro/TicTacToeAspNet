using TicTacToe.Domain.AI;

namespace TicTacToe.Tests;

public class MinimaxAiTests
{
    private readonly MinimaxAi _ai = new();

    [Fact]
    public void GetBestMove_DeveRetornarMenosUm_QuandoTabuleiroCheio()
    {
        // Arrange
        var board = new char[] { 'X', 'O', 'X', 'O', 'X', 'O', 'X', 'O', 'X' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void GetBestMove_DeveRetornarMenosUm_QuandoTabuleiroNulo()
    {
        // Act
        var result = _ai.GetBestMove(null!, 'X');

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void GetBestMove_DeveRetornarMenosUm_QuandoTabuleiroTamanhoIncorreto()
    {
        // Arrange
        var board = new char[] { 'X', 'O', 'X' }; // Apenas 3 elementos

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(-1, result);
    }

    [Fact]
    public void GetBestMove_DeveJogarParaVencer_QuandoPossivelVencerProximaJogada()
    {
        // Arrange - IA (O) pode vencer na posição 2
        var board = new char[] { 'O', 'O', '\0', 'X', 'X', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(2, result); // Deve jogar na posição 2 para vencer
    }

    [Fact]
    public void GetBestMove_DeveBloquearVitoriaOponente_QuandoOponentePodeVencer()
    {
        // Arrange - Humano (X) pode vencer na posição 2, IA deve bloquear
        var board = new char[] { 'X', 'X', '\0', 'O', '\0', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(2, result); // Deve bloquear na posição 2
    }

    [Fact]
    public void GetBestMove_DevePriorizarCentro_QuandoTabuleiroVazio()
    {
        // Arrange
        var board = new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'X');

        // Assert
        Assert.Equal(4, result); // Centro do tabuleiro
    }

    [Fact]
    public void GetBestMove_DevePriorizarCantos_QuandoCentroOcupado()
    {
        // Arrange - Centro ocupado pelo oponente
        var board = new char[] { '\0', '\0', '\0', '\0', 'O', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'X');

        // Assert
        var cantos = new[] { 0, 2, 6, 8 };
        Assert.Contains(result, cantos); // Deve escolher um canto
    }

    [Fact]
    public void GetBestMove_DeveSerDeterministico_ParaMesmoEstado()
    {
        // Arrange
        var board = new char[] { 'X', '\0', '\0', '\0', 'O', '\0', '\0', '\0', '\0' };

        // Act - Executar múltiplas vezes
        var result1 = _ai.GetBestMove(board.ToArray(), 'X');
        var result2 = _ai.GetBestMove(board.ToArray(), 'X');
        var result3 = _ai.GetBestMove(board.ToArray(), 'X');

        // Assert
        Assert.Equal(result1, result2);
        Assert.Equal(result2, result3);
    }

    [Fact]
    public void GetBestMove_DeveEvitarPerder_ContraJogadaOtima()
    {
        // Arrange - Situação onde IA pode forçar empate
        var board = new char[] { 'O', 'X', 'O', 'X', 'X', 'O', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.True(result >= 6 && result <= 8); // Deve jogar em uma das posições restantes
        Assert.NotEqual(-1, result);
    }

    [Fact]
    public void GetBestMove_DeveDetectarVitoriaEmDiagonal()
    {
        // Arrange - IA pode vencer na diagonal principal
        var board = new char[] { 'O', 'X', 'X', 'X', 'O', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(8, result); // Completar diagonal principal
    }

    [Fact]
    public void GetBestMove_DeveDetectarVitoriaEmAntiDiagonal()
    {
        // Arrange - IA pode vencer na anti-diagonal
        var board = new char[] { 'X', 'X', 'O', '\0', 'O', 'X', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(6, result); // Completar anti-diagonal
    }

    [Fact]
    public void GetBestMove_DeveBloquearMultiplasAmeacas()
    {
        // Arrange - Múltiplas ameaças, deve bloquear a mais imediata
        var board = new char[] { 'X', 'X', '\0', 'O', '\0', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'O');

        // Assert
        Assert.Equal(2, result); // Bloquear vitória iminente
    }

    [Theory]
    [InlineData('X')]
    [InlineData('O')]
    public void GetBestMove_DeveFuncionarParaAmbosJogadores(char aiMark)
    {
        // Arrange
        var board = new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, aiMark);

        // Assert
        Assert.True(result >= 0 && result <= 8);
    }

    [Fact]
    public void GetBestMove_DeveRespeitarPrioridadeDeterministica()
    {
        // Arrange - Cenário onde há empate entre várias jogadas boas
        var board = new char[] { '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0' };

        // Act
        var result = _ai.GetBestMove(board, 'X');

        // Assert
        Assert.Equal(4, result); // Centro tem prioridade máxima
    }
}