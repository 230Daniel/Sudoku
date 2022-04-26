using Sudoku.Extensions;

namespace Sudoku;

public partial class Sudoku
{
    public bool IsSolved()
    {
        var valuesInSection = new List<int>();

        // Check tiles in the same row
        for (var y = 0; y < 9; y++)
        {
            valuesInSection.Clear();
            for (var x = 0; x < 9; x++)
            {
                var tile = Grid[x, y];
                if (!tile.Value.HasValue) return false;

                var value = tile.Value.Value;
                if (valuesInSection.Contains(value)) return false;
                valuesInSection.Add(value);
            }
        }

        // Check tiles in the same column
        for (var x = 0; x < 9; x++)
        {
            valuesInSection.Clear();
            for (var y = 0; y < 9; y++)
            {
                var tile = Grid[x, y];
                if (!tile.Value.HasValue) return false;

                var value = tile.Value.Value;
                if (valuesInSection.Contains(value)) return false;
                valuesInSection.Add(value);
            }
        }

        // Check tiles in the same segment
        for (var startY = 0; startY < 9; startY += 3)
        {
            for (var startX = 0; startX < 9; startX += 3)
            {
                valuesInSection.Clear();
                for (var y = startY; y < startY + 3; y++)
                {
                    for (var x = startX; x < startX + 3; x++)
                    {
                        var tile = Grid[x, y];
                        if (!tile.Value.HasValue) return false;

                        var value = tile.Value.Value;
                        if (valuesInSection.Contains(value)) return false;
                        valuesInSection.Add(value);
                    }
                }
            }
        }

        // If we found no issues the sudoku is solved correctly
        return true;
    }

    public bool Solve()
    {
        // First propagate effects of all pre-set tiles
        foreach (var tile in Grid.Flatten().Where(x => x.Value.HasValue))
        {
            var result = TryPropegateEffectsToNeighbours(tile, tile.Value!.Value, null, false);
            if (!result) return false;
        }

        // Then iterate recursively, performing the main algorithm
        return IterateRecursively();
    }

    private bool IterateRecursively()
    {
        var affectedTiles = new Stack<Tile>();

        // Choose the tile with the lowest entropy to assign a value to
        var tile = Grid.Flatten().Where(x => !x.Value.HasValue).MinBy(x => x.PossibleValues.Count);

        if (tile is null)
        {
            // All tiles have a value, the sudoku is complete
            // Return true if completed correctly, false otherwise
            return IsSolved();
        }

        // Try to assign each of the tile's possible values until one works or we exhaust all options
        foreach (var possibleValue in tile.PossibleValues)
        {
            var immediateResult = TrySetValue(tile, possibleValue, affectedTiles);
            if (immediateResult)
            {
                // This assignment works for now
                var longTermResult = IterateRecursively();
                if (longTermResult)
                {
                    // This assignment did not cause any problems down the line
                    return true;
                }
            }

            // This assigment was immediately invalid or caused a problem down the line
            // Revert the assignment and proceed with the next possible
            while (affectedTiles.Count > 0)
            {
                var affectedTile = affectedTiles.Pop();
                affectedTile.RevertLastModification();
            }
        }

        // The tile cannot be assigned a valid value, the current sudoku is unsolvable
        // Previous assignments will be reverted and other values will be tried
        // (any assignments made in this iteration have already been reverted)
        return false;
    }

    private bool TrySetValue(Tile tile, int value, Stack<Tile>? affectedTiles)
    {
        tile.TrySetValue(value);
        affectedTiles?.Push(tile);
        return TryPropegateEffectsToNeighbours(tile, value, affectedTiles);
    }

    private bool TryPropegateEffectsToNeighbours(Tile tile, int value, Stack<Tile>? affectedTiles, bool setDeterminedValues = true)
    {
        var failed = false;

        // Try to disallow tiles in the same row from being the assigned value
        for (var x = 0; x < 9 && !failed; x++)
        {
            var otherTile = Grid[x, tile.Y];
            if (otherTile == tile) continue;

            var result = TryDisallowValue(otherTile, value, affectedTiles, setDeterminedValues);
            if (!result) failed = true;
        }

        // Try to disallow tiles in the same column from being the assigned value
        for (var y = 0; y < 9 && !failed; y++)
        {
            var otherTile = Grid[tile.X, y];
            if (otherTile == tile) continue;

            var result = TryDisallowValue(otherTile, value, affectedTiles, setDeterminedValues);
            if (!result) failed = true;
        }

        var segmentX = (int)Math.Floor(tile.X / 3f) * 3;
        var segmentY = (int)Math.Floor(tile.Y / 3f) * 3;

        // Try to disallow tiles in the same 3x3 segment from being the assigned value
        for (var x = segmentX; x < segmentX + 3 && !failed; x++)
        {
            for (var y = segmentY; y < segmentY + 3 && !failed; y++)
            {
                var otherTile = Grid[x, y];
                if (otherTile == tile) continue;

                var result = TryDisallowValue(otherTile, value, affectedTiles, setDeterminedValues);
                if (!result) failed = true;
            }
        }

        // If any tile had a problem with us disallowing this value, the assignment was invalid
        return !failed;
    }

    private bool TryDisallowValue(Tile tile, int value, Stack<Tile>? affectedTiles, bool setDeterminedValues = true)
    {
        var result = tile.TryDisallowValue(value);
        switch (result)
        {
            case DisallowValueResult.Success:
                // The tile has another possible value so this was okay
                affectedTiles?.Push(tile);
                return true;

            case DisallowValueResult.Failure:
                // This was the tile's only possible value so this assignment would make the sudoku unsolvable
                // No need to push this tile to affectedTiles as this action had no affect on the tile
                return false;

            case DisallowValueResult.DeterminedValue:
                // The tile only has one other possible value so can now be set
                // This still counts as the same "assignment" so pass the same affectedTiles stack
                // Return the result of this mini-assignment, if it causes issues then the original assignment would be invalid anyway
                affectedTiles?.Push(tile);
                return !setDeterminedValues || TrySetValue(tile, tile.PossibleValues[0], affectedTiles);

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
