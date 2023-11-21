using System;
using System.Linq.Expressions;
using System.Text;

namespace HULKLibrary;

/// <summary>
/// This is the HULKEngine, it handles the logic of the different parts of the program
/// </summary>
public class HULK
{
    /// <summary>
    /// Runs a given file
    /// </summary>
    /// <param name="path">File to run</param>
    public static void RunFile(string path)
    {
        Scanner.line = 0;
        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.Default.GetString(bytes));

        if (Error.hadError) Environment.Exit(65);
    }

    /// <summary>
    /// Runs line by line, like a python console
    /// </summary>
    public static void RunPrompt()
    {
        Scanner.line = 0;
        while (true)
        {
            Console.Write("[{0}] > ", Scanner.line + 1);
            string line = Console.ReadLine()!;
            if (line == null) break;
            Run(line);
            Error.hadError = false;
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
}