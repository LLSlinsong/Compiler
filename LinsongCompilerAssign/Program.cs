using System;
using System.IO;

namespace LinsongCompilerAssign
{
    class Program
    {
        static void Main(string[] args)
        {
            string FilePath = "";
            if (args.Length == 0)
            {
                Console.Write("Name not found in command line, Please enter the names of the inputfile(with extension)");
                FilePath = Console.ReadLine();
                args = FilePath.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
            string File1 = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, args[0]);
            #region print modify
            for (int i = 0; i < 25; i++)
                Console.Write("*");
            Console.WriteLine();
            Console.WriteLine("Following is LexicalAnalyzer:");
            for (int i = 0; i < 25; i++)
                Console.Write("*");
            Console.WriteLine();
            #endregion
            LexicalAnalyzer.LexicalA(File1);
            #region print modify 2
            for (int i = 0; i < 25; i++)
                Console.Write("*");
            Console.WriteLine();
            for (int i = 0; i < 25; i++)
                Console.Write("*");
            Console.WriteLine();
            Console.WriteLine("Following is Parser:");
            for (int i = 0; i < 25; i++)
                Console.Write("*");
            Console.WriteLine();
            #endregion
            parser.Parser();
            #region print modify 3
            for (int i = 0; i < 25; i++)
                Console.Write("*");
            Console.WriteLine();
            #endregion
        }
    }
}
