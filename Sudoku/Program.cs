namespace Sudoku;

public class Program
{
    public static void Main()
    {
        var sudoku = Sudoku.FromIntegerArray(new [,]
        {
            { 0, 0, 1, 0, 2, 9, 7, 0, 0 },
            { 7, 0, 0, 0, 0, 0, 0, 6, 8 },
            { 0, 0, 4, 7, 0, 8, 0, 0, 2 },
            { 2, 0, 9, 0, 7, 0, 4, 8, 0 },
            { 8, 0, 6, 0, 0, 0, 0, 7, 3 },
            { 0, 0, 0, 0, 0, 2, 1, 0, 0 },
            { 0, 0, 7, 0, 0, 0, 3, 0, 0 },
            { 0, 0, 5, 4, 0, 0, 0, 0, 0 },
            { 0, 2, 0, 9, 0, 5, 6, 0, 7 }
        });

        /*var sudoku = Sudoku.FromIntegerArray(new[,]
        {
            {0, 9, 0, 0, 0, 0, 0, 0, 1},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 2, 0, 0, 0, 0},
            {0, 2, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 0, 0, 0, 0, 0, 0, 0},
            {0, 0, 1, 0, 0, 0, 0, 4, 0},
            {0, 0, 3, 0, 0, 0, 0, 0, 0}
        });*/

        Console.WriteLine($"Input:\n{sudoku}\n");

        var result = sudoku.Solve();
        if (result)
            Console.WriteLine($"Solved successfully\n{sudoku}");
        else
            Console.WriteLine("Sudoku was not solveable");
    }
}
