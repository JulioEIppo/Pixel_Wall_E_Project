using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PixelWallE
{
    public class CanvasLogic
    {
        private readonly MainWindow mainWindow;
        public CanvasLogic(MainWindow window)
        {
            mainWindow = window;
            WallE.SetCanvasSize(window.DefaultSize);
        }
        public void ResizeCanvas(int newSize)
        {
            WallE.SetCanvasSize(newSize);
            UpdateCanvas();
        }
        public void UpdateCanvas()
        {
            var canvasData = WallE.GetCanvas();
            mainWindow.UpdateCanvas(canvasData);
        }
        public async Task ShowError(string message)
        {
            await mainWindow.ShowMessage("Error", message);
        }
        public async Task RunCode(string code)
        {
            try
            {
                WallE.RunCode(code);
                if (WallE.HadSyntaxError || WallE.HadRuntimeError)
                {
                    string errors = "";
                    foreach (string error in WallE.Errors)
                    {
                        errors = errors + error + "\n";
                    }
                    await ShowError(errors);
                    WallE.Errors.Clear();
                }
                UpdateCanvas();

            }
            catch (Exception ex)
            {
                await ShowError(ex.Message);
            }
        }
    }
}