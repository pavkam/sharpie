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

namespace Sharpie.Abstractions;

/// <summary>
///     Describes the traits specific to the <see cref="SubPad" />.
/// </summary>
[PublicAPI]
public interface ISubPad: ISurface
{
    /// <summary>
    ///     The parent pad of this sub-pad.
    /// </summary>
    IPad Pad { get; }

    /// <summary>
    ///     Gets or sets the location of the sub-pad within its parent pad.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the parent's bounds.</exception>
    /// <exception cref="ObjectDisposedException">Sub-pad is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    Point Location { get; set; }

    /// <summary>
    ///     Gets or sets the size of the sub-pad.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException">The <paramref name="value" /> is outside the bounds.</exception>
    /// <exception cref="ObjectDisposedException">Sub-pad is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    new Size Size { get; set; }

    /// <summary>
    ///     Duplicates and existing sub-pad, including its attributes.
    /// </summary>
    /// <returns>A new sub-pad object.</returns>
    /// <exception cref="ObjectDisposedException">Sub-pad is no longer usable.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    ISubPad Duplicate();
}

