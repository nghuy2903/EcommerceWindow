using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace WPFEcommerceApp
{
    public class AzureStorageAPI
    {
        // Connection string to your Azure Storage account
        private static readonly string connectionString = "YOUR_AZURE_STORAGE_CONNECTION_STRING";
        const string tempJPG = "CreateTempJpg.jpg";
        const string tempIMG = "TempIMG.jpg";

        // Push a local file to Azure Blob Storage
        public static async Task<string> Push(string path, string containerName, string blobName)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            bool createdFile = false;

            // Convert to JPEG if needed
            Image img = new Bitmap(stream);
            if (!img.RawFormat.Equals(ImageFormat.Jpeg))
            {
                using (var bitmap = new Bitmap(img.Width, img.Height))
                {
                    bitmap.SetResolution(img.HorizontalResolution, img.VerticalResolution);
                    using (var graphic = Graphics.FromImage(bitmap))
                    {
                        graphic.Clear(Color.White);
                        graphic.DrawImageUnscaled(img, 0, 0);
                    }
                    bitmap.Save(tempJPG, ImageFormat.Jpeg);
                }
                stream.Close();
                stream = File.Open(tempJPG, FileMode.Open, FileAccess.Read, FileShare.Read);
                createdFile = true;
            }

            // Upload the file
            BlobClient blobClient = container.GetBlobClient(blobName);
            await blobClient.UploadAsync(stream, overwrite: true);
            stream.Close();

            if (createdFile)
            {
                File.Delete(tempJPG);
            }

            return blobClient.Uri.ToString(); // Return URL of uploaded blob
        }

        // Push a file from a BitmapSource (e.g. image)
        public static async Task<string> PushFromImage(
            System.Windows.Media.Imaging.BitmapSource bm,
            string containerName,
            string blobName)
        {

            FileStream stream = new FileStream(tempIMG, FileMode.Create);
            System.Windows.Media.Imaging.BitmapEncoder encoder = new System.Windows.Media.Imaging.JpegBitmapEncoder();
            encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bm));
            encoder.Save(stream);
            stream.Close();

            var res = await Push(tempIMG, containerName, blobName);
            File.Delete(tempIMG);
            return res;
        }

        // Delete a file from Azure Blob Storage
        public static async Task<bool> Delete(string blobUrl)
        {
            Uri blobUri = new Uri(blobUrl);
            string blobName = Path.GetFileName(blobUri.LocalPath);
            string containerName = blobUri.Segments[1].Trim('/');

            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = container.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);
            return true;
        }

        // Check if a blob exists
        public static async Task<bool> Exist(string containerName, string blobName)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = container.GetBlobClient(blobName);

            var exists = await blobClient.ExistsAsync();
            return exists.Value;
        }
    }
}
