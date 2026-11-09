using Azure.Storage.Blobs;

using Azure.Storage.Blobs.Models;

namespace RaceDayAP1.Services

{

    public class AzureBlobStorageService

    {

        private readonly BlobServiceClient _blobServiceClient;

        public AzureBlobStorageService(

            BlobServiceClient blobServiceClient)

        {

            _blobServiceClient = blobServiceClient;

        }

        public async Task<string> UploadImageAsync(

            IFormFile image,

            string containerName,

            string? fileName = null)

        {

            if (image == null || image.Length == 0)

            {

                throw new ArgumentException(

                    "Please select an image.");

            }

            string[] allowedExtensions =

                { ".jpg", ".jpeg", ".png", ".webp" };

            string extension =

                Path.GetExtension(image.FileName)

                    .ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))

            {

                throw new ArgumentException(

                    "Only JPG, JPEG, PNG and WEBP images are allowed.");

            }

            if (image.Length > 5 * 1024 * 1024)

            {

                throw new ArgumentException(

                    "Image size cannot exceed 5 MB.");

            }

            if (containerName != "event-banners" &&

                containerName != "profile-pictures")

            {

                throw new ArgumentException(

                    "Invalid storage container.");

            }

            var containerClient =

                _blobServiceClient.GetBlobContainerClient(

                    containerName);

            await containerClient.CreateIfNotExistsAsync(

                PublicAccessType.None);

            string blobName =

                string.IsNullOrWhiteSpace(fileName)

                    ? $"{Guid.NewGuid()}{extension}"

                    : fileName;

            var blobClient =

                containerClient.GetBlobClient(blobName);

            string contentType = extension switch

            {

                ".jpg" or ".jpeg" => "image/jpeg",

                ".png" => "image/png",

                ".webp" => "image/webp",

                _ => "application/octet-stream"

            };

            await blobClient.UploadAsync(

                image.OpenReadStream(),

                new BlobUploadOptions

                {

                    HttpHeaders = new BlobHttpHeaders

                    {

                        ContentType = contentType

                    },

                    Conditions = null

                });

            return blobClient.Uri.ToString();

        }

        public async Task<(Stream? Content, string? ContentType)>

            GetEventBannerAsync(string eventId)

        {

            return await GetImageAsync(

                "event-banners", eventId);

        }

        public async Task<(Stream? Content, string? ContentType)>

            GetProfilePictureAsync(string participantId)

        {

            return await GetImageAsync(

                "profile-pictures", participantId);

        }

        private async Task<(Stream? Content, string? ContentType)>

            GetImageAsync(string containerName, string blobName)

        {

            var containerClient =

                _blobServiceClient.GetBlobContainerClient(

                    containerName);

            var blobClient =

                containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync())

            {

                return (null, null);

            }

            var response =

                await blobClient.DownloadStreamingAsync();

            return (

                response.Value.Content,

                response.Value.Details.ContentType

            );

        }

    }

}