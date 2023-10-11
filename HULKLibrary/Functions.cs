namespace HULKLibrary;

public class Functions
{
    public static bool contains(string name)
    {
        return functions.ContainsKey(name);
    }

    public static Expression.Function get(string name)
    {
        return functions[name];
    }

    public static void add(string name, Expression.Function function = null)
    {
        functions[name] = function;
    }

    public static void erase(string name)
    {
        functions.Remove(name);
    }

    public static void init()
    {
        add("print");
        add("sqrt");
        add("sin");
        add("cos");
        add("exp");
        add("log");
        add("rand");
    }

    public static Dictionary<string, Expression.Function> functions = new Dictionary<string, Expression.Function>();
}