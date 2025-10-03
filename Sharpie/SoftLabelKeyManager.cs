/*
Copyright (c) 2022-2023, Alexandru Ciobanu
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
    private readonly SoftLabelKeyMode _mode;

    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="terminal">The parent terminal.</param>
    /// <param name="mode">The mode of the manager.</param>
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="terminal" /> is <c>null</c>.</exception>
    internal SoftLabelKeyManager(Terminal terminal, SoftLabelKeyMode mode)
    {
        Terminal = terminal ?? throw new ArgumentNullException(nameof(terminal));
        _mode = mode;

        if (mode != SoftLabelKeyMode.Disabled)
        {
            _ = terminal.Curses.slk_init((int) mode)
                    .Check(nameof(terminal.Curses.slk_init), "Failed to initialize soft label keys.");
        }
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Terminal" />
    public Terminal Terminal
    {
        get;
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Terminal" />
    ITerminal ISoftLabelKeyManager.Terminal => Terminal;

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
            AssertSynchronized();

            _ = Terminal.Curses.slk_attr(out var attributes, out var colorPair)
                    .Check(nameof(Terminal.Curses.slk_attr), "Failed to get the soft label key attributes.");

            return new()
            {
                Attributes = attributes,
                ColorMixture = new()
                {
                    Handle = colorPair
                }
            };
        }
        set
        {
            AssertEnabled();
            AssertSynchronized();

            _ = Terminal.Curses.slk_attr_set(value.Attributes, value.ColorMixture.Handle, IntPtr.Zero)
                    .Check(nameof(Terminal.Curses.slk_attr_set), "Failed to configure the soft label key attributes.");
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
            AssertSynchronized();

            _ = Terminal.Curses.slk_color(value.Handle)
                    .Check(nameof(Terminal.Curses.slk_color), "Failed to configure the soft label key colors.");
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
        AssertSynchronized();

        _ = Terminal.Curses.slk_set(index + 1, title, (int) align)
                .Check(nameof(Terminal.Curses.slk_set), "Failed to set the soft label.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.EnableAttributes" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void EnableAttributes(VideoAttribute attributes)
    {
        AssertEnabled();
        AssertSynchronized();

        _ = Terminal.Curses.slk_attr_on(attributes, IntPtr.Zero)
                .Check(nameof(Terminal.Curses.slk_attr_on), "Failed to configure the soft label key attributes.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.DisableAttributes" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void DisableAttributes(VideoAttribute attributes)
    {
        AssertEnabled();
        AssertSynchronized();

        _ = Terminal.Curses.slk_attr_off(attributes, IntPtr.Zero)
                .Check(nameof(Terminal.Curses.slk_attr_off), "Failed to configure the soft label key attributes.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Clear" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Clear()
    {
        AssertEnabled();
        AssertSynchronized();

        _ = Terminal.Curses.slk_clear()
                .Check(nameof(Terminal.Curses.slk_clear), "Failed to clear the soft label keys.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Restore" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Restore()
    {
        AssertEnabled();
        AssertSynchronized();

        _ = Terminal.Curses.slk_restore()
                .Check(nameof(Terminal.Curses.slk_restore), "Failed to restore the soft label keys.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.MarkDirty" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void MarkDirty()
    {
        AssertEnabled();
        AssertSynchronized();

        _ = Terminal.Curses.slk_touch()
                .Check(nameof(Terminal.Curses.slk_touch), "Failed to mark soft label keys as dirty.");
    }

    /// <inheritdoc cref="ISoftLabelKeyManager.Refresh" />
    /// <exception cref="CursesOperationException">A Curses error occured.</exception>
    public void Refresh()
    {
        AssertEnabled();
        AssertSynchronized();

        _ = Terminal.AtomicRefreshOpen
            ? Terminal.Curses.slk_noutrefresh()
                    .Check(nameof(Terminal.Curses.slk_noutrefresh), "Failed to queue soft label key refresh.")
            : Terminal.Curses.slk_refresh()
                    .Check(nameof(Terminal.Curses.slk_refresh), "Failed to perform soft label key refresh.");
    }

    private void AssertSynchronized() => Terminal.AssertSynchronized();

    private void AssertEnabled()
    {
        if (_mode == SoftLabelKeyMode.Disabled)
        {
            throw new NotSupportedException("The soft key labels were not configured during terminal initialization.");
        }
    }
}
