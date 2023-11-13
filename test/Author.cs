using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class AuthorResult
{
    const string authorBaseUrl = "https://jsonmock.hackerrank.com/api/article_users?username=";
    const string articleBaseUrl = "https://jsonmock.hackerrank.com/api/articles?author=";

    public async static Task<List<string>> getAuthorHistory(string author)
    {
        var aboutAuthor = await GetAuthorAboutDetailsAsync(authorBaseUrl + author);

        var history = new List<string>();

        await foreach (string title in GetAuthorHistoryAsync(author))
        {
            history.Add(title);
        }

        return history;
    }


    static async IAsyncEnumerable<string> GetAuthorHistoryAsync(string author)
    {   int pageCount = 1;
        ArticlesResponse articlesResponse;

        do
        {
            articlesResponse = await FetchDataAsync<ArticlesResponse>(articleBaseUrl + author + "&page=" + pageCount);

            if (articlesResponse?.Data != null)
            {
                foreach (var item in articlesResponse.Data)
                {
                    if (!string.IsNullOrEmpty(item.title) || !string.IsNullOrEmpty(item.story_title))
                    {
                        yield return !string.IsNullOrEmpty(item.title) ? item.title : item.story_title;
                    }
                }
            }

            pageCount++;
        } while (pageCount <= articlesResponse?.TotalPages);
    }

    static async Task<List<Author>> GetAuthorAboutDetailsAsync(string url)
    {
        var authorResponse = await FetchDataAsync<AuthorResponse>(url);
        return authorResponse?.Data.FindAll(a => !string.IsNullOrEmpty(a.about));
    }

    static async Task<T> FetchDataAsync<T>(string url) where T : class
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonContent);
            }

            return null;
        }
    }


    public class AuthorResponse
    {
        public List<Author> Data { get; set; }
    }

    public class ArticlesResponse
    {
        public List<Articles> Data { get; set; }
        public int TotalPages { get; set; }
    }

    public class Articles
    {
        public string title { get; set; }
        public string story_title { get; set; }
    }

    public class Author
    {
        public string about { get; set; }
    }
}