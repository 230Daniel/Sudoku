namespace Sudoku.Extensions;

public static class ArrayExtensions
{
    public static T[] Flatten<T>(this T[,] input)
    {
        var size = input.Length;
        var result = new T[size];

        var write = 0;
        for (var i = 0; i <= input.GetUpperBound(0); i++)
        {
            for (var j = 0; j <= input.GetUpperBound(1); j++)
            {
                result[write++] = input[i, j];
            }
        }

        return result;
    }
}
