using System;
using System.Collections.Generic;
using System.Drawing;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using TextMateSharp.Themes;

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
            canvasData = new CanvasColor[Columns, Rows];
            Initialize();
            MinWidth = 100;
            MinHeight = 100;
            MaxWidth = 2000;
            MaxHeight = 2000;
        }
        public void Initialize()
        {
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                    canvasData[x, y] = CanvasColor.White;
        }

        // private static Avalonia.Media.Color ToAvaloniaColor(CanvasColor color)
        // {
        //     return color switch
        //     {
        //         CanvasColor.Red => Colors.Red,
        //         CanvasColor.Blue => Colors.Blue,
        //         CanvasColor.Green => Colors.Green,
        //         CanvasColor.Yellow => Colors.Yellow,
        //         CanvasColor.Orange => Colors.Orange,
        //         CanvasColor.Purple => Colors.Purple,
        //         CanvasColor.Black => Colors.Black,
        //         CanvasColor.White => Colors.White,
        //         CanvasColor.Transparent => Colors.Transparent,
        //         _ => Colors.Gray
        //     };

        // }

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
                canvasData = new CanvasColor[Columns, Rows];
                Initialize();
                InvalidateMeasure();
                InvalidateVisual();
            }

        }

        public void UpdatePixel(int x, int y, CanvasColor color)
        {
            if (canvasData == null) return;
            if (x < 0 || x >= Columns || y < 0 || y >= Rows) return;
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
            for (int y = 0; y < Rows; y++)
                for (int x = 0; x < Columns; x++)
                {
                    // var color = ToAvaloniaColor(canvasData[x, y]);
                    var brush = GetBrushColor(canvasData[x, y]);
                    context.FillRectangle(brush, new Rect(x * cellSize, y * cellSize, cellSize, cellSize));
                }
        }
        private void RenderGrid(DrawingContext context)
        {
            int cellSize = CellSize;
            var gridPen = new Pen(Brushes.LightGray, 0.5);

            for (int x = 0; x <= Columns; x++)
            {
                context.DrawLine(gridPen, new Avalonia.Point(x * cellSize, 0), new Avalonia.Point(x * cellSize, Rows * cellSize));
            }
            for (int y = 0; y <= Rows; y++)
            {
                context.DrawLine(gridPen, new Avalonia.Point(0, y * cellSize), new Avalonia.Point(Columns * cellSize, y * cellSize));
            }
        }

        public static IBrush GetBrushColor(CanvasColor color)
        {
            if (BrushColors.TryGetValue(color, out var brush))
            {
                return brush;
            }
            return Brushes.Transparent;
        }
        private static readonly Dictionary<CanvasColor, IBrush> BrushColors = new()
    {
        { CanvasColor.Transparent, Brushes.Transparent },
            { CanvasColor.AliceBlue, Brushes.AliceBlue },
            { CanvasColor.AntiqueWhite, Brushes.AntiqueWhite },
            { CanvasColor.Aqua, Brushes.Aqua },
            { CanvasColor.Aquamarine, Brushes.Aquamarine },
            { CanvasColor.Azure, Brushes.Azure },
            { CanvasColor.Beige, Brushes.Beige },
            { CanvasColor.Bisque, Brushes.Bisque },
            { CanvasColor.Black, Brushes.Black },
            { CanvasColor.BlanchedAlmond, Brushes.BlanchedAlmond },
            { CanvasColor.Blue, Brushes.Blue },
            { CanvasColor.BlueViolet, Brushes.BlueViolet },
            { CanvasColor.Brown, Brushes.Brown },
            { CanvasColor.BurlyWood, Brushes.BurlyWood },
            { CanvasColor.CadetBlue, Brushes.CadetBlue },
            { CanvasColor.Chartreuse, Brushes.Chartreuse },
            { CanvasColor.Chocolate, Brushes.Chocolate },
            { CanvasColor.Coral, Brushes.Coral },
            { CanvasColor.CornflowerBlue, Brushes.CornflowerBlue },
            { CanvasColor.Cornsilk, Brushes.Cornsilk },
            { CanvasColor.Crimson, Brushes.Crimson },
            { CanvasColor.Cyan, Brushes.Cyan },
            { CanvasColor.DarkBlue, Brushes.DarkBlue },
            { CanvasColor.DarkCyan, Brushes.DarkCyan },
            { CanvasColor.DarkGoldenrod, Brushes.DarkGoldenrod },
            { CanvasColor.DarkGray, Brushes.DarkGray },
            { CanvasColor.DarkGreen, Brushes.DarkGreen },
            { CanvasColor.DarkKhaki, Brushes.DarkKhaki },
            { CanvasColor.DarkMagenta, Brushes.DarkMagenta },
            { CanvasColor.DarkOliveGreen, Brushes.DarkOliveGreen },
            { CanvasColor.DarkOrange, Brushes.DarkOrange },
            { CanvasColor.DarkOrchid, Brushes.DarkOrchid },
            { CanvasColor.DarkRed, Brushes.DarkRed },
            { CanvasColor.DarkSalmon, Brushes.DarkSalmon },
            { CanvasColor.DarkSeaGreen, Brushes.DarkSeaGreen },
            { CanvasColor.DarkSlateBlue, Brushes.DarkSlateBlue },
            { CanvasColor.DarkSlateGray, Brushes.DarkSlateGray },
            { CanvasColor.DarkTurquoise, Brushes.DarkTurquoise },
            { CanvasColor.DarkViolet, Brushes.DarkViolet },
            { CanvasColor.DeepPink, Brushes.DeepPink },
            { CanvasColor.DeepSkyBlue, Brushes.DeepSkyBlue },
            { CanvasColor.DimGray, Brushes.DimGray },
            { CanvasColor.DodgerBlue, Brushes.DodgerBlue },
            { CanvasColor.Firebrick, Brushes.Firebrick },
            { CanvasColor.FloralWhite, Brushes.FloralWhite },
            { CanvasColor.ForestGreen, Brushes.ForestGreen },
            { CanvasColor.Fuchsia, Brushes.Fuchsia },
            { CanvasColor.Gainsboro, Brushes.Gainsboro },
            { CanvasColor.GhostWhite, Brushes.GhostWhite },
            { CanvasColor.Gold, Brushes.Gold },
            { CanvasColor.Goldenrod, Brushes.Goldenrod },
            { CanvasColor.Gray, Brushes.Gray },
            { CanvasColor.Green, Brushes.Green },
            { CanvasColor.GreenYellow, Brushes.GreenYellow },
            { CanvasColor.Honeydew, Brushes.Honeydew },
            { CanvasColor.HotPink, Brushes.HotPink },
            { CanvasColor.IndianRed, Brushes.IndianRed },
            { CanvasColor.Indigo, Brushes.Indigo },
            { CanvasColor.Ivory, Brushes.Ivory },
            { CanvasColor.Khaki, Brushes.Khaki },
            { CanvasColor.Lavender, Brushes.Lavender },
            { CanvasColor.LavenderBlush, Brushes.LavenderBlush },
            { CanvasColor.LawnGreen, Brushes.LawnGreen },
            { CanvasColor.LemonChiffon, Brushes.LemonChiffon },
            { CanvasColor.LightBlue, Brushes.LightBlue },
            { CanvasColor.LightCoral, Brushes.LightCoral },
            { CanvasColor.LightCyan, Brushes.LightCyan },
            { CanvasColor.LightGoldenrodYellow, Brushes.LightGoldenrodYellow },
            { CanvasColor.LightGray, Brushes.LightGray },
            { CanvasColor.LightGreen, Brushes.LightGreen },
            { CanvasColor.LightPink, Brushes.LightPink },
            { CanvasColor.LightSalmon, Brushes.LightSalmon },
            { CanvasColor.LightSeaGreen, Brushes.LightSeaGreen },
            { CanvasColor.LightSkyBlue, Brushes.LightSkyBlue },
            { CanvasColor.LightSlateGray, Brushes.LightSlateGray },
            { CanvasColor.LightSteelBlue, Brushes.LightSteelBlue },
            { CanvasColor.LightYellow, Brushes.LightYellow },
            { CanvasColor.Lime, Brushes.Lime },
            { CanvasColor.LimeGreen, Brushes.LimeGreen },
            { CanvasColor.Linen, Brushes.Linen },
            { CanvasColor.Magenta, Brushes.Magenta },
            { CanvasColor.Maroon, Brushes.Maroon },
            { CanvasColor.MediumAquamarine, Brushes.MediumAquamarine },
            { CanvasColor.MediumBlue, Brushes.MediumBlue },
            { CanvasColor.MediumOrchid, Brushes.MediumOrchid },
            { CanvasColor.MediumPurple, Brushes.MediumPurple },
            { CanvasColor.MediumSeaGreen, Brushes.MediumSeaGreen },
            { CanvasColor.MediumSlateBlue, Brushes.MediumSlateBlue },
            { CanvasColor.MediumSpringGreen, Brushes.MediumSpringGreen },
            { CanvasColor.MediumTurquoise, Brushes.MediumTurquoise },
            { CanvasColor.MediumVioletRed, Brushes.MediumVioletRed },
            { CanvasColor.MidnightBlue, Brushes.MidnightBlue },
            { CanvasColor.MintCream, Brushes.MintCream },
            { CanvasColor.MistyRose, Brushes.MistyRose },
            { CanvasColor.Moccasin, Brushes.Moccasin },
            { CanvasColor.NavajoWhite, Brushes.NavajoWhite },
            { CanvasColor.Navy, Brushes.Navy },
            { CanvasColor.OldLace, Brushes.OldLace },
            { CanvasColor.Olive, Brushes.Olive },
            { CanvasColor.OliveDrab, Brushes.OliveDrab },
            { CanvasColor.Orange, Brushes.Orange },
            { CanvasColor.OrangeRed, Brushes.OrangeRed },
            { CanvasColor.Orchid, Brushes.Orchid },
            { CanvasColor.PaleGoldenrod, Brushes.PaleGoldenrod },
            { CanvasColor.PaleGreen, Brushes.PaleGreen },
            { CanvasColor.PaleTurquoise, Brushes.PaleTurquoise },
            { CanvasColor.PaleVioletRed, Brushes.PaleVioletRed },
            { CanvasColor.PapayaWhip, Brushes.PapayaWhip },
            { CanvasColor.PeachPuff, Brushes.PeachPuff },
            { CanvasColor.Peru, Brushes.Peru },
            { CanvasColor.Pink, Brushes.Pink },
            { CanvasColor.Plum, Brushes.Plum },
            { CanvasColor.PowderBlue, Brushes.PowderBlue },
            { CanvasColor.Purple, Brushes.Purple },
            { CanvasColor.Red, Brushes.Red },
            { CanvasColor.RosyBrown, Brushes.RosyBrown },
            { CanvasColor.RoyalBlue, Brushes.RoyalBlue },
            { CanvasColor.SaddleBrown, Brushes.SaddleBrown },
            { CanvasColor.Salmon, Brushes.Salmon },
            { CanvasColor.SandyBrown, Brushes.SandyBrown },
            { CanvasColor.SeaGreen, Brushes.SeaGreen },
            { CanvasColor.SeaShell, Brushes.SeaShell },
            { CanvasColor.Sienna, Brushes.Sienna },
            { CanvasColor.Silver, Brushes.Silver },
            { CanvasColor.SkyBlue, Brushes.SkyBlue },
            { CanvasColor.SlateBlue, Brushes.SlateBlue },
            { CanvasColor.SlateGray, Brushes.SlateGray },
            { CanvasColor.Snow, Brushes.Snow },
            { CanvasColor.SpringGreen, Brushes.SpringGreen },
            { CanvasColor.SteelBlue, Brushes.SteelBlue },
            { CanvasColor.Tan, Brushes.Tan },
            { CanvasColor.Teal, Brushes.Teal },
            { CanvasColor.Thistle, Brushes.Thistle },
            { CanvasColor.Tomato, Brushes.Tomato },
            { CanvasColor.Turquoise, Brushes.Turquoise },
            { CanvasColor.Violet, Brushes.Violet },
            { CanvasColor.Wheat, Brushes.Wheat },
            { CanvasColor.White, Brushes.White },
            { CanvasColor.WhiteSmoke, Brushes.WhiteSmoke },
            { CanvasColor.Yellow, Brushes.Yellow },
            { CanvasColor.YellowGreen, Brushes.YellowGreen }
        };

    }
}


