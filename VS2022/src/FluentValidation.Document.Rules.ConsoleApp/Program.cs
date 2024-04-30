using FluentValidation.Document.Rules.enums;

namespace FluentValidation.Document.Rules.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var x = new DocumentRules();
            x.DocumentAssembly(OutputType.All);
        }
    }
}