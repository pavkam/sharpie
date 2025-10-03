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

namespace Sharpie.Backend;

/// <summary>
///     PDCursesMod-specific backend implementation.
/// </summary>
[PublicAPI]
internal class PdCursesMod32Backend: PdCursesBackend
{
    private const int _keyOffset = 0xeb00;

    /// <summary>
    ///     Creates a new instance of this class.
    /// </summary>
    /// <param name="dotNetSystemAdapter">The .NET system adapter.</param>
    /// <param name="pdCursesSymbolResolver">The PDCursesMod library symbol resolver.</param>
    /// <param name="libCSymbolResolver">The LibC symbol resolver.</param>
    internal PdCursesMod32Backend(IDotNetSystemAdapter dotNetSystemAdapter,
        INativeSymbolResolver pdCursesSymbolResolver, INativeSymbolResolver? libCSymbolResolver) : base(
        dotNetSystemAdapter, pdCursesSymbolResolver, libCSymbolResolver)
    {
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeKeyCodeType" />
    protected internal override CursesKeyCodeType DecodeKeyCodeType(int result, uint keyCode)
    {
#pragma warning disable IDE0072 // Add missing cases -- all cases are covered
        return (result, keyCode) switch
        {
            ( < 0, var _) => CursesKeyCodeType.Unknown,
            ((int) PdCursesKeyCode.Yes + _keyOffset, (uint) PdCursesKeyCode.Resize + _keyOffset) => CursesKeyCodeType
                .Resize,
            ((int) PdCursesKeyCode.Yes + _keyOffset, (uint) PdCursesKeyCode.Mouse + _keyOffset) =>
                CursesKeyCodeType.Mouse,
            ((int) PdCursesKeyCode.Yes + _keyOffset, var _) => CursesKeyCodeType.Key,
            ( >= 0, var _) => CursesKeyCodeType.Character,
        };
#pragma warning restore IDE0072 // Add missing cases
    }

    /// <inheritdoc cref="BaseCursesBackend.DecodeRawKey" />
    protected internal override (Key key, char @char, ModifierKey modifierKey) DecodeRawKey(uint keyCode) =>
        base.DecodeRawKey(keyCode - _keyOffset);

    // ReSharper disable IdentifierTypo
    // ReSharper disable InconsistentNaming

    public override int slk_attr_off(VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_attr_off>()(EncodeCursesAttribute(attributes, 0),
            reserved);

    public override int slk_attr_on(VideoAttribute attributes, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_attr_on>()(EncodeCursesAttribute(attributes, 0),
            reserved);

    public override int slk_attr(out VideoAttribute attributes, out short colorPair)
    {
        var ret = CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_attr>()();
        if (ret.Failed())
        {
            attributes = VideoAttribute.None;
            colorPair = 0;

            return ret;
        }

        (attributes, colorPair) = DecodeCursesAttributes((uint) ret);
        return 0;
    }

    public override int slk_attr_set(VideoAttribute attributes, short colorPair, IntPtr reserved) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_attr_set>()(EncodeCursesAttribute(attributes, 0),
            colorPair, reserved);

    public override int slk_clear() => CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_clear>()();

    public override int slk_color(short colorPair) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_color>()(colorPair);

    public override int slk_set(int labelIndex, string title, int align) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_set>()(labelIndex, title, align);

    public override int slk_init(int format) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_init>()(format);

    public override int slk_noutrefresh() => CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_noutrefresh>()();

    public override int slk_refresh() => CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_refresh>()();

    public override int slk_restore() => CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_restore>()();

    public override int slk_touch() => CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.slk_touch>()();

    public override int endwin() => CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.endwin_w32_4400>()();

    public override int getmouse(out CursesMouseState state) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.nc_getmouse>()(out state);

    public override bool is_immedok(IntPtr window) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.is_immedok>()(window);

    public override bool is_scrollok(IntPtr window) =>
        CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.is_scrollok>()(window);

    public override int getcchar(ComplexChar @char, StringBuilder dest, out VideoAttribute attributes,
        out short colorPair, IntPtr reserved)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        var ret = CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.getcchar>()(ref c[0], dest, out var attrs,
            out colorPair, reserved);

        (attributes, _) = DecodeCursesAttributes(attrs);
        return ret;
    }

    public override int setcchar(out ComplexChar @char, string text, VideoAttribute attributes, short colorPair,
        IntPtr reserved)
    {
        var c = new uint[1];

        var ret = CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.setcchar>()(out c[0], text,
            EncodeCursesAttribute(attributes, 0), colorPair, reserved);

        @char = new(c[0]);
        return ret;
    }

    public override int wadd_wch(IntPtr window, ComplexChar @char)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.wadd_wch>()(window, ref c[0]);
    }

    public override int wbkgrnd(IntPtr window, ComplexChar @char)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.wbkgrnd>()(window, ref c[0]);
    }

    public override int wborder_set(IntPtr window, ComplexChar leftSide, ComplexChar rightSide, ComplexChar topSide,
        ComplexChar bottomSide, ComplexChar topLeftCorner, ComplexChar topRightCorner, ComplexChar bottomLeftCorner,
        ComplexChar bottomRightCorner)
    {
        var c = new[]
        {
            leftSide.GetRawValue<uint>(),
            rightSide.GetRawValue<uint>(),
            topSide.GetRawValue<uint>(),
            bottomSide.GetRawValue<uint>(),
            topLeftCorner.GetRawValue<uint>(),
            topRightCorner.GetRawValue<uint>(),
            bottomLeftCorner.GetRawValue<uint>(),
            bottomRightCorner.GetRawValue<uint>()
        };

        return CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.wborder_set>()(window, ref c[0], ref c[1],
            ref c[2], ref c[3], ref c[4], ref c[5], ref c[6],
            ref c[7]);
    }

    public override int wgetbkgrnd(IntPtr window, out ComplexChar @char)
    {
        var c = new uint[1];

        var ret = CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.wgetbkgrnd>()(window, out c[0]);
        @char = new(c[0]);

        return ret;
    }

    public override int whline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.whline_set>()(window, ref c[0], count);
    }

    public override int win_wch(IntPtr window, out ComplexChar @char)
    {
        var c = new uint[1];

        var ret = CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.win_wch>()(window, out c[0]);
        @char = new(c[0]);
        return ret;
    }

    public override int wvline_set(IntPtr window, ComplexChar @char, int count)
    {
        var c = new[] { @char.GetRawValue<uint>() };

        return CursesSymbolResolver.Resolve<PdCursesMod32FunctionMap.wvline_set>()(window, ref c[0], count);
    }

    // ReSharper restore InconsistentNaming
    // ReSharper restore IdentifierTypo
}
