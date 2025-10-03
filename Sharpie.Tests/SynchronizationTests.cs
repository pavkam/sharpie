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

namespace Sharpie.Tests;

[TestClass]
public class SynchronizationTests
{
    private Mock<ICursesBackend> _cursesMock = null!;
    private Pad _pad = null!;
    private SubPad _subPad = null!;
    private SubWindow _subWindow = null!;
    private Surface _surface = null!;
    private Terminal _terminal = null!;
    private TerminalSurface _terminalSurface = null!;
    private Window _window = null!;

    [UsedImplicitly] public TestContext TestContext { get; set; } = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _cursesMock = new();
        _ = _cursesMock.Setup(s => s.initscr())
                   .Returns(new IntPtr(1));

        _ = _cursesMock.Setup(s => s.has_colors())
                   .Returns(true);

        _terminal = new(_cursesMock.Object,
            new(SoftLabelKeyMode: SoftLabelKeyMode.FourFour,
                ManagedWindows: TestContext.TestName!.Contains("_WhenManaged_")));

        _surface = new(_terminal, new(2));
        _pad = new(_terminal.Screen, new(3));
        _window = new(_terminal.Screen, new(4));
        _subPad = new(_pad, new(5));
        _subWindow = new(_window, new(6));
        _terminalSurface = new(_terminal, new(7));

        _cursesMock.MockArea(_surface, new Size(2, 2));
        _ = _cursesMock.Setup(s => s.newpad(1, 1))
                   .Returns(new IntPtr(100));

        _ = _cursesMock.Setup(s => s.has_colors())
                   .Returns(true);

        _ = _cursesMock.Setup(s => s.can_change_color())
                   .Returns(true);

        _ = _cursesMock.Setup(s => s.dupwin(It.IsAny<IntPtr>()))
                   .Returns(new IntPtr(101));
    }

    [TestCleanup]
    public void TestCleanup() => _terminal.Dispose();

    private void Check(Action action)
    {
        var startedEvent = new ManualResetEvent(false);
        _ = Task.Run(() => _terminal.Run((_, _) =>
        {
            _ = startedEvent.Set();
            return Task.CompletedTask;
        }));

        _ = startedEvent.WaitOne();

        _ = Should.Throw<CursesSynchronizationException>(action);

        _terminal.Stop(true);
    }

    [TestMethod]
    public void ColorManager_MixColors1() =>
        Check(() => _terminal.Colors.MixColors(1, 1));

    [TestMethod]
    public void ColorManager_MixColors2() =>
        Check(() => _terminal.Colors.MixColors(StandardColor.Black, StandardColor.Green));

    [TestMethod]
    public void ColorManager_RemixColors1() =>
        Check(() => _terminal.Colors.RemixColors(new() { Handle = 100 }, 1, 1));

    [TestMethod]
    public void ColorManager_RemixColors2() =>
        Check(() => _terminal.Colors.RemixColors(new() { Handle = 100 }, StandardColor.Black, StandardColor.Black));

    [TestMethod]
    public void ColorManager_RemixDefaultColors1() =>
        Check(() => _terminal.Colors.RemixDefaultColors(1, 1));

    [TestMethod]
    public void ColorManager_RemixDefaultColors2() =>
        Check(() => _terminal.Colors.RemixDefaultColors(StandardColor.Black, StandardColor.Black));

    [TestMethod]
    public void ColorManager_UnMixColors1() =>
        Check(() => _terminal.Colors.UnMixColors(new() { Handle = 100 }));

    [TestMethod]
    public void ColorManager_RedefineColor1() =>
        Check(() => _terminal.Colors.RedefineColor(1, 2, 3, 4));

    [TestMethod]
    public void ColorManager_RedefineColor2() =>
        Check(() => _terminal.Colors.RedefineColor(StandardColor.Blue, 2, 3, 4));

    [TestMethod]
    public void ColorManager_BreakdownColor1() =>
        Check(() => _terminal.Colors.BreakdownColor(1));

    [TestMethod]
    public void ColorManager_BreakdownColor2() =>
        Check(() => _terminal.Colors.BreakdownColor(StandardColor.Black));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Events_Listen1()
    {
        Check(() => _terminal.Events.Listen(_surface, CancellationToken.None)
                             .ToArray());
    }

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Events_Listen2()
    {
        Check(() => _terminal.Events.Listen(_surface)
                             .ToArray());
    }

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void EventPump_Listen3()
    {
        Check(() => _terminal.Events.Listen(CancellationToken.None)
                             .ToArray());
    }

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void EventPump_Listen4()
    {
        Check(() => _terminal.Events.Listen()
                             .ToArray());
    }

    [TestMethod]
    public void EventPump_Use() => Check(() => _terminal.Events.Use(_ => (null, 0)));

    [TestMethod]
    public void EventPump_Uses() => Check(() => _terminal.Events.Uses(_ => (null, 0)));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Pad_SubPads() => Check(() => _pad.SubPads.ToArray());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Pad_Size_Get() => Check(() => _pad.Size.ToString());

    [TestMethod]
    public void Pad_Size_Set() => Check(() => _pad.Size = new(1, 1));

    [TestMethod]
    public void Pad_Refresh1() => Check(() => _pad.Refresh(new(0, 0, 1, 1), Point.Empty));

    [TestMethod]
    public void Pad_Refresh2() => Check(() => _pad.Refresh(Point.Empty));

    [TestMethod]
    public void Pad_SubPad() => Check(() => _pad.SubPad(new(0, 0, 1, 1)));

    [TestMethod]
    public void Pad_Duplicate() => Check(() => _pad.Duplicate());

    [TestMethod]
    public void Pad_Destroy() => Check(() => _pad.Destroy());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Screen_Windows() => Check(() => _terminal.Screen.Windows.ToArray());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Screen_Pads() => Check(() => _terminal.Screen.Pads.ToArray());

    [TestMethod]
    public void Screen_Window() => Check(() => _terminal.Screen.Window(new(0, 0, 1, 1)));

    [TestMethod]
    public void Screen_Pad() => Check(() => _terminal.Screen.Pad(new(1, 1)));

    [TestMethod]
    public void Screen_Destroy() => Check(() => _terminal.Screen.Destroy());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void SoftLabelKeyManager_Style_Get() =>
        Check(() => _terminal.SoftLabelKeys.Style.ToString());

    [TestMethod]
    public void SoftLabelKeyManager_Style_Set() =>
        Check(() => _terminal.SoftLabelKeys.Style = Style.Default);

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void SoftLabelKeyManager_ColorMixture_Get() =>
        Check(() => _terminal.SoftLabelKeys.ColorMixture.ToString());

    [TestMethod]
    public void SoftLabelKeyManager_ColorMixture_Set() =>
        Check(() => _terminal.SoftLabelKeys.ColorMixture = ColorMixture.Default);

    [TestMethod]
    public void SoftLabelKeyManager_SetLabel() =>
        Check(() => _terminal.SoftLabelKeys.SetLabel(0, "title", SoftLabelKeyAlignment.Left));

    [TestMethod]
    public void SoftLabelKeyManager_EnableAttributes() =>
         Check(() => _terminal.SoftLabelKeys.EnableAttributes(VideoAttribute.Blink));

    [TestMethod]
    public void SoftLabelKeyManager_DisableAttributes() =>
        Check(() => _terminal.SoftLabelKeys.DisableAttributes(VideoAttribute.Blink));

    [TestMethod]
    public void SoftLabelKeyManager_Clear() =>
        Check(() => _terminal.SoftLabelKeys.Clear());

    [TestMethod]
    public void SoftLabelKeyManager_Restore() =>
        Check(() => _terminal.SoftLabelKeys.Restore());

    [TestMethod]
    public void SoftLabelKeyManager_MarkDirty() =>
        Check(() => _terminal.SoftLabelKeys.MarkDirty());

    [TestMethod]
    public void SoftLabelKeyManager_Refresh() =>
        Check(() => _terminal.SoftLabelKeys.Refresh());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void SubPad_Location_Get() => Check(() => _subPad.Location.ToString());

    [TestMethod]
    public void SubPad_Location_Set() => Check(() => _subPad.Location = new(1, 1));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void SubPad_Size_Get() => Check(() => _subPad.Size.ToString());

    [TestMethod]
    public void SubPad_Size_Set() => Check(() => _subPad.Size = new(1, 1));

    [TestMethod]
    public void SubPad_Duplicate() => Check(() => _subPad.Duplicate());

    [TestMethod]
    public void SubPad_Destroy() => Check(() => _subPad.Destroy());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void SubWindow_Location_Get() => Check(() => _subWindow.Location.ToString());

    [TestMethod]
    public void SubWindow_Location_Set() => Check(() => _subWindow.Location = new(1, 1));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void SubWindow_Size_Get() => Check(() => _subWindow.Size.ToString());

    [TestMethod]
    public void SubWindow_Size_Set() => Check(() => _subWindow.Size = new(1, 1));

    [TestMethod]
    public void SubWindow_Duplicate() => Check(() => _subWindow.Duplicate());

    [TestMethod]
    public void SubWindow_Destroy() => Check(() => _subWindow.Destroy());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Window_SubWindows() => Check(() => _window.SubWindows.ToArray());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Window_Location_Get() => Check(() => _window.Location.ToString());

    [TestMethod]
    public void Window_Location_Set() => Check(() => _window.Location = new(1, 1));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Window_Size_Get() => Check(() => _window.Size.ToString());

    [TestMethod]
    public void Window_Size_Set() => Check(() => _window.Size = new(1, 1));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Window_WhenManaged_Visible_Get() => Check(() => _window.Visible.ToString());

    [TestMethod]
    public void Window_WhenManaged_Visible_Set() => Check(() => _window.Visible = true);

    [TestMethod]
    public void Window_WhenManaged_SendToBack() => Check(() => _window.SendToBack());

    [TestMethod]
    public void Window_WhenManaged_BringToFront() => Check(() => _window.BringToFront());

    [TestMethod]
    public void Window_AdjustToExplicitArea() => Check(() => _window.AdjustToExplicitArea());

    [TestMethod]
    public void Window_SubWindow() => Check(() => _window.SubWindow(new(0, 0, 1, 1)));

    [TestMethod]
    public void Window_Duplicate() => Check(() => _window.Duplicate());

    [TestMethod]
    public void Window_Destroy() => Check(() => _window.Destroy());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void TerminalSurface_ImmediateRefresh_Get() =>
        Check(() => _terminalSurface.ImmediateRefresh.ToString());

    [TestMethod]
    public void TerminalSurface_ImmediateRefresh_Set() =>
        Check(() => _terminalSurface.ImmediateRefresh = true);

    [TestMethod]
    public void TerminalSurface_Refresh1() =>
        Check(() => _terminalSurface.Refresh());

    [TestMethod]
    public void TerminalSurface_Refresh2() =>
        Check(() => _terminalSurface.Refresh(0, 1));

    [TestMethod]
    public void TerminalSurface_Destroy() =>
        Check(() => _terminalSurface.Destroy());

    [TestMethod]
    public void Surface_DrawCell() =>
        Check(() => ((IDrawSurface) _surface).DrawCell(new(0, 0), new('A'), Style.Default));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_Scrollable_Get() => Check(() => _surface.Scrollable.ToString());

    [TestMethod]
    public void Surface_Scrollable_Set() => Check(() => _surface.Scrollable = true);

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_Size_Get() => Check(() => _surface.Size.ToString());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_Style_Get() => Check(() => _surface.Style.ToString());

    [TestMethod]
    public void Surface_Style_Set() => Check(() => _surface.Style = Style.Default);

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_ColorMixture_Get() => Check(() => _surface.ColorMixture.ToString());

    [TestMethod]
    public void Surface_ColorMixture_Set() => Check(() => _surface.ColorMixture = ColorMixture.Default);

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_Background_Get() => Check(() => _surface.Background.ToString());

    [TestMethod]
    public void Surface_Background_Set() => Check(() => _surface.Background = (new('A'), Style.Default));

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_CaretLocation_Get() => Check(() => _surface.CaretLocation.ToString());

    [TestMethod]
    public void Surface_CaretLocation_Set() => Check(() => _surface.CaretLocation = Point.Empty);

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_Dirty_Get() => Check(() => _surface.Dirty.ToString());

    [TestMethod, SuppressMessage("ReSharper", "ReturnValueOfPureMethodIsNotUsed")]
    public void Surface_ManagedCaret_Get() => Check(() => _surface.ManagedCaret.ToString());

    [TestMethod]
    public void Surface_ManagedCaret_Set() => Check(() => _surface.ManagedCaret = true);

    [TestMethod]
    public void Surface_EnableAttributes() =>
        Check(() => _surface.EnableAttributes(VideoAttribute.Blink));

    [TestMethod]
    public void Surface_DisableAttributes() =>
        Check(() => _surface.DisableAttributes(VideoAttribute.Blink));

    [TestMethod]
    public void Surface_ScrollUp() => Check(() => _surface.ScrollUp(1));

    [TestMethod]
    public void Surface_ScrollDown() => Check(() => _surface.ScrollDown(1));

    [TestMethod]
    public void Surface_InsertEmptyLines() => Check(() => _surface.InsertEmptyLines(1));

    [TestMethod]
    public void Surface_DeleteLines() => Check(() => _surface.DeleteLines(1));

    [TestMethod]
    public void Surface_ChangeTextStyle() => Check(() => _surface.ChangeTextStyle(1, Style.Default));

    [TestMethod]
    public void Surface_WriteText1() => Check(() => _surface.WriteText("text", Style.Default));

    [TestMethod]
    public void Surface_WriteText2() => Check(() => _surface.WriteText("text"));

    [TestMethod]
    public void Surface_WriteText3() =>
        Check(() => _surface.WriteText(new StyledText("hello", Style.Default)));

    [TestMethod]
    public void Surface_DrawVerticalLine1() =>
        Check(() => _surface.DrawVerticalLine(1, new('A'), Style.Default));

    [TestMethod]
    public void Surface_DrawVerticalLine2() =>
        Check(() => _surface.DrawVerticalLine(1));

    [TestMethod]
    public void Surface_DrawHorizontalLine1() =>
        Check(() => _surface.DrawHorizontalLine(1, new('A'), Style.Default));

    [TestMethod]
    public void Surface_DrawHorizontalLine2() => Check(() => _surface.DrawHorizontalLine(1));

    [TestMethod]
    public void Surface_DrawBorder1()
    {
        Check(() => _surface.DrawBorder(new('.'), new('.'), new('.'), new('.'), new('.'),
            new('.'), new('.'), new('.'), Style.Default));
    }

    [TestMethod]
    public void Surface_DrawBorder2() => Check(() => _surface.DrawBorder());

    [TestMethod]
    public void Surface_RemoveText() => Check(() => _surface.RemoveText(1));

    [TestMethod]
    public void Surface_GetText() => Check(() => _surface.GetText(1));

    [TestMethod]
    public void Surface_Clear() => Check(() => _surface.Clear());

    [TestMethod]
    public void Surface_Replace1() =>
        Check(() => _surface.Replace(_terminalSurface, ReplaceStrategy.Overlay));

    [TestMethod]
    public void Surface_Replace2() =>
        Check(() => _surface.Replace(_terminalSurface, new(0, 0, 1, 1), Point.Empty, ReplaceStrategy.Overlay));

    [TestMethod]
    public void Surface_MarkDirty1() =>
        Check(() => _surface.MarkDirty(0, 1));

    [TestMethod]
    public void Surface_MarkDirty2() =>
        Check(() => _surface.MarkDirty());

    [TestMethod]
    public void Surface_IsPointWithin() =>
        Check(() => _surface.IsPointWithin(Point.Empty));

    [TestMethod]
    public void Surface_IsRectangleWithin() =>
        Check(() => _surface.IsRectangleWithin(new(0, 0, 1, 1)));

    [TestMethod]
    public void Surface_Draw1() =>
        Check(() => _surface.Draw(Point.Empty, new(0, 0, 1, 1), new Canvas(new(10, 10))));

    [TestMethod]
    public void Surface_Draw2() =>
        Check(() => _surface.Draw(Point.Empty, new Canvas(new(10, 10))));

    [TestMethod]
    public void Surface_LineDirty() => Check(() => _surface.LineDirty(0));

    [TestMethod]
    public void Surface_Destroy() => Check(() => _surface.Destroy());

    [TestMethod]
    public void Terminal_SetTitle() => Check(() => _terminal.SetTitle("test"));

    [TestMethod]
    public void Terminal_Alert() => Check(() => _terminal.Alert(true));

    [TestMethod]
    public void Terminal_AtomicRefresh() => Check(() => _terminal.AtomicRefresh());

    [TestMethod]
    public void Terminal_TryUpdate() => Check(() => _terminal.TryUpdate());

    private sealed class Surface: Sharpie.Surface
    {
        private readonly Terminal _terminal;

        public Surface(Terminal terminal, IntPtr handle) : base(terminal.Curses, handle) => _terminal = terminal;

        protected internal override void AssertSynchronized() => _terminal.AssertSynchronized();
    }
}
