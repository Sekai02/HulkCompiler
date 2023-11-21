namespace HULKLibrary;

/// <summary>
/// Useful functions that are used by other classes
/// </summary>
public static class Utils
{
    /// <summary>
    /// Current stack calls
    /// </summary>
    public static int callCount = 0;
    /// <summary>
    /// Maximum number of calls admited by stack
    /// </summary>
    public readonly static int stackLimit = 10000;

    /// <summary>
    /// Checks if a value is number(double) or not
    /// </summary>
    /// <param name="value">value to check</param>
    /// <returns>True if it is double, false otherwise</returns>
    public static bool OperandIsNumber(object value)
    {
        return value is double;
    }

    /// <summary>
    /// Checks if a value is bool or not
    /// </summary>
    /// <param name="value">value to check</param>
    /// <returns>True if it is bool, false otherwise</returns>
    public static bool OperandIsBool(object value)
    {
        return value is bool;
    }

    /// <summary>
    /// Checks if both operands are number(double)
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <returns>True if both are true, false otherwise</returns>
    public static bool OperandsAreNumbers(object left, object right)
    {
        return left is double && right is double;
    }

    /// <summary>
    /// Checks if both operands are bool
    /// </summary>
    /// <param name="left">left operand</param>
    /// <param name="right">right operand</param>
    /// <returns>True if both are true, false otherwise</returns>
    public static bool OperandsAreBool(object left, object right)
    {
        return left is bool && right is bool;
    }

    /// <summary>
    /// Checks if two values are equal
    /// </summary>
    /// <param name="left">left value</param>
    /// <param name="right">right value</param>
    /// <returns>True if are equal, false otherwise</returns>
    public static bool IsEqual(object left, object right)
    {
        if (left == null && right == null) return true;
        if (left == null) return false;

        return left.Equals(right);
    }
}