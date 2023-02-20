// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System.Threading.Tasks;

namespace Sekai;

/// <summary>
/// Provides necessary methods in starting a <see cref="Host"/>.
/// </summary>
public interface IHostStartup
{
    /// <summary>
    /// Runs the host on the calling thread.
    /// </summary>
    void Run();

    /// <summary>
    /// Runs the host that returns a task that only completes when the host closes.
    /// </summary>
    /// <returns>A task that represents the entire host's lifetime.</returns>
    Task RunAsync();
}
