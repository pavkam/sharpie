/*
Copyright (c) 2022, Alexandru Ciobanu
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

namespace Sharpie;

/// <summary>
///     Adds support for soft function keys.
/// </summary>
[PublicAPI]
public sealed class SoftLabelKeyManager: ISoftLabelKeyManager
{
    private readonly ICursesProvider _curses;
    private readonly SoftLabelKeyMode _mode;

    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="curses">The curses backend.</param>
    /// <param name="mode">The mode of the manager.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="curses" /> is <c>null</c>.</exception>
    internal SoftLabelKeyManager(ICursesProvider curses, SoftLabelKeyMode mode)
    {
        _curses = curses ?? throw new ArgumentNullException(nameof(curses));
        _mode = mode;

        if (mode != SoftLabelKeyMode.Disabled)
        {
            _curses.slk_init((int) mode)
                   .Check(nameof(_curses.slk_init), "Failed to initialize soft label keys.");
        }
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Enabled" />
    public bool Enabled => _mode != SoftLabelKeyMode.Disabled;

    /// <inheritdoc cref="ISoftLabelKeyManager.LabelCount" />
    public int LabelCount
    {
        get
        {
            AssertEnabled();
            return _mode is SoftLabelKeyMode.FourFourFour or SoftLabelKeyMode.FourFourFourWithIndex ? 12 : 8;
        }
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Style" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public Style Style
    {
        get
        {
            AssertEnabled();

            var attrsAndColors = _curses.slk_attr()
                                        .Check(nameof(_curses.slk_attr),
                                            "Failed to get the soft label key attributes.");

            var colorPair = (short) _curses.COLOR_PAIR((uint) attrsAndColors);
            return new() { Attributes = (VideoAttribute) attrsAndColors, ColorMixture = new() { Handle = colorPair } };
        }
        set
        {
            AssertEnabled();

            _curses.slk_attr_set((uint) value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                   .Check(nameof(_curses.slk_attr_set), "Failed to configure the soft label key attributes.");
        }
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.ColorMixture" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public ColorMixture ColorMixture
    {
        get => Style.ColorMixture;
        set
        {
            AssertEnabled();

            _curses.slk_color(value.Handle)
                   .Check(nameof(_curses.slk_color), "Failed to configure the soft label key colors.");
        }
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.SetLabel" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void SetLabel(int index, string title, SoftLabelKeyAlignment align)
    {
        if (title == null)
        {
            throw new ArgumentNullException(nameof(title));
        }

        if (index < 0 || index >= LabelCount)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        AssertEnabled();

        _curses.slk_set(index + 1, title, (int) align)
               .Check(nameof(_curses.slk_set), "Failed to set the soft label.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.EnableAttributes" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        AssertEnabled();

        _curses.slk_attr_on((uint) attributes, IntPtr.Zero)
               .Check(nameof(_curses.slk_attr_on), "Failed to configure the soft label key attributes.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.DisableAttributes" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        AssertEnabled();

        _curses.slk_attr_off((uint) attributes, IntPtr.Zero)
               .Check(nameof(_curses.slk_attr_off), "Failed to configure the soft label key attributes.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Clear" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Clear()
    {
        AssertEnabled();
        _curses.slk_clear()
               .Check(nameof(_curses.slk_clear), "Failed to clear the soft label keys.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Restore" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Restore()
    {
        AssertEnabled();
        _curses.slk_restore()
               .Check(nameof(_curses.slk_restore), "Failed to restore the soft label keys.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Invalidate" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Invalidate()
    {
        AssertEnabled();
        _curses.slk_touch()
               .Check(nameof(_curses.slk_touch), "Failed to mark soft label keys as dirty.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Refresh" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Refresh(bool batch)
    {
        AssertEnabled();

        if (batch)
        {
            _curses.slk_noutrefresh()
                   .Check(nameof(_curses.slk_noutrefresh), "Failed to queue soft label key refresh.");
        } else
        {
            _curses.slk_refresh()
                   .Check(nameof(_curses.slk_refresh), "Failed to perform soft label key refresh.");
        }
    }

    private void AssertEnabled()
    {
        if (_mode == SoftLabelKeyMode.Disabled)
        {
            throw new NotSupportedException("The soft key labels were not configured during terminal initialization.");
        }
    }
}
