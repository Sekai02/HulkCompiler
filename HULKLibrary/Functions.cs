namespace HULKLibrary;

/// <summary>
/// Function Handler Class
/// </summary>
public class Functions
{
    /// <summary>
    /// Checks if the given function is declared
    /// </summary>
    /// <param name="name">name of the function to look for</param>
    /// <returns>true if the function exists, false otherwise</returns>
    public static bool Contains(string name)
    {
        return functions.ContainsKey(name);
    }

    /// <summary>
    /// Gets the declaration of the given function
    /// </summary>
    /// <param name="name">Function to look for</param>
    /// <returns>The expression of the given function to look for</returns>
    public static Expression.Function Get(string name)
    {
        return functions[name];
    }

    /// <summary>
    /// Adds a Function to the scope
    /// </summary>
    /// <param name="name">Name of the function to add</param>
    /// <param name="function">Declaration of the function to add</param>
    public static void Add(string name, Expression.Function function = null!)
    {
        functions[name] = function;
    }


    /// <summary>
    /// Erases the given function
    /// </summary>
    /// <param name="name">Name of the function to erase</param>
    public static void Erase(string name)
    {
        functions.Remove(name);
    }

    /// <summary>
    /// Initializes language functions
    /// </summary>
    public static void Init()
    {
        Add("print");
        Add("sqrt");
        Add("sin");
        Add("cos");
        Add("exp");
        Add("log");
        Add("rand");
    }

    public static Dictionary<string, Expression.Function> functions = new Dictionary<string, Expression.Function>();
}