using System.Text;

namespace Sudoku;

public partial class Sudoku
{
    public Tile[,] Grid { get; }

    public Sudoku()
    {
        Grid = new Tile[9, 9];
    }

    public static Sudoku FromIntegerArray(int[,] array)
    {
        if (array.GetLength(0) != 9 || array.GetLength(1) != 9)
            throw new ArgumentException("Given 2D array must be 9x9", nameof(array));

        var sudoku = new Sudoku();

        for (var x = 0; x < 9; x++)
        {
            for (var y = 0; y < 9; y++)
            {
                var value = array[y, x];
                if (value == 0) sudoku.Grid[x, y] = Tile.Empty(x, y);
                else sudoku.Grid[x, y] = Tile.FromValue(value, x, y);
            }
        }

        return sudoku;
    }

    public static Sudoku Empty()
    {
        var sudoku = new Sudoku();

        for (var x = 0; x < 9; x++)
        {
            for (var y = 0; y < 9; y++)
            {
                sudoku.Grid[x, y] = Tile.Empty(x, y);
            }
        }

        return sudoku;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("╔═════════╤═════════╤═════════╗\n");
        for (var y = 0; y < 9; y++)
        {
            sb.Append('║');
            for (var x = 0; x < 9; x++)
            {
                sb.Append(Grid[x, y]);
                if ((x + 1) % 3 == 0 && x != 8)
                    sb.Append('│');
            }
            sb.Append("║\n");

            if ((y + 1) % 3 == 0 && y != 8)
                sb.Append("╟─────────┼─────────┼─────────╢\n");
        }
        sb.Append("╚═════════╧═════════╧═════════╝");

        return sb.ToString();
    }
}
