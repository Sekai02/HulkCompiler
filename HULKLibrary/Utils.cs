namespace HULKLibrary;

public static class Utils
{
    public static int callCount = 0;
    public readonly static int stackLimit = 10000;

    public static bool operandIsNumber(object value)
    {
        return value is double;
    }

    public static bool operandIsBool(object value)
    {
        return value is bool;
    }

    public static bool operandsAreNumbers(object left, object right)
    {
        return left is double && right is double;
    }

    public static bool operandsAreBool(object left, object right)
    {
        return left is bool && right is bool;
    }

    public static bool isEqual(object left, object right)
    {
        if (left == null && right == null) return true;
        if (left == null) return false;

        return left.Equals(right);
    }
}