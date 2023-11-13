
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Net;
using System.Net.Http;

using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections.Specialized;

class Result
{

    /*
     * Complete the 'getAuthorHistory' function below.
     *
     * The function is expected to return a STRING_ARRAY.
     * The function accepts STRING author as parameter.
     *
     * Base urls:
     *   https://jsonmock.hackerrank.com/api/article_users?username=
     *   https://jsonmock.hackerrank.com/api/articles?author=
     *
     */
    private static readonly IAuthorClient _authorClient = new AuthorClient();
    private const string authorBaseUrl = "https://jsonmock.hackerrank.com/api/article_users?username=";
    private const string articleBaseUrl = "https://jsonmock.hackerrank.com/api/articles?author=";

    public static List<string> getAuthorHistory(string author)
    {
        var aboutAuthor = getAuthorAboutDetails($"{authorBaseUrl}{author}");

        var history = GetAuthorHistory(author);

        return history;
    }

    private static List<string> GetAuthorHistory(string author)
    {
        var history = new List<string>();

        var articlesResponse = _authorClient.FetchDataAsync<ArticlesResponse>($"{articleBaseUrl}{author}");

        int? pageCount = articlesResponse?.Result?.total_pages;

        for (int page = 1; page <= pageCount; page++)
        {
            var articles = _authorClient.FetchDataAsync<ArticlesResponse>($"{articleBaseUrl}{author}");

            if (articles.Result?.Data.Count > 0)
                history.AddRange(articles.Result.Data                   
                    .Where(item => !string.IsNullOrEmpty(item.title) || !string.IsNullOrEmpty(item.story_title))
                     .Select(item => !string.IsNullOrEmpty(item.title) ? item.title : item.story_title));
        }

        return history;
    }

    private static List<string?> getAuthorAboutDetails(string url)
    {
        var authorResponse = _authorClient.FetchDataAsync<AuthorResponse>(url);

        if (authorResponse == null || authorResponse.Result?.Data == null)
            return null;

        var authors = authorResponse.Result.Data;

         return authors.Where(a => !string.IsNullOrEmpty(a.about)).Select(a => a.about).ToList();        
    }

}

public interface IAuthorClient
{
    public Task<TResponse?> FetchDataAsync<TResponse>(string url)
         where TResponse : class;

}

public class AuthorClient : IAuthorClient
{
    public async Task<TResponse?> FetchDataAsync<TResponse>(string url)
        where TResponse : class
    {
        using (var httpClient = new HttpClient())
        {
            var response = await httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<TResponse>(jsonContent);
            }

            return null;
        }
    }
}



public class AuthorResponse
{
    public int page { get; set; }
    public int per_page { get; set; }
    public int total { get; set; }
    public int total_pages { get; set; }
    public List<Author> Data { get; set; }
}

public class ArticlesResponse
{
    public int page { get; set; }
    public int per_page { get; set; }
    public int total { get; set; }
    public int total_pages { get; set; }
    public List<Articles> Data { get; set; }
}

public class Articles
{
    public string? title { get; set; }
    public string? author { get; set; }
    public string story_title { get; set; }
}

public class Author
{   
    public string username { get; set; }
    public string? about { get; set; }
}

class Solution
{
    public static void Main(string[] args)
    {
        TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

        string author = Console.ReadLine();

        List<string> result = Result.getAuthorHistory(author);

        textWriter.WriteLine(String.Join("\n", result));

        textWriter.Flush();
        textWriter.Close();
    }
}
