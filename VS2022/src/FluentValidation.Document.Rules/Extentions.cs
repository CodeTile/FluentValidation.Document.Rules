using System.IO;

namespace FluentValidation.Document.Rules
{
    public static class Extentions
    {
        public static string NameNoExtension(this FileInfo fi)
        {
            int nameLength = fi.Name.Length - fi.Extension.Length;
            return fi.Name.Substring(0, nameLength);
        }
    }
}