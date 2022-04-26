using System.Collections.Immutable;

namespace Sudoku;

public class Tile
{
    public int X { get; }
    public int Y { get; }

    public int? Value { get; private set; }
    public IReadOnlyList<int> PossibleValues => _possibleValues.ToImmutableList();

    private List<int> _possibleValues;
    private Stack<TileModificationAction> _history;

    public Tile(int x, int y)
    {
        X = x;
        Y = y;
        _possibleValues = new List<int>();
        _history = new Stack<TileModificationAction>();
    }

    public bool TrySetValue(int value)
    {
        if (Value.HasValue) throw new InvalidOperationException("This tile already has a value");
        if (!_possibleValues.Contains(value)) return false;

        _history.Push(new TileModificationAction
        {
            Type = TileModificationActionType.SetValue,
            Value = value
        });
        Value = value;

        return true;
    }

    public DisallowValueResult TryDisallowValue(int value)
    {
        if (Value == value) return DisallowValueResult.Failure;

        if (!_possibleValues.Contains(value))
        {
            _history.Push(new TileModificationAction
            {
                Type = TileModificationActionType.NoAffect
            });
            return DisallowValueResult.Success;
        }

        if (_possibleValues.Count == 1) return DisallowValueResult.Failure;

        _possibleValues.Remove(value);
        _history.Push(new TileModificationAction
        {
            Type = TileModificationActionType.DisallowValue,
            Value = value
        });

        if (_possibleValues.Count == 1 && !Value.HasValue)
        {
            return DisallowValueResult.DeterminedValue;
        }

        return DisallowValueResult.Success;
    }

    public void RevertLastModification()
    {
        var action = _history.Pop();
        switch (action.Type)
        {
            case TileModificationActionType.DisallowValue:
                _possibleValues.Add(action.Value);
                break;

            case TileModificationActionType.SetValue:
                Value = null;
                break;

            case TileModificationActionType.NoAffect:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static Tile FromValue(int value, int x, int y)
    {
        var tile = new Tile(x, y)
        {
            Value = value
        };
        return tile;
    }

    public static Tile Empty(int x, int y)
    {
        var tile = new Tile(x, y)
        {
            _possibleValues = Enumerable.Range(1, 9).ToList()
        };
        return tile;
    }

    public override string ToString()
    {
        return Value.HasValue ? $" {Value.Value} " : "   ";
    }
}
