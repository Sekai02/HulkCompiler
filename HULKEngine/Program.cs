using System;
using System.Text;
using HULKLibrary;

namespace HULKEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: HULK [script]");
                Console.WriteLine("! EXECUTION ERROR: Number of args should be less than 2");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                HULK.RunFile(args[0]);
            }
            else
            {
                HULK.RunPrompt();
            }
        }
    }
}