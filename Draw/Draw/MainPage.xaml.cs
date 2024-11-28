#if ANDROID
using Android.Content;
using Microsoft.Maui.Graphics.Platform;

namespace Draw
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSaveImageClicked(object sender, EventArgs e)
        {
            try
            {
                int width = 500; // Ancho de la imagen
                int height = 500; // Alto de la imagen

                // Obtén los datos de la imagen como bytes
                var imageData = (SignatureCanvas as SignatureView)?.ExportAsImage(width, height);

                if (imageData != null)
                {
                    //Rota imagen 90° sentido horario
                    imageData = RotateImage(imageData, 90);

                    // Verifica permisos para Android 10 o inferior
                    if (DeviceInfo.Version.Major < 11 && !await CheckAndRequestStoragePermissionAsync())
                    {
                        await DisplayAlert("Permiso Denegado", "No se puede guardar la imagen sin permiso de almacenamiento.", "OK");
                        return;
                    }

                    // Guarda en la carpeta de imágenes usando MediaStore en Android 11+
                    if (DeviceInfo.Version.Major >= 11)
                    {
                        await SaveImageToPicturesFolderAsync(imageData);
                    }
                    else
                    {

                        // Guarda directamente en la carpeta de imágenes para Android < 11
                        var picturesPath = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryPictures)?.AbsolutePath;

                        if (!Directory.Exists(picturesPath))
                        {
                            Directory.CreateDirectory(picturesPath);
                        }

                        var fileName = Path.Combine(picturesPath, "signature.png");
                        File.WriteAllBytes(fileName, imageData);
                        await DisplayAlert("Éxito", $"Imagen guardada en: {fileName}", "OK");
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo capturar el canvas.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar la imagen: {ex.Message}", "OK");
            }
        }


        private void OnClearCanvasClicked(object sender, EventArgs e)
        {
            // Limpiar todas las líneas dibujadas
            var drawable = (SignatureCanvas.Drawable as GraphicsDrawable);
            if (drawable != null)
            {
                drawable.Lines.Clear();
                drawable.CurrentLine.Clear();
                SignatureCanvas.Invalidate();
            }
        }

        private async Task<bool> CheckAndRequestStoragePermissionAsync()
        {
            var status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            return status == PermissionStatus.Granted;
        }

        private async Task SaveImageToPicturesFolderAsync(byte[] imageData)
        {
            try
            {
                var resolver = Android.App.Application.Context.ContentResolver;

                var values = new ContentValues();
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.DisplayName, $"signature_{DateTime.Now:yyyyMMdd_HHmmss}.png");
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.MimeType, "image/png");
                values.Put(Android.Provider.MediaStore.Images.Media.InterfaceConsts.RelativePath, "Pictures/Signatures");

                var uri = resolver.Insert(Android.Provider.MediaStore.Images.Media.ExternalContentUri, values);

                if (uri != null)
                {
                    using (var stream = resolver.OpenOutputStream(uri))
                    {
                        await stream.WriteAsync(imageData, 0, imageData.Length);
                    }

                    await DisplayAlert("Éxito", "Imagen guardada en la carpeta Imágenes.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "No se pudo guardar la imagen en MediaStore.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo guardar la imagen: {ex.Message}", "OK");
            }
        }

        private byte[] RotateImage(byte[] imageData, float angle)
        {
            // Crea un bitmap desde los bytes de la imagen
            using var originalBitmap = Android.Graphics.BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            // Calcula el nuevo tamaño del bitmap después de la rotación
            int width = originalBitmap.Width;
            int height = originalBitmap.Height;

            var matrix = new Android.Graphics.Matrix();
            matrix.PostRotate(angle);

            // Crea un nuevo bitmap rotado
            using var rotatedBitmap = Android.Graphics.Bitmap.CreateBitmap(originalBitmap, 0, 0, width, height, matrix, true);

            using var stream = new MemoryStream();
            rotatedBitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, stream);

            return stream.ToArray();
        }

    }
}
#endif