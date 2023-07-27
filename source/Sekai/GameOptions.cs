// Copyright (c) Cosyne
// Licensed under MIT. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using Sekai.Audio;
using Sekai.Graphics;
using Sekai.Headless;
using Sekai.Input;
using Sekai.Logging;
using Sekai.Mathematics;
using Sekai.Storages;
using Sekai.Windowing;

namespace Sekai;

/// <summary>
/// Options passed to the <see cref="Game"/>.
/// </summary>
public class GameOptions
{
    /// <summary>
    /// The update rate (in seconds) of the game when <see cref="TickMode"/> is <see cref="TickMode.Fixed"/>.
    /// </summary>
    public double UpdateRate = 120.0;

    /// <summary>
    /// The ticking mode.
    /// </summary>
    /// <remarks>
    /// When set to <see cref="TickMode.Fixed"/>, the target time between frames is provided by <see cref="UpdateRate"/>.
    /// Otherwise, it is the duration of each elapsed frame.
    /// </remarks>
    public TickMode TickMode = TickMode.Fixed;

    /// <summary>
    /// The execution mode.
    /// </summary>
    public ExecutionMode ExecutionMode = ExecutionMode.MultiThread;

    /// <summary>
    /// The view options.
    /// </summary>
    public ViewOptions View = new();

    /// <summary>
    /// The audio options.
    /// </summary>
    public AudioOptions Audio = new();

    /// <summary>
    /// The input options.
    /// </summary>
    public InputOptions Input = new();

    /// <summary>
    /// The logger options.
    /// </summary>
    public LoggerOptions Logger = new();

    /// <summary>
    /// The window options.
    /// </summary>
    public WindowOptions Window => View as WindowOptions ?? throw new InvalidOperationException("Cannot modify view options as window options.");

    /// <summary>
    /// The storage options.
    /// </summary>
    public StorageOptions Storage = new();

    /// <summary>
    /// The graphics options.
    /// </summary>
    public GraphicsOptions Graphics = new();

    /// <summary>
    /// The launch options.
    /// </summary>
    public readonly LaunchOptions Launch = LaunchOptions.Default;

    public class LaunchOptions
    {
        public static readonly LaunchOptions Default = new();

        private LaunchOptions()
        {
        }

        /// <summary>
        /// The arguments passed when the program was launched.
        /// </summary>
        public IReadOnlyList<string> Arguments { get; } = Environment.GetCommandLineArgs();

        /// <summary>
        /// The environment variables passed when the program was launched.
        /// </summary>
        public IDictionary Variables { get; } = Environment.GetEnvironmentVariables();
    }

    public class AudioOptions
    {
        /// <summary>
        /// The default audio options.
        /// </summary>
        public static readonly AudioOptions Default = new();

        /// <summary>
        /// The audio device name to use.
        /// </summary>
        public string Device = "Default";

        /// <summary>
        /// The device creator.
        /// </summary>
        public Func<AudioDevice> CreateDevice = () => new HeadlessAudioDevice();
    }

    public class GraphicsOptions
    {
        /// <summary>
        /// Should vertical sync be enabled.
        /// </summary>
        public bool SyncMode = true;

        /// <summary>
        /// The device creator.
        /// </summary>
        public Func<IView, GraphicsDevice> CreateDevice = _ => new HeadlessGraphicsDevice();
    }

    public class InputOptions
    {
        private readonly ImmutableArray<IInputSource>.Builder sources = ImmutableArray.CreateBuilder<IInputSource>();

        /// <summary>
        /// Adds an <see cref="IInputSource"/>.
        /// </summary>
        /// <param name="source">The source to add.</param>
        public void AddSource(IInputSource source)
        {
            sources.Add(source);
        }

        /// <summary>
        /// Builds the final <see cref="IInputSource"/>.
        /// </summary>
        public IInputSource Create()
        {
            if (sources.Count == 1)
            {
                return sources[0];
            }
            else
            {
                return new InputSource(sources.ToImmutable());
            }
        }
    }

    public class StorageOptions
    {
        private readonly ImmutableDictionary<string, Storage>.Builder storages = ImmutableDictionary.CreateBuilder<string, Storage>();

    /// <summary>
    /// Adds a storage at a given path.
    /// </summary>
    /// <param name="path">The path to be mounted</param>
    /// <param name="storage">The storage to mount.</param>
    /// <param name="writable">Whether the storage can be written to.</param>
    /// <exception cref="ArgumentException">Thrown when a storage is already mounted at a given path.</exception>
    public void AddStorage([StringSyntax(StringSyntaxAttribute.Uri)] string path, Storage storage)
    {
        path = makeRelativePath(path);

        if (storages.ContainsKey(path))
        {
            throw new ArgumentException($"There is a storage already mounted at {path}", nameof(path));
        }

        storages.Add(path, storage);
    }

    /// <summary>
    /// Adds a local directory from a <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="info">The directory to mount.</param>
    public void AddDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string path, DirectoryInfo info)
    {
        AddStorage(path, new NativeStorage(info));
    }

    /// <summary>
    /// Adds a local directory at a given path.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="directoryPath">The filesystem path to the directory to mount.</param>
    public void AddDirectory([StringSyntax(StringSyntaxAttribute.Uri)] string path, string directoryPath)
    {
        AddStorage(path, new NativeStorage(directoryPath));
    }

    /// <summary>
    /// Adds an <see cref="Assembly"/>.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="assembly">The assembly to mount.</param>
    public void AddAssembly([StringSyntax(StringSyntaxAttribute.Uri)] string path, Assembly assembly)
    {
        AddStorage(path, new AssemblyStorage(assembly));
    }

    /// <summary>
    /// Adds an <see cref="Assembly"/> from an <see cref="AssemblyName"/>.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="assemblyName">The assembly name to mount.</param>
    public void AddAssembly([StringSyntax(StringSyntaxAttribute.Uri)] string path, AssemblyName assemblyName)
    {
        AddStorage(path, new AssemblyStorage(assemblyName));
    }

    /// <summary>
    /// Adds an <see cref="Assembly"/> from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="stream">The stream to mount as an assembly.</param>
    public void AddAssembly([StringSyntax(StringSyntaxAttribute.Uri)] string path, Stream stream)
    {
        byte[] buffer = new byte[(int)stream.Length];

        if (stream.Read(buffer) <= 0)
        {
            throw new InvalidOperationException("Failed to read stream.");
        }

        stream.Position = 0;

        AddStorage(path, new AssemblyStorage(Assembly.Load(buffer)));
    }

    /// <summary>
    /// Adds an <see cref="Assembly"/> from a file.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="assemblyPath">The assembly's path to mount.</param>
    public void AddAssembly([StringSyntax(StringSyntaxAttribute.Uri)] string path, string assemblyPath)
    {
        AddStorage(path, new AssemblyStorage(assemblyPath));
    }

    /// <summary>
    /// Adds an archive.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="archive">The archive to mount.</param>
    public void AddArchive([StringSyntax(StringSyntaxAttribute.Uri)] string path, ZipArchive archive)
    {
        AddStorage(path, new ArchiveStorage(archive));
    }

    /// <summary>
    /// Adds an archive from a <see cref="Stream"/>.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="stream">The stream to mount as an archive.</param>
    public void AddArchive([StringSyntax(StringSyntaxAttribute.Uri)] string path, Stream stream, ZipArchiveMode mode = ZipArchiveMode.Read)
    {
        AddStorage(path, new ArchiveStorage(stream, mode));
    }

    /// <summary>
    /// Adds an archive from a file.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    /// <param name="archivePath">The archive's path to mount.</param>
    public void AddArchive([StringSyntax(StringSyntaxAttribute.Uri)] string path, string archivePath, ZipArchiveMode mode = ZipArchiveMode.Read)
    {
        AddStorage(path, new ArchiveStorage(archivePath, mode));
    }

    /// <summary>
    /// Adds a non-persistent in-memory storage.
    /// </summary>
    /// <param name="path">The virtual path.</param>
    public void AddMemory([StringSyntax(StringSyntaxAttribute.Uri)] string path)
    {
        AddStorage(path, new MemoryStorage());
    }

    /// <summary>
    /// Compiles the storage builder.
    /// </summary>
    /// <returns>The built storage.</returns>
    public Storage Create()
    {
        return new VirtualStorage(storages.ToImmutable());
    }

    private static string makeRelativePath([StringSyntax(StringSyntaxAttribute.Uri)] string path)
    {
        if (!(path.EndsWith(Path.DirectorySeparatorChar) || path.EndsWith(Path.AltDirectorySeparatorChar)))
        {
            path += Path.AltDirectorySeparatorChar;
        }

        return new Uri(baseUri, path).AbsolutePath;
    }

    private static readonly Uri baseUri = new("file:///");
    }

    public class ViewOptions
    {
        /// <summary>
        /// The window creator.
        /// </summary>
        public Func<IView> Create = () => new HeadlessView();
    }

    public class WindowOptions : ViewOptions
    {
        /// <summary>
        /// The window title.
        /// </summary>
        public string Title = string.Empty;

        /// <summary>
        /// The window size.
        /// </summary>
        public Size Size = new(1280, 720);

        /// <summary>
        /// The window minimum size.
        /// </summary>
        public Size MinimumSize;

        /// <summary>
        /// The window maximum size.
        /// </summary>
        public Size MaximumSize;

        /// <summary>
        /// The window initial position.
        /// </summary>
        public Point Position = new(-1, -1);

        /// <summary>
        /// The window border style.
        /// </summary>
        public WindowBorder Border = WindowBorder.Resizable;
    }

    public class LoggerOptions
    {
        /// <summary>
        /// The frequency of logging.
        /// </summary>
        public int Frequency = 1;

        /// <summary>
        /// The minimum log level.
        /// </summary>
        public LogLevel MinimumLevel = LogLevel.Trace;

        /// <summary>
        /// The maximum log level.
        /// </summary>
        public LogLevel MaximumLevel = LogLevel.Critical;

        /// <summary>
        /// The predicate used to filter messages.
        /// </summary>
        /// <remarks>
        /// Return <see langword="true"/> to filter the message. Otherwise, return <see langword="false"/>
        /// </remarks>
        public Predicate<LogMessage>? Filter;

        private readonly ImmutableArray<LogWriter>.Builder writers = ImmutableArray.CreateBuilder<LogWriter>();
        private readonly ImmutableArray<string>.Builder textWriters = ImmutableArray.CreateBuilder<string>();
        private readonly ImmutableArray<string>.Builder jsonWriters = ImmutableArray.CreateBuilder<string>();
        private readonly ImmutableDictionary<Type, LogWriter>.Builder typedWriters = ImmutableDictionary.CreateBuilder<Type, LogWriter>();

        /// <summary>
        /// Adds a custom writer.
        /// </summary>
        /// <param name="writer">The writer to add.</param>
        public void AddWriter(LogWriter writer)
        {
            writers.Add(writer);
        }

        /// <summary>
        /// Adds a console logger.
        /// </summary>
        public void AddConsole()
        {
            addTypedWriter<LogWriterConsole>();
        }

        /// <summary>
        /// Adds a debug logger.
        /// </summary>
        public void AddDebug()
        {
            addTypedWriter<LogWriterDebug>();
        }

        /// <summary>
        /// Adds a trace logger.
        /// </summary>
        public void AddTrace()
        {
            addTypedWriter<LogWriterTrace>();
        }

        /// <summary>
        /// Adds a text logger.
        /// </summary>
        /// <param name="path">The virtual path where the contents will be written to.</param>
        public void AddText(string path)
        {
            textWriters.Add(path);
        }

        /// <summary>
        /// Adds a json logger.
        /// </summary>
        /// <param name="path">The virtual path where the contents will be written to.</param>
        public void AddJson(string path)
        {
            jsonWriters.Add(path);
        }

        /// <summary>
        /// Compiles and builds the logger.
        /// </summary>
        /// <param name="storage">The storage for the logger to access to.</param>
        public ILogger Create(Storage storage)
        {
            foreach (string path in textWriters)
            {
                writers.Add(new LogWriterText(storage.Open(path, FileMode.Truncate, FileAccess.Write)));
            }

            foreach (string path in jsonWriters)
            {
                writers.Add(new LogWriterJson(storage.Open(path, FileMode.Truncate, FileAccess.Write)));
            }

            foreach (var writer in typedWriters)
            {
                writers.Add(writer.Value);
            }

            return new Logger(writers.ToImmutable(), Frequency, MinimumLevel, MaximumLevel, Filter);
        }

        private void addTypedWriter<T>()
            where T : LogWriter, new()
        {
            if (typedWriters.ContainsKey(typeof(T)))
            {
                return;
            }

            typedWriters.Add(typeof(T), new T());
        }
    }
}
