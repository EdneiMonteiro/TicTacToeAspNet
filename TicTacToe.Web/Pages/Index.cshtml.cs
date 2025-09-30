using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicTacToe.Domain;
using TicTacToe.Domain.AI;

namespace TicTacToe.Web.Pages;

/// <summary>
/// Model da página principal que gerencia o jogo da velha, incluindo jogadas do usuário e da IA.
/// </summary>
public class IndexModel : PageModel
{
    private const string SessionKey = "tictactoe.game";
    private readonly IAiPlayer _aiPlayer;

    public IndexModel(IAiPlayer aiPlayer)
    {
        _aiPlayer = aiPlayer;
    }

    public GameStatus Status { get; private set; }
    public Player CurrentPlayer { get; private set; }
    public Player?[,] Board { get; private set; } = new Player?[TicTacToe.Domain.Board.Size, TicTacToe.Domain.Board.Size];
    public string? Error { get; private set; }

    public void OnGet()
    {
        var game = GetGame();
        LoadFrom(game);
    }

    public IActionResult OnPostMove(int row, int col)
    {
        var game = GetGame();
        
        // Fazer jogada do humano
        if (!game.MakeMove(new Move(row, col), out var err))
        {
            Error = err;
            SaveGame(game);
            LoadFrom(game);
            return Page();
        }

        // Se o jogo ainda está em progresso e é vez da IA (Player.O)
        if (game.Status == GameStatus.InProgress && game.CurrentPlayer == Player.O)
        {
            // Fazer jogada da IA
            var board = BoardConverter.ToCharArray(game.Board);
            var aiMove = _aiPlayer.GetBestMove(board, 'O');
            
            if (aiMove >= 0)
            {
                var aiMoveCoords = BoardConverter.ToMoveCoords(aiMove);
                game.MakeMove(aiMoveCoords, out _); // IA sempre faz jogadas válidas
            }
        }

        SaveGame(game);
        LoadFrom(game);
        return Page();
    }

    public IActionResult OnPostReset()
    {
        var game = new GameService();
        SaveGame(game);
        LoadFrom(game);
        return RedirectToPage();
    }

    private GameService GetGame()
    {
        var snapshot = HttpContext.Session.GetObject<GameSnapshot>(SessionKey);
        if (snapshot is null)
        {
            var fresh = new GameService();
            SaveGame(fresh);
            return fresh;
        }
        return snapshot.ToGameService();
    }

    private void SaveGame(GameService game)
    {
        HttpContext.Session.SetObject(SessionKey, GameSnapshot.From(game));
    }

    private void LoadFrom(GameService g)
    {
        Status = g.Status;
        CurrentPlayer = g.CurrentPlayer;
        for (int r = 0; r < Domain.Board.Size; r++)
            for (int c = 0; c < Domain.Board.Size; c++)
            {
                Board[r, c] = g.Board[r, c];
            }
    }
}

/// <summary>
/// DTO para serializar estado do jogo na sessão.
/// </summary>
public record GameSnapshot(Player CurrentPlayer, GameStatus Status, Player?[][] Cells)
{
    public static GameSnapshot From(GameService g)
    {
        var cells = new Player?[Domain.Board.Size][];
        for (int r = 0; r < Domain.Board.Size; r++)
        {
            cells[r] = new Player?[Domain.Board.Size];
            for (int c = 0; c < Domain.Board.Size; c++)
                cells[r][c] = g.Board[r, c];
        }
        return new GameSnapshot(g.CurrentPlayer, g.Status, cells);
    }

    public GameService ToGameService()
    {
        var g = new GameService();
        // Reconstroi o tabuleiro e os campos
        g.Reset();
        // Copia estado
        // Preenche o tabuleiro de forma determinística, mantendo Status e CurrentPlayer
        for (int r = 0; r < Domain.Board.Size; r++)
            for (int c = 0; c < Domain.Board.Size; c++)
            {
                if (Cells[r][c] is Player p)
                {
                    // Ignora erros: célula está vazia ao reconstruir
                    g.Board.TryPlace(new Move(r, c), p);
                }
            }
        typeof(GameService).GetProperty(nameof(GameService.CurrentPlayer))!
            .SetValue(g, CurrentPlayer);
        typeof(GameService).GetProperty(nameof(GameService.Status))!
            .SetValue(g, Status);
        return g;
    }
}
