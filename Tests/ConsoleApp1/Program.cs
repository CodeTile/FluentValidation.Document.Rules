using System;
using FluentValidation.Document.Rules;

namespace ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            using (var x = new DocumentRules())
            {
                x.DocumentAssembly();
            }

            Console.ReadKey();
        }
    }
}