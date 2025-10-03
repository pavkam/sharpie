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
///     A Curses, back-end-specific complex character that encodes the rune and its attributes.
/// </summary>
[PublicAPI, DebuggerDisplay("{ToString, nq}")]
public sealed class ComplexChar
{
    /// <summary>
    ///     Creates a new complex character.
    /// </summary>
    /// <param name="raw">The backend-specific payload.</param>
    internal ComplexChar(object? raw) => Raw = raw;

    /// <summary>
    ///     The backend-specific payload.
    /// </summary>
    internal object? Raw
    {
        get;
    }

    /// <inheritdoc cref="object.Equals(object?)" />
    public override bool Equals(object? obj) => obj is ComplexChar cc && Equals(cc.Raw, Raw);

    /// <inheritdoc cref="object.GetHashCode" />
    public override int GetHashCode() => Raw?.GetHashCode() ?? 0;

    /// <summary>
    ///     Equality comparison operator.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>The result of the comparison.</returns>
    public static bool operator ==(ComplexChar? left, ComplexChar? right) => Equals(left, right);

    /// <summary>
    ///     Inequality comparison operator.
    /// </summary>
    /// <param name="left">The left value.</param>
    /// <param name="right">The right value.</param>
    /// <returns>The result of the comparison.</returns>
    public static bool operator !=(ComplexChar? left, ComplexChar? right) => !Equals(left, right);

    /// <inheritdoc cref="object.ToString" />
    public override string? ToString() => Raw?.ToString();
}
