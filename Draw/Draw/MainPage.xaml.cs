#if ANDROID
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
            // Verifica permisos para Android 10 o inferior
            if (DeviceInfo.Version.Major < 11 && !await CheckAndRequestStoragePermissionAsync())
            {
                await DisplayAlert("Permiso Denegado", "No se puede guardar la imagen sin permiso de almacenamiento.", "OK");
                return;
            }

            // Guarda en la carpeta de imágenes usando MediaStore en Android 11+
            
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
    }
}
#endif