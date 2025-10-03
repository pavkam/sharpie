/*
Copyright (c) 2022-2025, Alexandru Ciobanu
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this
   list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice,
   this list of conditions and the following disclaimer in the documentation
   and/or other materials provided with the distribution.

3. Neither the name of the copyright holder nor the names of its
   contributors may be used to endorse or promote products derived from
   this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

namespace Sharpie.Demos.Snake;

using System.Drawing;

using Abstractions;

public sealed class Game(Style snakeHeadStyle, Style snakeBodyStyle, Style foodStyle, Style lostStyle)
{
    public enum Direction
    {
        /// <summary>
        ///     Up.
        /// </summary>
        Up = 1,

        /// <summary>
        ///     Right.
        /// </summary>
        Right,

        /// <summary>
        ///     Down.
        /// </summary>
        Down,

        /// <summary>
        ///     Left.
        /// </summary>
        Left
    }

    private readonly List<Point> _clear = [];
    private readonly Style _foodStyle = foodStyle;

    private readonly Canvas _glyph = new(new(1, 1));
    private readonly Style _lostStyle = lostStyle;
    private readonly Random _random = new();
    private readonly List<Point> _snake = [];
    private readonly Style _snakeBodyStyle = snakeBodyStyle;
    private readonly Style _snakeHeadStyle = snakeHeadStyle;
    private Direction _direction = 0;
    private (Point, int) _food;
    private bool _lost;
    private int _pendingGrowth;
    private Rectangle _perimeter;

    public int Score
    {
        get; private set;
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
        }
        else
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
                _glyph.Glyph(Point.Empty, Canvas.CheckGlyphStyle.Diamond, Canvas.FillStyle.Black, _snakeHeadStyle);
                surface.Draw(_snake[0], _glyph);
            }

            return;
        }

        var headStyle = _direction switch
        {
            Direction.Down => Canvas.TriangleGlyphStyle.Down,
            Direction.Up => Canvas.TriangleGlyphStyle.Up,
            Direction.Left => Canvas.TriangleGlyphStyle.Left,
            Direction.Right => Canvas.TriangleGlyphStyle.Right,
            var _ => (Canvas.TriangleGlyphStyle) 0
        };

        _glyph.Glyph(Point.Empty, headStyle, Canvas.GlyphSize.Normal, Canvas.FillStyle.Black, _snakeHeadStyle);
        surface.Draw(_snake[0], _glyph);

        for (var i = 1; i < _snake.Count; i++)
        {
            _glyph.Glyph(Point.Empty, Canvas.CheckGlyphStyle.Square, Canvas.FillStyle.Black, _snakeBodyStyle);
            surface.Draw(_snake[i], _glyph);
        }
    }
}
