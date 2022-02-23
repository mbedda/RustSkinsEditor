using Steam.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RustSkinsEditor.Helpers
{
    public class SteamApi
    {
        public static async Task<IReadOnlyCollection<PublishedFileDetailsModel>> GetPublishedFileDetailsAsync(string steamApiKey, List<ulong> skinList)
        {
            var webInterfaceFactory = new SteamWebInterfaceFactory(steamApiKey);
            var steamInterface = webInterfaceFactory.CreateSteamWebInterface<SteamRemoteStorage>(new HttpClient());

            var publishedFileDataResponse = await steamInterface.GetPublishedFileDetailsAsync((uint)skinList.Count(), skinList);
            return publishedFileDataResponse.Data;
        }
    }
}
