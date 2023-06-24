// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

namespace Sekai.Input;

/// <summary>
/// The available deadzone methods to control the deadzone of a control stick.
/// </summary>
public enum DeadzoneMethod
{
    /// <summary>
    /// The traditional deadzone method, where the reported value is drectly proportional with the actual value is within the deadzone - the reported value will be 0 in this case.
    /// </summary>
    /// <remarks>
    /// <para>
    /// y = x except where |x| is between 0 and d
    /// </para>
    /// <para>
    /// y is the output, x is the raw value, and d is the deadzone value.
    /// </para>
    /// </remarks>
    Traditional,

    /// <summary>
    /// A deadzone method where the reported value adapts to the range of the deadzone. If the value is within the deadzone, the reported value is 0.
    /// After exiting the deadzone, the reported value increases from 0 (when the actual value first exits the deadzone) to 1 (when the actual value is 1).
    /// </summary>
    /// <remarks>
    /// <para>
    /// y = (1 - d)x + (d * sign(x))
    /// </para>
    /// <para>
    /// y is the output, x is the raw value, and d is the deadzone value.
    /// </para>
    /// </remarks>
    AdaptiveGradient,
}
