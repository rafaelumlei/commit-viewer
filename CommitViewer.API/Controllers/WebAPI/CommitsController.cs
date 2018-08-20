using CommitFetcher.Interfaces;
using CommitViewer.Logger.Interfaces;
using CommitViewer.Model.DTOs;
using Polly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace CommitViewer.API.Controllers.WebAPI
{
    [RoutePrefix("api")]
    public class CommitsController : ApiController
    {
        private readonly ICommitViewerLog commitViewerLog;

        private readonly ICommitFetcher<CommitDTO> defaultCommitFetcher;

        private readonly ICommitFetcher<CommitDTO> fallbackCommitFetcher;

        private readonly IAsyncPolicy baseResiliencePolicy;

        /// <summary>
        /// Constructor  with two injectable fetchers (default & fallback) and 
        /// a baseline resilience policy (timeout & circuit breaker) 
        /// </summary>
        /// <param name="defaultCommitFetcher"></param>
        /// <param name="fallbackCommitFetcher"></param>
        /// <param name="commitViewerLog"></param>
        /// <param name="baseResiliencePolicy"></param>
        public CommitsController(ICommitFetcher<CommitDTO> defaultCommitFetcher,
            ICommitFetcher<CommitDTO> fallbackCommitFetcher,
            IAsyncPolicy baseResiliencePolicy,
            ICommitViewerLog commitViewerLog)
        {
            this.defaultCommitFetcher = defaultCommitFetcher;
            this.fallbackCommitFetcher = fallbackCommitFetcher;
            this.commitViewerLog = commitViewerLog;
            this.baseResiliencePolicy = baseResiliencePolicy;
        }


        private async Task<IEnumerable<CommitDTO>> GetCommitsImpl(string url, int page = 0, int per_page = 10)
        {
            // first uses the default, if any exception arises, the circuit is open or
            // timeout has been reached fallsback to the fallbackCommitFetcher
            var fallback = Policy<IEnumerable<CommitDTO>>
                .Handle<Exception>()
                .FallbackAsync(async (ct) =>
                {
                    return await this.fallbackCommitFetcher.GetCommits(url, page, per_page);
                });

            var commits = await fallback.WrapAsync(this.baseResiliencePolicy)
                .ExecuteAsync(async () =>
                {
                    return await this.defaultCommitFetcher.GetCommits(url, page, per_page);
                });

            return commits;
        }

        [HttpGet]
        [Route("commits")]
        [ResponseType(typeof(IEnumerable<CommitDTO>))]
        public async Task<HttpResponseMessage> GetCommits(string url, int page = 0, int per_page = 10)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"Invalid request, parameter {nameof(url)} was not provided.");
            }

            try
            {
                return Request.CreateResponse(await this.GetCommitsImpl(url, page, per_page));
            }
            catch (Exception exp)
            {
                this.commitViewerLog.Error($"Internal error fetching commits for {url}, {page} and {per_page}. Check inner exception for details", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal error fetching commits");
            }
        }

        [HttpGet]
        [Route("repositories/{owner}/{repo}/commits")]
        [ResponseType(typeof(IEnumerable<CommitDTO>))]
        public async Task<HttpResponseMessage> GetCommits(string owner, string repo, int page = 0, int per_page = 10)
        {

            if (string.IsNullOrWhiteSpace(owner))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"Invalid request, parameter {nameof(owner)} was not provided.");
            }
            else if (string.IsNullOrWhiteSpace(repo))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"Invalid request, parameter {nameof(repo)} was not provided.");
            }

            string url = $"{Properties.Settings.Default.GithubBaseUrl}/{owner}/{repo}.git";

            try
            {
                return Request.CreateResponse(await this.GetCommitsImpl(url, page, per_page));
            }
            catch (Exception exp)
            {
                this.commitViewerLog.Error($"Internal error fetching commits for {url}, {owner}, {repo}, {page} and {per_page}. Check inner exception for details", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal error fetching commits");
            }
        }

    }
}
