using UnityEngine;

public static class GameColors
{
    // total count (used for cycling)
    public static int Count => 4;

    // map color type to Unity Color
    public static Color ToColor(ColorType t) => t switch
    {
        ColorType.Red => Color.red,
        ColorType.Green => Color.green,
        ColorType.Yellow => Color.yellow,
        ColorType.Blue => Color.blue,
        _ => Color.white
    };
}
