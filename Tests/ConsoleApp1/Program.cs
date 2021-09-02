using System;
using FluentValidation.Document.Rules;
using FluentValidation.Document.Rules.enums;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using var x = new DocumentRules();
            x.DocumentAssembly(OutputType.HTML);
        }
    }
}