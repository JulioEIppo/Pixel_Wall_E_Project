using System;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace PixelWallE
{
    public class PixelCanvas : Control
    {
        private CanvasColor[,] canvasData;
        private bool showGrid = true;
        public bool ShowGrid
        {
            get => showGrid;
            set
            {
                showGrid = value;
                InvalidateVisual();
            }
        }
        public static readonly StyledProperty<int> RowsProperty =
        AvaloniaProperty.Register<PixelCanvas, int>(nameof(Rows), defaultValue: 100);

        public static readonly StyledProperty<int> ColumnsProperty =
            AvaloniaProperty.Register<PixelCanvas, int>(nameof(Columns), defaultValue: 100);

        public static readonly StyledProperty<int> CellSizeProperty =
            AvaloniaProperty.Register<PixelCanvas, int>(nameof(CellSize), defaultValue: 10);

        public int Rows
        {
            get => GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }
        public int Columns
        {
            get => GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public int CellSize
        {
            get => GetValue(CellSizeProperty);
            set => SetValue(CellSizeProperty, value);
        }
        public PixelCanvas()
        {
            canvasData = new CanvasColor[Rows, Columns];
            Initialize();
            MinWidth = 100;
            MinHeight = 100;
            MaxWidth = 2000;
            MaxHeight = 2000;
        }
        public void Initialize()
        {
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Columns; y++)
                    canvasData[x, y] = CanvasColor.White;
        }

        private static Avalonia.Media.Color ToAvaloniaColor(CanvasColor color)
        {
            return color switch
            {
                CanvasColor.Red => Colors.Red,
                CanvasColor.Blue => Colors.Blue,
                CanvasColor.Green => Colors.Green,
                CanvasColor.Yellow => Colors.Yellow,
                CanvasColor.Orange => Colors.Orange,
                CanvasColor.Purple => Colors.Purple,
                CanvasColor.Black => Colors.Black,
                CanvasColor.White => Colors.White,
                CanvasColor.Transparent => Colors.Transparent,
                _ => Colors.Gray
            };

        }

        public void SetCanvasData(CanvasColor[,] data)
        {
            canvasData = data;
            InvalidateVisual();
        }
        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == RowsProperty || change.Property == ColumnsProperty || change.Property == CellSizeProperty)
            {
                canvasData = new CanvasColor[Rows, Columns];
                Initialize();
                InvalidateMeasure();
                InvalidateVisual();
            }

        }



        public void UpdatePixel(int x, int y, CanvasColor color)
        {
            if (canvasData == null) return;
            if (x < 0 || x >= canvasData.GetLength(0) || y < 0 || y >= canvasData.GetLength(1)) return;
            canvasData[x, y] = color;
            InvalidateVisual();
        }
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (canvasData == null) return;

            RenderPixels(context);

            if (ShowGrid)
                RenderGrid(context);

        }
        private void RenderPixels(DrawingContext context)
        {
            int cellSize = CellSize;
            for (int x = 0; x < Rows; x++)
                for (int y = 0; y < Columns; y++)
                {
                    var color = ToAvaloniaColor(canvasData[x, y]);
                    var brush = new SolidColorBrush(color);
                    context.FillRectangle(brush, new Rect(y * cellSize, x * cellSize, cellSize, cellSize));
                }
        }
        private void RenderGrid(DrawingContext context)
        {
            int cellSize = CellSize;
            var gridPen = new Pen(Brushes.LightGray, 0.5);

            for (int x = 0; x <= Rows; x++)
            {
                context.DrawLine(gridPen, new Avalonia.Point(0, x * cellSize), new Avalonia.Point(Columns * cellSize, x * cellSize));
            }
            for (int y = 0; y <= Columns; y++)
            {
                context.DrawLine(gridPen, new Avalonia.Point(y * cellSize, 0), new Avalonia.Point(y * cellSize, Rows * cellSize));
            }

        }

    }
}

