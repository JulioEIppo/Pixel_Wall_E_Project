using System;
using System.Collections.Generic;


namespace PixelWallE
{
    public interface INativeFunction
    {
        object Invoke(List<object> args, Token token);
        public int Arity { get; }
    }
    public class GetActualXFunction : INativeFunction
    {
        private WallEEngine wallEContext;
        public GetActualXFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public object Invoke(List<object> args, Token token)
        {
            return wallEContext.PositionX;
        }
        public int Arity => 0;
    }
    public class GetActualYFunction : INativeFunction
    {
        private WallEEngine wallEContext;
        public GetActualYFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public object Invoke(List<object> args, Token token)
        {
            return wallEContext.PositionY;
        }
        public int Arity => 0;
    }
    public class GetCanvasSizeFunction : INativeFunction
    {
        private WallEEngine wallEContext;
        public GetCanvasSizeFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public object Invoke(List<object> args, Token token)
        {
            return wallEContext.CanvasSize;
        }
        public int Arity => 0;
    }
    public class GetColorCountFunction : INativeFunction
    {
        public WallEEngine wallEContext;
        public GetColorCountFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public int Arity => 5;
        public object Invoke(List<object> args, Token token)
        {
            if (args[0] is not string colorName || args[1] is not int x1 || args[2] is not int x2 || args[3] is not int y1
                || args[4] is not int y2)
            {
                throw new RuntimeErrorException(token, "Invalid parameters, GetColorCount requires: (string,int,int,int,int)");
            }
            if (!Enum.TryParse<CanvasColor>(colorName, true, out var color) || !Enum.IsDefined(color))
            {
                throw new RuntimeErrorException(token, $"Invalid color : {color}");
            }
            int minX = Math.Min(x1, x2);
            int maxX = Math.Max(x1, x2);
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);

            if (!wallEContext.InBounds(minX, minY) || !wallEContext.InBounds(maxX, maxY))
                return 0;

            int count = 0;
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (wallEContext.GetCanvasColor(x, y) == color)
                        count++;
                }
            }

            return count;
        }
    }
    public class IsBrushColorFunction : INativeFunction
    {
        public WallEEngine wallEContext;
        public IsBrushColorFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public int Arity => 1;
        public object Invoke(List<object> args, Token token)
        {
            if (args[0] is not string colorName)
            {
                throw new RuntimeErrorException(token, "Expected color string");
            }
            if (!Enum.TryParse<CanvasColor>(colorName, out var color) || !Enum.IsDefined(color))
            {
                throw new RuntimeErrorException(token, $"Invalid color : {color}");
            }
            return wallEContext.BrushColor == color ? 1 : 0;
        }
    }
    public class IsBrushSizeFunction : INativeFunction
    {
        public WallEEngine wallEContext;
        public IsBrushSizeFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public int Arity => 1;
        public object Invoke(List<object> args, Token token)
        {
            if (args[0] is not int size)
            {
                throw new RuntimeErrorException(token, $"Expected int parameter");
            }
            return wallEContext.BrushSize == size ? 1 : 0;
        }
    }
    public class IsCanvasColorFunction : INativeFunction
    {
        public WallEEngine wallEContext;
        public IsCanvasColorFunction(WallEEngine context)
        {
            wallEContext = context;
        }
        public int Arity => 3;

        public object Invoke(List<object> args, Token token)
        {
            if (args[0] is not string colorName || args[1] is not int vert || args[2] is not int horizon)
            {
                throw new RuntimeErrorException(token, "Invalid parameters, expected: (string colorName, int vertical, int horizontal)");
            }
            if (!Enum.TryParse<CanvasColor>(colorName, out var color) || !Enum.IsDefined(color))
            {
                throw new RuntimeErrorException(token, $"Invalid color input: {color}");
            }

            int x = wallEContext.PositionX + horizon;
            int y = wallEContext.PositionY + vert;
            if (!wallEContext.InBounds(x, y)) return 0;

            return wallEContext.GetCanvasColor(x, y) == color ? 1 : 0;
        }
    }
}