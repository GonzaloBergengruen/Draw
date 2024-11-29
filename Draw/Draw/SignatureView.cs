using SkiaSharp;

namespace Draw
{
    public class SignatureView : GraphicsView
    {
        GraphicsDrawable graphicsDrawable = null;

        public SignatureView()
        {
            graphicsDrawable = new GraphicsDrawable();
            this.Drawable = graphicsDrawable;
            this.StartInteraction += SignatureView_StartInteraction;
            this.DragInteraction += SignatureView_DragInteraction;
            this.EndInteraction += SignatureView_EndInteraction;
        }

        private void SignatureView_StartInteraction(object sender, TouchEventArgs e)
        {
            graphicsDrawable.ShowArrowAndText = false;
            graphicsDrawable.CurrentLine = new List<PointF> { e.Touches[0] };
            this.Invalidate();
        }

        private void SignatureView_DragInteraction(object sender, TouchEventArgs e)
        {
            graphicsDrawable.CurrentLine.Add(e.Touches[0]);
            this.Invalidate();
        }

        private void SignatureView_EndInteraction(object sender, TouchEventArgs e)
        {
            if (graphicsDrawable.CurrentLine.Count > 0)
            {
                graphicsDrawable.Lines.Add(new List<PointF>(graphicsDrawable.CurrentLine));
            }

            graphicsDrawable.CurrentLine.Clear();
            this.Invalidate();
        }

        // Método para exportar el contenido como imagen
        public byte[] ExportAsImage(int width, int height)
        {
            using var surface = SKSurface.Create(new SKImageInfo(width, height));
            var canvas = surface.Canvas;

            // Rellena el fondo
            canvas.Clear(SKColors.White);

            // Dibuja las líneas
            foreach (var line in graphicsDrawable.Lines)
            {
                using var paint = new SKPaint
                {
                    Style = SKPaintStyle.Stroke,
                    Color = SKColors.Black,
                    StrokeWidth = 2,
                    IsAntialias = true
                };

                var path = new SKPath();
                if (line.Count > 0)
                {
                    path.MoveTo(line[0].X, line[0].Y);
                    foreach (var point in line)
                    {
                        path.LineTo(point.X, point.Y);
                    }
                }
                canvas.DrawPath(path, paint);
            }

            // Exporta la imagen como PNG
            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }
    }

}
