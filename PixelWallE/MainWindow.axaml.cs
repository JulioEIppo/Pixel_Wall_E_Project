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
    private int cellSize;
    public MainWindow()
    {
        InitializeComponent();
        SizeInput.TextChanged += (s, e) => UpdateDimensions();
        PixelSizeInput.TextChanged += (s, e) => UpdateDimensions();
        canvasLogic = new CanvasLogic(this);
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
            catch (Exception ex)
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
    private async Task OnResizeClick(object sender, RoutedEventArgs e)
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
        catch (Exception ex)
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
        await canvasLogic.RunCode(CodeEditor.Text);
    }

    
    private async void OnLoadClick(object sender, RoutedEventArgs e)
    {
        
    }

    private async void OnSaveClick(object sender, RoutedEventArgs e)
    {
        
    }

    // Método para alternar la cuadrícula (pendiente de implementar)
    private void OnGridClick(object sender, RoutedEventArgs e)
    {
        PixelCanvasControl.ShowGrid = !PixelCanvasControl.ShowGrid;
        GridButton.Content = PixelCanvasControl.ShowGrid ? "Grid: On" : "Grid: Off";
    }

}