namespace Jagapippi.Layer2.Editor
{
    public static class StringExtensions
    {
        public static string ReplaceSpaceForPopup(this string self)
        {
            // SEE: https://qiita.com/su10/items/ab33adefda8c2f7423e3
            return self?.Replace(" ", "\u00A0");
        }
    }
}
