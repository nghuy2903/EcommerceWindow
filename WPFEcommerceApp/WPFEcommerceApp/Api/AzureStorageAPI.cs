using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WPFEcommerceApp
{
    public class AzureStorageAPI
    {
        private static readonly string connectionString = "DefaultEndpointsProtocol=https;AccountName=warby;AccountKey=vQfo8DI2qxqpcCUmEckVNUSJfhSGOJA864+f8z024qu3WYE4vAQYkB+FrtzzTq2TEOXgPoSMze/S+ASt7Nxmsg==;EndpointSuffix=core.windows.net";
        const string tempIMG = "TempIMG.jpg";

        public static async Task<string> Push(string filePath, string containerName, string blobName, params string[] child)
        {
            string blobFullName = GenerateBlobName(blobName, child);

            // Create a BlobServiceClient to interact with the Blob service
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get a reference to the container
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(blobFullName);

            // Upload the file to the blob
            using (FileStream uploadFileStream = File.OpenRead(filePath))
            {
                await blobClient.UploadAsync(uploadFileStream, true);
            }

            // Return the URI of the uploaded blob
            return blobClient.Uri.ToString();
        }

        public static async Task<Tuple<bool, string>> PushFromFile(
            string filePath,
            string containerName,
            string blobName,
            string oldBlobUri = null,
            params string[] child)
        {
            if (!File.Exists(filePath))
            {
                return new Tuple<bool, string>(false, "");
            }

            if (oldBlobUri != null)
            {
                await Delete(oldBlobUri);
            }

            var res = await Push(filePath, containerName, blobName, child);
            return new Tuple<bool, string>(true, res);
        }

        public static async Task<string> PushFromImage(
            BitmapSource bitmapSource,
            string containerName,
            string blobName,
            string oldBlobUri = null,
            params string[] child)
        {
            // Save BitmapSource to a temporary file
            using (FileStream stream = new FileStream(tempIMG, FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
            }

            // Delete the old blob if provided
            if (oldBlobUri != null)
            {
                await Delete(oldBlobUri);
            }

            // Upload the image and return the blob URI
            var res = await Push(tempIMG, containerName, blobName, child);
            File.Delete(tempIMG); // Clean up temp file
            return res;
        }

        public static async Task<bool> Exist(string containerName, string blobName)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = container.GetBlobClient(blobName);

            var exists = await blobClient.ExistsAsync();
            return exists.Value;
        }

        public static async Task<bool> Delete(string blobUri)
        {
            try
            {
                // Parse the URI to get container and blob name
                Uri uri = new Uri(blobUri);
                string[] segments = uri.Segments;

                // The container name is the first segment after the initial slash
                string containerName = segments[1].TrimEnd('/');

                // The blob name is everything after the container name
                string blobName = string.Join("", segments.Skip(2));

                // Create clients using the connection string
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Delete the blob
                await blobClient.DeleteIfExistsAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log or handle the exception
                throw new Exception($"Failed to delete blob at {blobUri}", ex);
            }
        }

        private static string GenerateBlobName(string name, params string[] childPaths)
        {
            List<string> paths = new List<string>(childPaths) { name };
            return string.Join("/", paths);
        }
    }
}