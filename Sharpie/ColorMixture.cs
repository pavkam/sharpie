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
///     Defines a color pair (foreground and background colors).
/// </summary>
[PublicAPI]
public struct ColorMixture
{
    /// <summary>
    ///     The default color mixture of the terminal. Use it to reset to default colors.
    /// </summary>
    public static ColorMixture Default { get; } = new() { Handle = 0 };

    /// <summary>
    ///     The handle of the color pair.
    /// </summary>
    internal short Handle { get; init; }

    /// <inheritdoc cref="object.ToString" />
    public override string ToString() => $"Mixture [{Handle}]";

    /// <inheritdoc cref="object.Equals(object)" />
    public override bool Equals(object? obj) => obj is ColorMixture cm && cm.Handle == Handle;

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => Handle.GetHashCode();

    /// <summary>
    ///     The equality operator.
    /// </summary>
    /// <param name="left">LHS argument.</param>
    /// <param name="right">RHS argument.</param>
    /// <returns>The result of the check.</returns>
    public static bool operator ==(ColorMixture left, ColorMixture right) => left.Equals(right);

    /// <summary>
    ///     The inequality operator.
    /// </summary>
    /// <param name="left">LHS argument.</param>
    /// <param name="right">RHS argument.</param>
    /// <returns>The result of the check.</returns>
    public static bool operator !=(ColorMixture left, ColorMixture right) => !(left == right);
}
