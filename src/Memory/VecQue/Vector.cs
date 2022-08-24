namespace Rustic.Memory;

/// <summary>
/// Defines the behavior of an array based collection when the capacity is exceeded.
/// </summary>
public enum GrowthStrategy
{
    /// <summary>
    /// Increases the capacity of the array by at least the required amount, also exponentially, usually the previous capacity multiplied by 2.
    /// </summary>
    Exponential = 0,
    /// <summary>
    /// Never increase the capacity of the array. Overwrite the disadvantaged element.
    /// </summary>
    /// <remarks>
    /// For FIFO queues, this is the last added element.
    /// For LIFO queues, this is the first added element.
    /// </remarks>
    FixedSizeAlign,
}

public static class VectorExtensions
{

}
