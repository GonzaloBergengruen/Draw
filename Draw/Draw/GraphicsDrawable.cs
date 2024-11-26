namespace Draw
{
    public class GraphicsDrawable : IDrawable
    {
        public List<PointF> CurrentLine { get; set; } = new List<PointF>();
        public List<List<PointF>> Lines { get; set; } = new List<List<PointF>>();

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
        }
    }

}
