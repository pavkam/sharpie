namespace Sharpie.Demos.Snake;

using System.Drawing;
using Abstractions;

public sealed class Game
{
    private readonly Style _snakeHeadStyle;
    private readonly Style _snakeBodyStyle;
    private readonly Style _foodStyle;
    private readonly Style _lostStyle;

    public enum Direction
    {
        /// <summary>
        /// Up.
        /// </summary>
        Up = 1,

        /// <summary>
        /// Right.
        /// </summary>
        Right,

        /// <summary>
        /// Down.
        /// </summary>
        Down,

        /// <summary>
        /// Left.
        /// </summary>
        Left
    }

    private readonly Drawing _glyph = new(new(1,1));
    private int _pendingGrowth;
    private Direction _direction = 0;
    private readonly List<Point> _snake = new();
    private readonly List<Point> _clear = new();
    private (Point, int) _food;
    private Rectangle _perimeter;
    private bool _lost;
    private readonly Random _random = new();

    public Game(Style snakeHeadStyle, Style snakeBodyStyle, Style foodStyle, Style lostStyle)
    {
        _snakeHeadStyle = snakeHeadStyle;
        _snakeBodyStyle = snakeBodyStyle;
        _foodStyle = foodStyle;
        _lostStyle = lostStyle;
    }
    
    public void ResetSize(Rectangle perimeter)
    {
        _perimeter = perimeter;
        var ng = _pendingGrowth;
        if (_snake.Count > 0)
        {
            ng += _snake.Count - 1;
            _snake.Clear();
        }

        _food = (Point.Empty, 0);
        _direction = 0;
        _pendingGrowth = ng;
        _snake.Add(new(perimeter.Width / 2, perimeter.Height / 2));
    }

    public bool Tick()
    {
        if (_lost)
        {
            return false;
        }
        
        if (_snake.Count == 0 || _direction == 0)
        {
            return true;
        }

        if (_food.Item2 == 0)
        {
            var fc = new Point(_random.Next(_perimeter.Left, _perimeter.Right), 
                _random.Next(_perimeter.Top, _perimeter.Bottom));

            if (!_snake.Contains(fc))
            {
                _food = (fc, _random.Next(1, 5));
            }
        }
        
        var head = _snake[0];
        var newHead = _direction switch
        {
            Direction.Down => head with { Y = head.Y + 1 },
            Direction.Up => head with { Y = head.Y - 1 },
            Direction.Right => head with { X = head.X + 1 },
            Direction.Left => head with { X = head.X - 1 },
            var _ => head
        };
        
        if (!_perimeter.Contains(newHead) || _snake.Contains(newHead))
        {
            _lost = true;
            return true;
        }

        if (_food.Item1 == newHead)
        {
            _pendingGrowth += _food.Item2;
            Score += _food.Item2;
            _food = (Point.Empty, 0);
        }
        
        if (_pendingGrowth == 0)
        {
            _clear.Add(_snake.Last());
            _snake.RemoveAt(_snake.Count - 1);
        } else
        {
            _pendingGrowth--;
        }

        _snake.Insert(0, newHead);

        return true;
    }

    public void Turn(Direction direction)
    {
        if (!_lost)
        {
            _direction = direction;
        }
    }

    public void Update(ISurface surface)
    {
        if (_lost)
        {
            surface.Clear();
            surface.Background = (new(' '), _lostStyle);
        }
        
        if (_clear.Count > 0)
        {
            foreach (var p in _clear)
            {
                _glyph.Glyph(Point.Empty, new(' '), Style.Default);
                surface.Draw(p, _glyph);
            }

            _clear.Clear();
        }
        
        if (_food.Item2 != 0)
        {
            _glyph.Glyph(Point.Empty, new('0' + _food.Item2), _foodStyle);
            surface.Draw(_food.Item1, _glyph);
        }

        if (_direction == 0)
        {
            if (_snake.Count > 0)
            {
                _glyph.Glyph(Point.Empty, Drawing.CheckGlyphStyle.Diamond, Drawing.FillStyle.Black, _snakeHeadStyle);
                surface.Draw(_snake[0], _glyph);
            }

            return;
        }
        
        var headStyle = _direction switch
        {
            Direction.Down => Drawing.TriangleGlyphStyle.Down,
            Direction.Up => Drawing.TriangleGlyphStyle.Up,
            Direction.Left => Drawing.TriangleGlyphStyle.Left,
            Direction.Right => Drawing.TriangleGlyphStyle.Right,
            var _ => (Drawing.TriangleGlyphStyle) 0
        };

        _glyph.Glyph(Point.Empty, headStyle, Drawing.GlyphSize.Normal,
            Drawing.FillStyle.Black, _snakeHeadStyle);
        surface.Draw(_snake[0], _glyph);
        
        for (var i = 1; i < _snake.Count; i++)
        {
            _glyph.Glyph(Point.Empty, Drawing.CheckGlyphStyle.Square, Drawing.FillStyle.Black, _snakeBodyStyle);
            surface.Draw(_snake[i], _glyph);
        }
    }
    
    public int Score { get; private set; }
}
