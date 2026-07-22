public struct Mult
{
    /// <summary>
    /// Combines all the given multipliers additively
    /// </summary>
    /// <param name="mults"></param>
    /// <returns>The final multiplier</returns>
    public static float Additive(float[] mults)
    {
        float sum = 0;
        foreach (var m in mults)
            sum += m - 1;

        return 1 + sum;
    }

    /// <summary>
    /// Combines all the given multipliers multiplicatively
    /// </summary>
    /// <param name="mults"></param>
    /// <returns>The final multiplier</returns>
    public static float Multiplicative(float[] mults)
    {
        float mult = 1;
        foreach (var m in mults)
            mult *= m;

        return mult;
    }
}