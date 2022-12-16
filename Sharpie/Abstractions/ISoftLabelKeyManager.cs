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
///     Defines the traits needed to implement <see cref="SoftLabelKeyManager" />.
/// </summary>
[PublicAPI]
public interface ISoftLabelKeyManager
{
    /// <summary>
    ///     The terminal this manager belongs to.
    /// </summary>
    ITerminal Terminal { get; }

    /// <summary>
    ///     Specifies if the manager is enabled.
    /// </summary>
    bool Enabled { get; }

    /// <summary>
    ///     Gets the number of labels within the soft key label panel.
    /// </summary>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    int LabelCount { get; }

    /// <summary>
    ///     Gets or sets the style of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    Style Style { get; set; }

    /// <summary>
    ///     Gets or sets the color mixture of the window.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    ColorMixture ColorMixture { get; set; }

    /// <summary>
    ///     Sets a given label within the soft key label panel.
    /// </summary>
    /// <param name="index">The index of the label.</param>
    /// <param name="title">The title of the label.</param>
    /// <param name="align">Alignment of the label title.</param>
    /// <exception cref="ArgumentNullException">The <paramref name="title" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     The <paramref name="index" /> negative or greater than
    ///     <see cref="LabelCount" />.
    /// </exception>
    /// <exception cref="ObjectDisposedException">The terminal has been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void SetLabel(int index, string title, SoftLabelKeyAlignment align);

    /// <summary>
    ///     Enables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to enable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void EnableAttributes(VideoAttribute attributes);

    /// <summary>
    ///     Disables specified attributes and keep the others untouched.
    /// </summary>
    /// <param name="attributes">The attributes to disable.</param>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void DisableAttributes(VideoAttribute attributes);

    /// <summary>
    ///     Clears the soft key labels from the screen. They can be restored by calling <see cref="Restore" /> method.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Clear();

    /// <summary>
    ///     Restores the soft key labels to the screen. They can be cleared by calling <see cref="Clear" /> method.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Restore();

    /// <summary>
    ///     Marks the the soft key labels as dirty, and thus, eligible for refresh.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void MarkDirty();

    /// <summary>
    ///     Refreshes the soft label keys.
    /// </summary>
    /// <exception cref="ObjectDisposedException">The terminal or the current window have been disposed.</exception>
    /// <exception cref="NotSupportedException">The soft key labels are disabled.</exception>
    /// <exception cref="CursesSynchronizationException">
    ///     Thrown if this operation was expected to run on the main
    ///     thread/context but wasn't.
    /// </exception>
    /// <remarks>This operation is not thread safe.</remarks>
    void Refresh();
}

