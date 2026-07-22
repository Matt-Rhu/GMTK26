using System;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;

public struct NumberFormatting
{
    public static int WrapAroundRange(int value, int min, int max)
    {
        int length = max - min + 1;
        return min + ((value - min) % length + length) % length;
    }

    //haven't tested the float version yet
    public static float WrapAroundRange(float value, float min, float max)
    {
        float length = max - min;
        return min + ((value - min) % length + length) % length;
    }
    
    

    //DECIMALS & THOUSANDS FORMATTING

    /// <summary>
    /// Converts a number to a string with a separator between groups of thousands
    /// </summary>
    /// <param name="input">The number to convert</param>
    /// <param name="separator">The separator between thousands</param>
    /// <param name="decimals">Number of decimals displayed</param>
    /// <param name="padWithZeros">Whether decimals will be padded with 0s to fit the number of decimals</param>
    /// <returns></returns>
    public static string SpaceOutThousands(float input, string separator = " ", int decimals = 0, bool padWithZeros = true)
    {
        var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
        nfi.NumberGroupSeparator = separator;
        return input.ToString("#,0."+Dec(decimals, padWithZeros), nfi);
    }
    
    /// <summary>
    /// Converts a number to a string with the chosen number of decimals
    /// </summary>
    /// <param name="input">The number to convert</param>
    /// <param name="decimals">The number of decimals to display</param>
    /// <param name="padWithZeros">Whether decimals will be padded with 0s to fit the specified number of decimals</param>
    /// <returns></returns>
    public static string Decimals(float input, int decimals, bool padWithZeros = true)
    {
        return input.ToString("0."+Dec(decimals, padWithZeros), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a number to a string and pads it with a number of 0s.
    /// </summary>
    /// <param name="input">The number to convert</param>
    /// <param name="padRanks">The number of ranks to pad for</param>
    /// <returns></returns>
    public static string SetPadding(float input, int padRanks)
    {
        return input.ToString(Pad(padRanks), CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Converts a number to a string, pads it with a number of 0s and displays a number of decimals
    /// </summary>
    /// <param name="input">The number to convert</param>
    /// <param name="padRanks">The number of ranks to pad for</param>
    /// <param name="decimals">The number of decimals to display</param>
    /// <param name="padDecimalsWithZeros">Whether decimals will be padded with 0s to fit the specified number of decimals</param>
    /// <returns></returns>
    public static string SetPaddingAndDecimals(float input, int padRanks, int decimals, bool padDecimalsWithZeros = true)
    {
        return input.ToString(Pad(padRanks) + "." + Dec(decimals, padDecimalsWithZeros), CultureInfo.InvariantCulture);
    }

    private static string Pad(int ranks)
    {
        if (ranks < 1) ranks = 1;
        
        string pad = String.Empty;
        for (int i = 0; i < ranks; i++)
            pad += "0";

        return pad;
    }

    private static string Dec(int decimals, bool padWithZeros = true)
    {
        string dec = String.Empty;
        string format = padWithZeros ? "0" : "#";
        
        for (int i = 0; i < decimals; i++)
            dec += format;

        return dec;
    }
    
    
    
    //TIME CONVERSIONS
    
    /// <summary>
    /// Converts a number of seconds into a string formatted mm:ss.ddd
    /// </summary>
    /// <param name="seconds">Seconds to convert</param>
    /// <param name="decimals">Number of decimals to display</param>
    /// <returns></returns>
    public static string SecToMinSec(float seconds, int decimals = 0)
    {
        return UnitUp(seconds) + ":" + UnitDown(seconds, decimals);
    }

    /// <summary>
    /// Converts a number of seconds into a string formatted hh:mm:ss
    /// </summary>
    /// <param name="seconds">Seconds to convert</param>
    /// <returns></returns>
    public static string SecToHourMinSec(float seconds)
    {
        int minutes = UnitUp(seconds);
        
        return UnitUp(minutes) + ":" + UnitDown(minutes) + ":" + UnitDown(seconds);
    }
    
    private static int UnitUp(float input)
    {
        return Mathf.FloorToInt(input / 60);
    }

    private static string UnitDown(float input, int decimals = 0)
    {
        return SetPaddingAndDecimals(input % 60, 2, decimals);
    }
}
