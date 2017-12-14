using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using Git.hub.util;

namespace Git.hub
{
    public class Repository
    {
        public string Name { get; internal set; }
        public string Description { get; internal set; }
        public string Homepage { get; internal set; }
        public User Owner { get; internal set; }
        public bool Fork { get; internal set; }
        public int Forks { get; internal set; }
        public bool Private { get; internal set; }
        public Organization Organization { get; internal set; }

        private Repository _Parent;
        public Repository Parent
        {
            get
            {
                if (!Detailed)
                    throw new NotSupportedException();
                return _Parent;
            }
            private set
            {
                _Parent = value;
            }
        }

        /// <summary>
        /// Read-only clone url
        /// git://github.com/{user}/{repo}.git
        /// </summary>
        public string GitUrl { get; internal set; }

        /// <summary>
        /// Read/Write clone url via SSH
        /// git@github.com/{user}/{repo.git}
        /// </summary>
        public string SshUrl { get; internal set; }

        /// <summary>
        /// Read/Write clone url via HTTPS
        /// https://github.com/{user}/{repo}.git
        /// </summary>
        public string CloneUrl { get; internal set; }

        internal RestClient _client;

        /// <summary>
        /// true if fetched from github.com/{user}/{repo}, false if from github.com/{user}
        /// </summary>
        public bool Detailed { get; internal set; }

        /// <summary>
        /// Lists all branches
        /// </summary>
        /// <remarks>Not really sure if that's even useful, mind the 'git branch'</remarks>
        /// <returns>list of all branches</returns>
        public IList<Branch> GetBranches()
        {
            RestRequest request = new RestRequest("/repos/{user}/{repo}/branches");
            request.AddUrlSegment("user", Owner.Login);
            request.AddUrlSegment("repo", Name);

            return _client.GetList<Branch>(request);
        }

        /// <summary>
        /// Gets the commits.
        /// </summary>
        /// <returns>list of all commits on that repository</returns>
        public IList<PullRequestCommit> GetCommits()
        {
            RestRequest request = new RestRequest("/repos/{user}/{repo}/commits");
            request.AddUrlSegment("user", Owner.Login);
            request.AddUrlSegment("repo", Name);

            return _client.GetList<PullRequestCommit>(request);
        }

        public SearchCommit GetCommitsByAuthorName(string username)
        {
            RestRequest request = new RestRequest(string.Format("/search/commits?q=repo:{0}/{1}+author:{2}", Owner.Login, Name, username));

            // TODO: remove later
            request.AddHeader("Accept", "application/vnd.github.cloak-preview+json");

            return _client.Get<SearchCommit>(request).Data;
        }

        public SearchIssue GetCommitsByCommitterName(string username)
        {
            RestRequest request = new RestRequest(string.Format("/search/commits?q=repo:{0}/{1}+committer:{2}", Owner.Login, Name, username));

            return _client.Get<SearchIssue>(request).Data;
        }

        public IList<Issue> GetIssues()
        {
            RestRequest request = new RestRequest("/repos/{user}/{repo}/issues");
            request.AddUrlSegment("user", Owner.Login);
            request.AddUrlSegment("repo", Name);

            return _client.GetList<Issue>(request);
        }

        public SearchIssue GetIssuesByAuthorName(string username)
        {
            RestRequest request = new RestRequest(string.Format("/search/issues?q=type:issue+repo:{0}/{1}+author:{2}", Owner.Login, Name, username));

            return _client.Get<SearchIssue>(request).Data;
        }

        public SearchIssue GetIssuesByAssigneeName(string username)
        {
            RestRequest request = new RestRequest(string.Format("/search/issues?q=type:issue+repo:{0}/{1}+assignee:{2}", Owner.Login, Name, username));

            return _client.Get<SearchIssue>(request).Data;
        }

        public IList<User> GetCollaborators()
        {
            RestRequest request = new RestRequest("/repos/{user}/{repo}/collaborators");
            request.AddUrlSegment("user", Owner.Login);
            request.AddUrlSegment("repo", Name);

            return _client.GetList<User>(request);
        }

        /// <summary>
        /// Lists all open pull requests
        /// </summary>
        /// <returns>llist of all open pull requests</returns>
        public IList<PullRequest> GetPullRequests()
        {
            var request = new RestRequest("/repos/{user}/{repo}/pulls");
            request.AddUrlSegment("user", Owner.Login);
            request.AddUrlSegment("repo", Name);

            var list = _client.GetList<PullRequest>(request);
            if (list == null)
                return null;

            list.ForEach(pr => { pr._client = _client; pr.Repository = this; });
            return list;
        }

        /// <summary>
        /// Returns a single pull request.
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>the single pull request</returns>
        public PullRequest GetPullRequest(int id)
        {
            var request = new RestRequest("/repos/{user}/{repo}/pulls/{pull}");
            request.AddUrlSegment("user", Owner.Login);
            request.AddUrlSegment("repo", Name);
            request.AddUrlSegment("pull", id.ToString());

            var pullrequest = _client.Get<PullRequest>(request).Data;
            if (pullrequest == null)
                return null;

            pullrequest._client = _client;
            pullrequest.Repository = this;
            return pullrequest;
        }

        public GitHubReference GetRef(string refName)
        {
            var request = new RestRequest("/repos/{owner}/{repo}/git/refs/{ref}");
            request.AddUrlSegment("owner", Owner.Login);
            request.AddUrlSegment("repo", Name);
            request.AddUrlSegment("ref", refName);

            var ghRef = _client.Get<GitHubReference>(request).Data;
            if (ghRef == null)
                return null;

            ghRef._client = _client;
            ghRef.Repository = this;
            return ghRef;
        }

        public GitHubMisc GetMisc()
        {
            var ghRef = new GitHubMisc();
            ghRef._client = _client;
            ghRef.Repository = this;
            return ghRef;
        }


        /// <summary>
        /// Creates a new issue
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="body">body</param>
        /// <returns>the issue if successful, null otherwise</returns>
        public Issue CreateIssue(string title, string body)
        {
            var request = new RestRequest("/repos/{owner}/{repo}/issues");
            request.AddUrlSegment("owner", Owner.Login);
            request.AddUrlSegment("repo", Name);

            request.RequestFormat = DataFormat.Json;
            request.AddBody(new
            {
                title = title,
                body = body
            });

            var issue = _client.Post<Issue>(request).Data;
            if (issue == null)
                return null;

            issue._client = _client;
            issue.Repository = this;
            return issue;
        }

        public override bool Equals(object obj)
        {
            return obj is Repository && GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() + ToString().GetHashCode();
        }

        public override string ToString()
        {
            return Owner.Login + "/" + Name;
        }
    }
}
