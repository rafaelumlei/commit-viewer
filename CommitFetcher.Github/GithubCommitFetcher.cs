using CommitFetcher.Interfaces;
using CommitFetcher.Interfaces.Exceptions;
using CommitViewer.Logger.Interfaces;
using CommitViewer.Model.DTOs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CommitFetcher.Github
{
    public class GithubCommitFetcher : ICommitFetcher<CommitDTO>
    {
        private readonly HttpClient githubClient;

        private readonly ICommitViewerLog logger;

        public GithubCommitFetcher(string gitHubBaseEndpoint,
            int timeoutMs,
            ICommitViewerLog logger)
        {
            this.githubClient = new HttpClient();
            this.githubClient.BaseAddress = new Uri(gitHubBaseEndpoint);
            this.githubClient.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            this.githubClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            this.githubClient.DefaultRequestHeaders.Add("User-Agent", $"{nameof(GithubCommitFetcher)}App");
            this.logger = logger;
            // to be able to perform https requests
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            ServicePointManager.Expect100Continue = false;
        }

        private (string owner, string repo) ParseGithubUrl(string url)
        {
            Uri uri = new Uri(url);
            string path = Regex.Replace(uri.PathAndQuery, ".git", "", RegexOptions.IgnoreCase);
            var ownerAndRepo = path.Split(new char[] { '/' }).Skip(1).ToArray(); 

            if (ownerAndRepo.Length != 2)
            {
                throw new ArgumentException($"Github url invalid. Impossible to get owner & repo");
            }

            return (ownerAndRepo[0], ownerAndRepo[1]);
        }

        public async Task<IEnumerable<CommitDTO>> GetCommits(string url, int page = 0, int per_page = 10)
        {
            try
            {
                var repoInfo = ParseGithubUrl(url);

                string path = $"/repos/{repoInfo.owner}/{repoInfo.repo}/commits?page={page}&per_page={per_page}";

                var response = await this.githubClient.GetAsync(path).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var json = JsonConvert.DeserializeObject<JArray>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
                    return CommitMapper.Map(json);
                }
                else
                {
                    logger.Error($"Insuccess status code while getting commits from Github. StatusCode: {response.StatusCode}");
                    throw new CommitFetchingOperationException($"Insuccess status code while getting commits from Github. StatusCode: {response.StatusCode}");
                }
            }
            catch (Exception exp)
            {
                logger.Error("Exception while getting commits from Github.");
                throw new CommitFetchingOperationException($"Exception while getting commits from Github.", exp);
            }
        }
    }
}
