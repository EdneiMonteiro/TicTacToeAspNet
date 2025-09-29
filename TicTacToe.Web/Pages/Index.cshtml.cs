using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TicTacToe.Domain;

namespace TicTacToe.Web.Pages;

public class IndexModel : PageModel
{
    private const string SessionKey = "tictactoe.game";

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
        if (!game.MakeMove(new Move(row, col), out var err))
        {
            Error = err;
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

// DTO para serializar estado do jogo na sessão
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
