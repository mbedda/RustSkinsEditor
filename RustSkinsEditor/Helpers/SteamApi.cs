using RustSkinsEditor.Models;
using Steam.Models;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RustSkinsEditor.Helpers
{
    public class SteamApi
    {
        public static async Task<IReadOnlyCollection<PublishedFileDetailsModel>> GetPublishedFileDetailsAsync(string steamApiKey, List<ulong> skinList)
        {
            var webInterfaceFactory = new SteamWebInterfaceFactory(steamApiKey);
            var steamInterface = webInterfaceFactory.CreateSteamWebInterface<SteamRemoteStorage>(new HttpClient());

            skinList.Remove(0);

            var publishedFileDataResponse = await steamInterface.GetPublishedFileDetailsAsync((uint)skinList.Count(), skinList);
            return publishedFileDataResponse.Data;
        }

        private static readonly HttpClient client = new HttpClient();
        public static async Task<SteamCollectionWebResponse> GetCollectionDetailsAsync(List<ulong> collectionList)
        {
            try
            {

                var values = new Dictionary<string, string>
                  {
                      { "collectioncount", collectionList.Count()+"" }
                  };

                for (int i = 0; i < collectionList.Count; i++)
                {
                    values.Add($"publishedfileids[{i}]", collectionList[0].ToString());
                }

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://api.steampowered.com/ISteamRemoteStorage/GetCollectionDetails/v1/", content);

                var responseString = await response.Content.ReadAsStringAsync();

                SteamCollectionWebResponse result = Common.DeserializeJSONString<SteamCollectionWebResponse>(responseString);

                return result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
