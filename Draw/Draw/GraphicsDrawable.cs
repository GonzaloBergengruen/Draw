namespace Draw
{
    public class GraphicsDrawable : IDrawable
    {
        public List<PointF> CurrentLine { get; set; } = new List<PointF>();
        public List<List<PointF>> Lines { get; set; } = new List<List<PointF>>();

        public bool ShowArrowAndText { get; set; } = true;

        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // Dibujar el fondo
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(dirtyRect);

            // Dibujar todas las líneas existentes
            foreach (var line in Lines)
            {
                if (line.Count > 1)
                {
                    PathF path = new PathF();
                    path.MoveTo(line[0].X, line[0].Y);

                    foreach (var point in line)
                    {
                        path.LineTo(point);
                    }

                    canvas.StrokeColor = Colors.Black;
                    canvas.StrokeSize = 2;
                    canvas.DrawPath(path);
                }
            }

            // Dibujar la línea actual (en curso)
            if (CurrentLine.Count > 1)
            {
                PathF path = new PathF();
                path.MoveTo(CurrentLine[0].X, CurrentLine[0].Y);

                foreach (var point in CurrentLine)
                {
                    path.LineTo(point);
                }

                canvas.StrokeColor = Colors.Red;
                canvas.StrokeSize = 2;
                canvas.DrawPath(path);
            }

            //Muestra la flecha y el texto
            if (ShowArrowAndText)
            {
                DrawArrowWithText(canvas, dirtyRect);
            }
        }

        private void DrawArrowWithText(ICanvas canvas, RectF dirtyRect)
        {
            float centerX = dirtyRect.Center.X;
            float bottomY = dirtyRect.Bottom - 20;
            float topY = dirtyRect.Top + 20;

            // Dibujar línea de la flecha
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 3;
            canvas.DrawLine(centerX, bottomY, centerX, topY);

            // Dibujar la punta de la flecha
            canvas.DrawLine(centerX, topY, centerX - 10, topY + 20);
            canvas.DrawLine(centerX, topY, centerX + 10, topY + 20);

            // Dibujar el texto perpendicular a la flecha
            canvas.SaveState();
            canvas.Translate(centerX - 20, (bottomY + topY) / 2);
            canvas.Rotate(-90); // Rotar el texto para que sea perpendicular
            canvas.FontColor = Colors.Black;
            canvas.FontSize = 18;
            canvas.DrawString("Escriba en esta dirección", 0, 0, HorizontalAlignment.Center);
            canvas.RestoreState();
        }
    }
}
