using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TicTacToe.Web;

public static class SessionExtensions
{
    public static void SetObject<T>(this ISession session, string key, T value) =>
        session.SetString(key, JsonSerializer.Serialize(value));

    public static T? GetObject<T>(this ISession session, string key) =>
        session.TryGetValue(key, out _) ? JsonSerializer.Deserialize<T>(session.GetString(key)!) : default;
}
