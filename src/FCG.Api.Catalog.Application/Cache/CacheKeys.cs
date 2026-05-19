namespace FCG.Api.Catalog.Application.Cache;

public static class CacheKeys
{
    public static string GamesAll(int page, int size) => $"games:all:{page}:{size}";
    public static string GamesActive(int page, int size) => $"games:active:{page}:{size}";

    public static IEnumerable<string> GamesInvalidationKeys()
    {
        foreach (var size in new[] { 10, 20, 50 })
        foreach (var page in Enumerable.Range(1, 5))
        {
            yield return GamesAll(page, size);
            yield return GamesActive(page, size);
        }
    }
}
