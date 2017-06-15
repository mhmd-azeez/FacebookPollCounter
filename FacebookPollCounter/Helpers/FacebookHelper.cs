using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace FacebookPollCounter.Helpers
{
    public static class FacebookHelper
    {
        static FacebookHelper()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_baseApiUrl);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        private static HttpClient _client;
        private static string _baseApiUrl = "https://graph.facebook.com/v2.9/";
        private static Regex _numberRegex = new Regex(@"([\d٠١٢٣٤٥٦٧٨٩]+)", RegexOptions.Compiled);

        public static async Task<PagedList<Comment>> GetComments(string token, string postId, string from, string after = null, int limit = 250)
        {
            var requestUrl = $"{postId}/comments?access_token={token}&total_count=1&order=chronological&filter=stream&summary=1&limit={limit}{(after == null ? string.Empty : "&after="+after)}";

            var response = await _client.GetAsync(requestUrl).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<PagedList<Comment>>(json);
            }

            return null;
        }

        public static async Task<string> GetPostIdFromUrl(string token, string url)
        {
            var parts = url.Replace("https://www.facebook.com/", string.Empty).Split('/');
            var pageName = parts.First();
            string postId;
            
            if (url.Contains("/posts/"))
            {
                // url format: https://www.facebook.com/{page_name}/posts/{post_id}
                postId = parts.Last().Split(new char[] { ':', '?' }).First();
            }
            else if (url.Contains("/photos/") || url.Contains("/vidoes/"))
            {
                // url format: https://www.facebook.com/{page_name}/{photos|vidoes}/pcb.812370652242430/{post_id}/?type=3&theater
                postId = parts[3].Split(new char[] { ':', '?' }).First();
            }
            else
            {
                MessageBox.Show("Unknown Url format, please make sure it's from a facebook Page");
                return null;
            }

            var requestUrl = $"{pageName}?access_token={token}";

            var response = await _client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var creator = JsonConvert.DeserializeObject<Item>(json);

                return $"{creator.Id}_{postId}";
            }

            return null;
        }
        
        public static string GetVotes(string comment)
        {
            var matches = _numberRegex.Matches(comment);
            return string.Join(",", matches.OfType<Match>().Select(m => HindiToArabicNumbers(m.Value)).ToArray());
        }

        private static string HindiToArabicNumbers(string number)
        {
            string arabicNumber = string.Empty;
            foreach (var c in number)
            {
                if (char.IsDigit(c))
                {
                    arabicNumber += char.GetNumericValue(c);
                }
                else
                {
                    arabicNumber += c;
                }
            }

            return arabicNumber;
        }
    }
}
