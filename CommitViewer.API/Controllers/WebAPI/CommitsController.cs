using CommitFetcher.Interfaces;
using CommitViewer.Logger.Interfaces;
using CommitViewer.Model.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly ICommitFetcher<CommitDTO> commitsFetcher;

        public CommitsController(/*/ICommitFetcher<CommitDTO> commitsFetcher,
            ICommitViewerLog commitViewerLog*/)
        {
            //this.commitsFetcher = commitsFetcher;
            //this.commitViewerLog = commitViewerLog;
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
            else if (Uri.IsWellFormedUriString(url ?? string.Empty, UriKind.Absolute))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, $"Github project {nameof(url)} is not a valid.");
            }

            try
            {
                return Request.CreateResponse(await this.commitsFetcher.GetCommits(url, page, per_page));
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
                return Request.CreateResponse(await this.commitsFetcher.GetCommits(url, page, per_page));
            }
            catch (Exception exp)
            {
                this.commitViewerLog.Error($"Internal error fetching commits for {url}, {owner}, {repo}, {page} and {per_page}. Check inner exception for details", exp);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Internal error fetching commits");
            }
        }

    }
}
