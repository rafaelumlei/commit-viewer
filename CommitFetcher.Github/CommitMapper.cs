using CommitViewer.Model.DTOs;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommitFetcher.Github
{
    public static class CommitMapper
    {

        private static CommitDTO MapOne(JObject jobject)
        {
            try
            {
                var author = jobject["commit"]["author"];

                return new CommitDTO()
                {
                    Id = jobject["sha"].Value<string>(),
                    AuthorEmail = author["email"].Value<string>(),
                    AuthorName = author["name"].Value<string>(),
                    Message = jobject["commit"]["message"].Value<string>(),
                    Date = author["date"].Value<DateTime>()
                };
            }
            catch (Exception exp)
            {
                // TODO: skiping commit with unsupported json, add logging!
                return null;
            }
        }

        public static IEnumerable<CommitDTO> Map(JArray jarr)
        {
            return jarr?.Select(t => MapOne(t as JObject)).Where(c => c != null).ToArray();
        }

    }
}
