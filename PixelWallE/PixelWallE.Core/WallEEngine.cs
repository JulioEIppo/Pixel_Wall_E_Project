using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using PixelWallE;

namespace PixelWallE
{
    public class WallEEngine
    {
        public CanvasColor[,] Canvas;
        public int CanvasSize => Canvas.GetLength(0);
        public int PositionX { get; private set; }
        public int PositionY { get; private set; }
        public CanvasColor BrushColor { get; private set; }
        public int BrushSize { get; private set; } = 1;
        public WallEEngine(int size)
        {
            Canvas = new CanvasColor[size, size];
            ResizeCanvas(size);
        }

        public void ResizeCanvas(int size)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Canvas[i, j] = CanvasColor.White;
                }
            }
            //reboot WallE state
            PositionX = 0;
            PositionY = 0;
            BrushColor = CanvasColor.Transparent;
            BrushSize = 1;
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < CanvasSize && y < CanvasSize;
        }
        public void Spawn(int x, int y, Token spawnToken)
        {
            if (!InBounds(x, y))
            {
                throw new RuntimeErrorException(spawnToken, $"Coordinates out of canvas: ({x},{y})");
            }
            PositionX = x;
            PositionY = y;
        }
        public void SetColor(string colorString, Token colorToken)
        {
            if (!Enum.TryParse<CanvasColor>(colorString, true, out var color) || !Enum.IsDefined(color))
            {
                throw new RuntimeErrorException(colorToken, $"Invalid color, {colorString} not found");
            }
            BrushColor = color;
        }
        public void SetSize(int size, Token sizeToken)
        {
            if (size <= 0)
                BrushSize = 1;
            else
                BrushSize = size % 2 == 0 ? size - 1 : size;
        }

        public void DrawLine(int dx, int dy, int distance, Token lineToken)
        {
            if (dx < -1 || dy < -1 || dx > 1 || dy > 1 || (dx == 0 && dy == 0))
            {
                throw new RuntimeErrorException(lineToken, "Invalid directions");
            }
            if (distance < 0) distance = 0;

            int x = PositionX;
            int y = PositionY;
            for (int steps = 0; steps < distance; steps++)
            {
                PaintBrush(x, y);
                x += dx;
                y += dy;
            }
            PaintBrush(x, y); // last cell to paint

            if (InBounds(x, y))
            {
                PositionX = x;
                PositionY = y;
            }
            // in case of out of bounds, we don't update positions
        }

        public void DrawCircle(int dx, int dy, int radius, Token circleToken)
        {
            if (dx < -1 || dy < -1 || dx > 1 || dy > 1)
            {
                throw new RuntimeErrorException(circleToken, "Invalid directions");
            }
            if (radius < 0) radius = 0;

            int centerX = PositionX + dx * radius;
            int centerY = PositionY + dy * radius;
            int targetRadius = radius * radius;
            for (int x = -radius; x <= radius; x++)
            {
                for (int y = -radius; y <= radius; y++)
                {
                    int paintDistance = x * x + y * y;
                    if (Math.Abs(paintDistance - targetRadius) <= radius)
                    {
                        PaintBrush(centerX + x, centerY + y);
                    }
                }
            }
            if (InBounds(centerX, centerY))
            {
                PositionX = centerX;
                PositionY = centerY;
            }

        }
        public void DrawRectangle(int dx, int dy, int distance, int width, int height, Token rectangleToken)
        {
            if (dx < -1 || dy < -1 || dx > 1 || dy > 1)
            {
                throw new RuntimeErrorException(rectangleToken, "Invalid directions");
            }
            if (distance < 0) distance = 0;
            if (width < 0) width = 0;
            if (height < 0) height = 0;

            int centerX = PositionX + dy * distance;
            int centerY = PositionY + dx * distance;
            int halfW = width / 2;
            int halfH = height / 2;
            int left = centerX - halfW;
            int right = centerX + halfW;
            int up = centerY - halfH;
            int down = centerY + halfH;
            for (int x = left; x <= right; x++)
            {
                PaintBrush(x, up);
                PaintBrush(x, down); // paint up and down 
            }
            for (int y = up + 1; y <= down; y++)
            {
                PaintBrush(left, y);
                PaintBrush(right, y); // paint sides 
            }
            if (InBounds(centerX, centerY))
            {
                PositionX = centerX;
                PositionY = centerY;
            }

        }
        public void Fill()
        {
            CanvasColor originColor = Canvas[PositionX, PositionY];
            CanvasColor targetColor = BrushColor;
            if (originColor == targetColor || targetColor == CanvasColor.Transparent) return;

            Queue<(int x, int y)> values = new();
            HashSet<(int x, int y)> visited = new();
            values.Enqueue((PositionX, PositionY));
            visited.Add((PositionX, PositionY));
            while (values.Count > 0)
            {
                var (x, y) = values.Dequeue();
                PaintCell(x, y);
                foreach (var (bx, by) in GetBounds(x, y))
                {
                    if (InBounds(bx, by) && !visited.Contains((bx, by)) && Canvas[bx, by] == originColor)
                    {
                        values.Enqueue((bx, by));
                        visited.Add((bx, by));
                    }
                }
            }

        }
        public void PaintBrush(int cx, int cy)
        {
            int half = BrushSize / 2;
            for (int dx = -half; dx <= half; dx++)
            {
                for (int dy = -half; dy <= half; dy++)
                {
                    PaintCell(cx + dx, cy + dy);
                }
            }
        }
        public void PaintCell(int x, int y)
        {
            if (!InBounds(x, y)) return;
            if (BrushColor == CanvasColor.Transparent)
            {
                return;
            }
            Canvas[x, y] = BrushColor;
        }
        public IEnumerable<(int, int)> GetBounds(int x, int y)
        {
            yield return (x + 1, y);
            yield return (x - 1, y);
            yield return (x, y + 1);
            yield return (x, y - 1);
        }
        public CanvasColor GetCanvasColor(int x, int y)
        {
            return Canvas[x, y];
        }
        public CanvasColor[,] GetCanvas()
        {
            var clone = new CanvasColor[CanvasSize, CanvasSize];
            Array.Copy(Canvas, clone, Canvas.Length);
            return clone;
        }
    }

   
}