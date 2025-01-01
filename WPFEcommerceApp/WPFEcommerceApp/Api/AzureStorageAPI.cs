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
        // It's recommended to store the connection string in a more secure way (e.g., environment variables or app settings)
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

    // Set the content type based on the file extension
    BlobHttpHeaders headers = new BlobHttpHeaders();
    string extension = Path.GetExtension(filePath).ToLowerInvariant();
    if (extension == ".jpg" || extension == ".jpeg")
    {
        headers.ContentType = "image/jpeg";
    }
    else if (extension == ".png")
    {
        headers.ContentType = "image/png";
    }
    else if (extension == ".gif")
    {
        headers.ContentType = "image/gif";
    }
    else
    {
        headers.ContentType = "application/octet-stream"; // Default content type
    }

    // Upload the file to the blob and set the content type
    using (FileStream uploadFileStream = File.OpenRead(filePath))
    {
        await blobClient.UploadAsync(uploadFileStream, true);
        await blobClient.SetHttpHeadersAsync(headers); // Set the content type here
    }

    // Return the URI of the uploaded blob
    return blobClient.Uri.ToString();
}


        // Method to upload a file and replace the old blob if it exists
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
            // Add a unique identifier to the blob name
            blobName = $"{Path.GetFileNameWithoutExtension(blobName)}_{Guid.NewGuid()}{Path.GetExtension(blobName)}";

            // Rest of the method remains unchanged
            string tempFilePath = Path.ChangeExtension(tempIMG, ".jpg");
            using (FileStream stream = new FileStream(tempFilePath, FileMode.Create))
            {
                BitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
                encoder.Save(stream);
            }

            if (oldBlobUri != null)
            {
                await Delete(oldBlobUri);
            }

            var res = await Push(tempFilePath, containerName, blobName, child);
            File.Delete(tempFilePath);
            return res;
        }


        public static async Task<bool> Exist(string containerName, string blobName)
        {
            try
            {
                // Validate container and blob names
                if (string.IsNullOrWhiteSpace(containerName) || string.IsNullOrWhiteSpace(blobName))
                {
                    throw new ArgumentException("Container name or blob name cannot be null or empty.");
                }

                // Initialize container and blob clients
                BlobContainerClient containerClient = new BlobContainerClient(connectionString, containerName);
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Check blob existence
                BlobProperties properties = await blobClient.GetPropertiesAsync();
                return true;
            }
            catch (Azure.RequestFailedException ex) when (ex.Status == 404)
            {
                // Blob does not exist
                return false;
            }
            catch (Exception ex)
            {
                // Log unexpected errors (for production, use a logging framework)
                Console.WriteLine($"Error checking blob existence: {ex.Message}");
                throw;
            }
        }

        public static  async Task<bool> CheckIfBlobExists(string blobUri)
        {
            var blobClient = new BlobClient(new Uri(blobUri));
            return await blobClient.ExistsAsync();
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
                return false;
            }
        }

        private static string GenerateBlobName(string name, params string[] childPaths)
        {
            string uniqueId = Guid.NewGuid().ToString(); 
            string newName = Path.GetFileNameWithoutExtension(name) + "_" + uniqueId + Path.GetExtension(name);
            return string.Join("/", childPaths.Append(newName));
        }

    }
}
