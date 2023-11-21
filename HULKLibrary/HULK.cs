using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Transactions;

namespace HULKLibrary;

/// <summary>
/// This is the HULKEngine, it handles the logic of the different parts of the program
/// </summary>
public class HULK
{
    public static int Line = 1;

    /// <summary>
    /// Runs a given file
    /// </summary>
    /// <param name="path">File to run</param>
    public static void RunFile(string path)
    {
        HULK.Line = 1;
        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        RunMultiLine(Encoding.Default.GetString(bytes));

        if (Error.hadError) Environment.Exit(65);
    }

    /// <summary>
    /// Runs line by line, like a python console
    /// </summary>
    public static void RunPrompt()
    {
        HULK.Line = 1;
        while (true)
        {
            Console.Write("[{0}] > ", HULK.Line);
            string line = Console.ReadLine()!;
            if (line == null) break;
            Run(line);
            Error.hadError = false;
            HULK.Line++;
        }
    }

    /// <summary>
    /// Runs the given code
    /// </summary>
    /// <param name="source">string representing the code to run</param>
    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.ScanTokens();

        if (Error.hadError) return;

        Parser parser = new Parser(tokens);
        Expression Ast = parser.Parse();

        if (Error.hadError) return;

        Evaluate evaluate = new Evaluate(Ast);
        object output = evaluate.Run();
        if (output != null) Console.WriteLine(output is bool ? output.ToString()!.ToLower() : output);
    }

    /// <summary>
    /// Runs the code from the text of a file in multiple lines
    /// </summary>
    /// <param name="source">string code to run</param>
    private static void RunMultiLine(string source)
    {
        List<string> lines = new List<string>();

        string line = "";
        foreach (char c in source)
        {
            if (c == '\n')
            {
                HULK.Line++;
                if (line.Length > 0)
                {
                    Run(line);
                    line = "";
                }
            }
            else line += c;
        }
        if (line.Length > 0) Run(line);
    }
}