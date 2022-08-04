// Copyright (c) The Vignette Authors
// Licensed under MIT. See LICENSE for details.

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Sekai.Framework.Windowing;

namespace Sekai.Forms;

public class FormsWindow : IWindow, INativeWindow
{
    public nint Handle => form.Handle;
    public bool Focused => form.Focused;
    public event Action OnLoad = null!;
    public event Action OnClose = null!;
    public event Func<bool> OnCloseRequested = null!;
    public event Action<Size> OnResize = null!;
    public event Action<bool> OnFocusChanged = null!;
    public event Action<string[]> OnDataDropped = null!;
    private bool resizable;
    private ReadOnlyMemory<byte> icon;
    private readonly Form form;

    public string Title
    {
        get => form.Text;
        set => form.Text = value;
    }

    public bool Visible
    {
        get => form.Visible;
        set => form.Visible = value;
    }

    public ReadOnlyMemory<byte> Icon
    {
        get => icon;
        set
        {
            if (icon.Equals(value))
                return;

            icon = value;

            using var stream = new MemoryStream();
            stream.Write(icon.Span);
            form.Icon = new Icon(stream);
        }
    }

    public Size Size
    {
        get => form.Size;
        set => form.Size = value;
    }

    public Size MinimumSize
    {
        get => form.MinimumSize;
        set => form.MaximumSize = value;
    }

    public Size MaximumSize
    {
        get => form.MaximumSize;
        set => form.MaximumSize = value;
    }

    public Point Position
    {
        get => form.DesktopLocation;
        set => form.DesktopLocation = value;
    }

    public bool Resizable
    {
        get => resizable;
        set
        {
            resizable = value;
            form.MaximizeBox = value;
            form.MinimizeBox = value;
            form.FormBorderStyle = resizable ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle;
        }
    }

    public FormsWindow()
    {
        form = new();
        form.Load += handleFormLoad;
        form.Resize += handleFormResize;
        form.GotFocus += handleFormFocusChanged;
        form.LostFocus += handleFormFocusChanged;
        form.FormClosed += handleFormClose;
        form.FormClosing += handleFormClosing;
        form.AllowDrop = true;
    }

    public void DoEvents()
    {
        Application.DoEvents();
    }

    private void handleFormLoad(object? sender, EventArgs args)
    {
        OnLoad?.Invoke();
    }

    private void handleFormClose(object? sender, EventArgs args)
    {
        OnClose?.Invoke();
    }

    private void handleFormClosing(object? sender, CancelEventArgs args)
    {
        args.Cancel = OnCloseRequested?.Invoke() ?? false;
    }

    private void handleFormResize(object? sender, EventArgs args)
    {
        OnResize?.Invoke(Size);
    }

    private void handleFormFocusChanged(object? sender, EventArgs args)
    {
        OnFocusChanged?.Invoke(Focused);
    }

    private void handleFormDragEnter(object? sender, DragEventArgs args)
    {
        if (args.Data == null)
            return;

        if (args.Data.GetDataPresent(DataFormats.FileDrop) || args.Data.GetDataPresent(DataFormats.StringFormat))
            args.Effect = DragDropEffects.Copy;
    }

    private void handleFormDragDrop(object? sender, DragEventArgs args)
    {
        if (args.Data == null)
            return;

        if (args.Data.GetDataPresent(DataFormats.FileDrop))
        {
            string[] files = (string[])args.Data.GetData(DataFormats.FileDrop);
            OnDataDropped?.Invoke(files);
            return;
        }

        if (args.Data.GetDataPresent(DataFormats.StringFormat))
        {
            string[] files = (string[])args.Data.GetData(DataFormats.StringFormat);
            OnDataDropped?.Invoke(files);
            return;
        }
    }
}
