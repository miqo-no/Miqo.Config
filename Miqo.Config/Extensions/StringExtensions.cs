namespace Miqo.Config
{
    public static class StringExtensions
    {
        public static bool IsNull(this string text)
        {
            return string.IsNullOrWhiteSpace(text);
        }
    }
}
