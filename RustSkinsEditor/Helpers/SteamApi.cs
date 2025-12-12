using RustSkinsEditor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace RustSkinsEditor.Helpers
{
    public class SteamApi
    {
        public static async Task<SteamPublishedFileResponse> GetPublishedFileDetailsAsync(List<ulong> skinList)
        {
            try
            {
                if (skinList.Contains(0))
                    skinList.Remove(0);

                if (skinList.Count == 0)
                    return null;

                var values = new Dictionary<string, string>
                {
                    { "itemcount", skinList.Count()+"" }
                };

                for (int i = 0; i < skinList.Count; i++)
                {
                    values.Add($"publishedfileids[{i}]", skinList[i].ToString());
                }

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://api.steampowered.com/ISteamRemoteStorage/GetPublishedFileDetails/v1/", content);

                var responseString = await response.Content.ReadAsStringAsync();

                SteamPublishedFileResponse result = Common.DeserializeJSONString<SteamPublishedFileResponse>(responseString);

                return result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
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

        public static async Task<RustDLCResponse> GetDLCs()
        {
            try
            {
                var response = await client.GetAsync("https://api.rusthelp.com/v1/facepunch/skins");

                var responseString = await response.Content.ReadAsStringAsync();

                RustDLCResponse result = Common.DeserializeJSONString<RustDLCResponse>(responseString);

                return result;
            }
            catch (HttpRequestException)
            {
                return null;
            }
        }
    }
}
