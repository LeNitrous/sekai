namespace Sekai.Framework.Graphics;

public class GraphicsContextOptions
{
    /// <summary>
    /// Whether debugging features are to be enabled. Use null to determine based on current configuration.
    /// </summary>
    public bool? Debug { get; set; } = null;

    /// <summary>
    /// The graphics API to be used. Use null to determine based on the current platform.
    /// </summary>
    public GraphicsAPI? GraphicsAPI { get; set; } = null;

    /// <summary>
    /// Whether to sync the presentation to the window system's vertical refresh rate.
    /// </summary>
    public bool VerticalSync { get; set; } = true;

    /// <summary>
    /// The depth target format to use.
    /// </summary>
    public PixelFormat? DepthTargetFormat { get; set; } = null;

    /// <summary>
    /// Whether the color target of the Swapchain will use an sRGB PixelFormat.
    /// </summary>
    public bool ColorSRGB { get; set; } = false;
}
