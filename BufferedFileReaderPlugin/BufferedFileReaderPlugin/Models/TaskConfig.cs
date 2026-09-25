namespace BufferedFileReaderPlugin.Models;

/// <summary>
/// Task configuration.
/// </summary>
public sealed class TaskConfig
{
    /// <summary>
    /// Path to the text file.
    /// </summary>
    public string FilePath { get; init; } = string.Empty;
}
