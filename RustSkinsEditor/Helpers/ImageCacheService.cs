using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RustSkinsEditor.Helpers
{
    public static class ImageCacheService
    {
        private static readonly HttpClient _http = new();
        private static readonly SemaphoreSlim _semaphore = new(3); // Limit concurrent downloads to 3
        private static readonly ConcurrentDictionary<string, Task<BitmapImage>> _loadingTasks = new();
        private static volatile bool _cancelNewRequests = false;

        public static void CancelAll() => _cancelNewRequests = true;
        public static void ResetCancel() => _cancelNewRequests = false;

        /// <summary>
        /// Downloads and caches an image from the given URI using the provided file name.
        /// </summary>
        /// <param name="uri">The source URI of the image.</param>
        /// <param name="saveFileName">The file name (without extension) to save the image as.</param>
        public static async Task<BitmapImage> GetImageAsync(Uri uri, string saveFileName)
        {
            if (uri == null || string.IsNullOrWhiteSpace(saveFileName))
                return null;

            if (_cancelNewRequests)
                return null;

            string urlKey = uri.ToString();

            try
            {
                string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Cache");
                Directory.CreateDirectory(cacheDir);

                string filePath = Path.Combine(cacheDir, saveFileName + ".png");

                // Already cached → load directly
                if (File.Exists(filePath))
                    return LoadFromFile(filePath);

                // Prevent duplicate concurrent downloads for the same URL
                return await _loadingTasks.GetOrAdd(urlKey, async _ =>
                {
                    await _semaphore.WaitAsync();
                    try
                    {
                        // Stop if cancel was triggered while waiting
                        if (_cancelNewRequests)
                            return null;

                        using var localCts = new CancellationTokenSource();
                        var token = localCts.Token;

                        // Download raw bytes
                        var bytes = await _http.GetByteArrayAsync(uri, token);

                        // Decode and save
                        var bmp = DecodeImage(bytes);
                        if (bmp != null)
                        {
                            await SaveAsPngAsync(bmp, filePath);
                            return LoadFromFile(filePath);
                        }

                        return null;
                    }
                    catch (OperationCanceledException)
                    {
                        return null;
                    }
                    catch
                    {
                        return null;
                    }
                    finally
                    {
                        _semaphore.Release();
                        _loadingTasks.TryRemove(urlKey, out Task<BitmapImage> _);
                    }
                });
            }
            catch
            {
                return null;
            }
        }

        private static BitmapImage DecodeImage(byte[] bytes)
        {
            try
            {
                using var ms = new MemoryStream(bytes);
                var bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch
            {
                return null;
            }
        }

        private static async Task SaveAsPngAsync(BitmapImage bmp, string path)
        {
            try
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                using var ms = new MemoryStream();
                encoder.Save(ms);
                ms.Position = 0;

                using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None);
                await ms.CopyToAsync(fs);
            }
            catch
            {
                // Ignore failed saves (e.g., disk locked)
            }
        }

        private static BitmapImage LoadFromFile(string path)
        {
            if (!File.Exists(path))
                return null;

            try
            {
                var bmp = new BitmapImage();
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                bmp.BeginInit();
                bmp.CacheOption = BitmapCacheOption.OnLoad;
                bmp.StreamSource = fs;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch
            {
                return null;
            }
        }
    }
}
