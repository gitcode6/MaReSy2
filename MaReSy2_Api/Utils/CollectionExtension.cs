namespace MaReSy2_Api.Extensions
{
    public static class CollectionExtension
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null || source.Any()) { return false; }
            return source.Any();
        }
    }
}
