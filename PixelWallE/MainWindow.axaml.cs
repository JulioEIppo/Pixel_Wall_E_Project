using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using PixelWallE;
using System;
using Avalonia.Interactivity;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using AvaloniaEdit.Document;
using Avalonia.Platform.Storage;
using System.Threading.Tasks;
using MsBox.Avalonia;
using Avalonia.Threading;

namespace PixelWallE;

public partial class MainWindow : Window
{
    public int DefaultSize = WallE.GetCanvas().GetLength(0);
    private CanvasLogic canvasLogic;
    public MainWindow()
    {
        InitializeComponent();
        SizeInput.TextChanged += (s, e) => UpdateDimensions();
        PixelSizeInput.TextChanged += (s, e) => UpdateDimensions();
        canvasLogic = new CanvasLogic(this);
        CodeTextBox.TextChanged += CodeTextBox_TextChanged;
        CodeScrollViewer.ScrollChanged += CodeScrollViewer_ScrollChanged;
    }
    public void UpdateCanvas(CanvasColor[,] canvasData)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            try
            {
                for (int x = 0; x < canvasData.GetLength(0); x++)
                    for (int y = 0; y < canvasData.GetLength(1); y++)
                        PixelCanvasControl.UpdatePixel(x, y, canvasData[x, y]);
            }
            catch (Exception)
            {
                await ShowMessage("Error", "Error updating canvas");
            }

        });

    }

    private void UpdateDimensions()
    {
        if (int.TryParse(SizeInput.Text, out int gridSize) && gridSize > 0 && int.TryParse(PixelSizeInput.Text, out int pixelSize) && pixelSize > 0)
        {
            PixelCanvasControl.Rows = gridSize;
            PixelCanvasControl.Columns = gridSize;
            PixelCanvasControl.CellSize = pixelSize;

            PixelCanvasControl.Width = gridSize * pixelSize;
            PixelCanvasControl.Height = gridSize * pixelSize;
        }
    }
    public async Task ShowMessage(string title, string message)
    {
        await MessageBoxManager.GetMessageBoxStandard(title, message).ShowWindowAsync();

    }
    private async void OnResizeClick(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(SizeInput.Text, out int size) || size <= 0 || size > 1024 || !int.TryParse(PixelSizeInput.Text, out int pixelSize)
            || pixelSize <= 0 || pixelSize > 50)
        {
            await ShowMessage("Error", "Invalid values for Canvas's size and pixel's size ( 0 < canvas < 1024) (0 < pixel < 50)");
            return;
        }

        try
        {
            ResizeButton.IsEnabled = false;

            PixelCanvasControl.Rows = size;
            PixelCanvasControl.Columns = size;
            PixelCanvasControl.CellSize = pixelSize;

            await Task.Run(() => canvasLogic.ResizeCanvas(size));
            PixelCanvasControl.InvalidateVisual();
        }
        catch (Exception)
        {
            await ShowMessage("Error", $"Couldn't resize canvas");
        }
        finally
        {
            ResizeButton.IsEnabled = true;
        }
    }


    private async void OnRunClick(object sender, RoutedEventArgs e)
    {
        await canvasLogic.RunCode(CodeTextBox.Text!);
    }


    private async void OnLoadClick(object? sender, RoutedEventArgs e)
    {
        var filePicker = GetTopLevel(this)?.StorageProvider;
        if (filePicker == null) return;
        try
        {
            var files = await filePicker.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Load file",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("PixelWallE Files")
                    {
                        Patterns = new[] { "*.gw", "*.pw" },
                        MimeTypes = new[] { "text/plain" }
                    }
                }
            });
            if (files != null && files.Count > 0)
            {
                var text = await File.ReadAllTextAsync(files[0].Path.LocalPath);
                CodeTextBox.Text = text;
            }
        }
        catch (Exception ex)
        {
            await ShowMessage("Error", "Couldn't load file: " + ex.Message);
        }
    }

    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var filePicker = GetTopLevel(this)?.StorageProvider;
        if (filePicker == null) return;
        try
        {
            var file = await filePicker.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Save file",
                SuggestedFileName = "code.pw",
                FileTypeChoices = new[]
                {
                    new FilePickerFileType("PixelWallE Files")
                    {
                        Patterns = new[] { "*.gw", "*.pw" },
                        MimeTypes = new[] { "text/plain" }
                    }
                }
            });
            if (file != null)
            {
                await File.WriteAllTextAsync(file.Path.LocalPath, CodeTextBox.Text);
            }
        }
        catch (Exception ex)
        {
            await ShowMessage("Error", "Couldn't save file: " + ex.Message);
        }
    }

    private void OnGridClick(object sender, RoutedEventArgs e)
    {
        PixelCanvasControl.ShowGrid = !PixelCanvasControl.ShowGrid;
        GridButton.Content = PixelCanvasControl.ShowGrid ? "Grid: On" : "Grid: Off";
    }
    private void CodeTextBox_TextChanged(object? sender, EventArgs e)
    {
        var lines = CodeTextBox.Text?.Split('\n').Length ?? 1;
        LineNumbers.Text = string.Join("\n", Enumerable.Range(1, lines));
    }

    private void CodeScrollViewer_ScrollChanged(object? sender, ScrollChangedEventArgs e)
    {
        // Sincroniza el scroll vertical de la numeración con el del editor
        LineScrollViewer.Offset = new Vector(LineScrollViewer.Offset.X, CodeScrollViewer.Offset.Y);
    }
}