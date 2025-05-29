using PdfSharp.Fonts;
using System.Reflection;
using System.IO;

public class CustomFontResolver : IFontResolver
{
    public byte[] GetFont(string faceName)
    {
        if (faceName == "Arial#Regular")
        {
            string path = @"C:\Windows\Fonts\Arial.ttf";
            return File.ReadAllBytes(path);
        }

        return null;
    }

    public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
    {
        if (familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
        {
            return new FontResolverInfo("Arial#Regular");
        }

        return null;
    }
}
