using System;
using System.Text;
using HULKLibrary;

namespace HULKEngine
{
    /// <summary>
    /// Class to handle the logic and input of the program
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            Functions.Init();

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