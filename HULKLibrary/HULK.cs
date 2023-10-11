using System;
using System.Linq.Expressions;
using System.Text;

namespace HULKLibrary;

public class HULK
{
    public static void RunFile(string path)
    {
        Scanner.line = 0;
        byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
        Run(Encoding.Default.GetString(bytes));

        if (Error.hadError) Environment.Exit(65);
    }

    public static void RunPrompt()
    {
        Scanner.line = 0;
        while (true)
        {
            Console.Write("line:{0} > ", Scanner.line + 1);
            string line = Console.ReadLine();
            if (line == null) break;
            Run(line);
            Error.hadError = false;
        }
    }

    private static void Run(string source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        if (Error.hadError) return;

        Parser parser = new Parser(tokens);
        Expression Ast = parser.parse();

        if (Error.hadError) return;

        Evaluate evaluate = new Evaluate(Ast);
        object output = evaluate.run();
        if (output != null) Console.WriteLine(output);
    }
}