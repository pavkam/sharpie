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

namespace Sharpie.Tests;

[TestClass]
public class ScreenInterfaceTests
{
    [TestMethod]
    public void ForceInvalidateAndRefresh_CallsInADeepScrub()
    {
        var w1 = new Mock<IWindow>();
        var w2 = new Mock<IWindow>();
        var scr = new Mock<IScreen>();

        w1.Setup(s => s.Children)
          .Returns(new[] { w2.Object });
        scr.Setup(s => s.Children)
           .Returns(new[] { w1.Object });
        
        scr.Object.ForceInvalidateAndRefresh();
        
        scr.Verify(v => v.Invalidate(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        scr.Verify(v => v.Refresh(false, true), Times.Once);
        w1.Verify(v => v.Invalidate(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
        w2.Verify(v => v.Invalidate(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }
}
