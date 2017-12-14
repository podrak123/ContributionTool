using Git.hub;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            string userName = ConfigurationManager.AppSettings["username"];
            string access_token = ConfigurationManager.AppSettings["access_token"];
            client.setCredentials(userName, access_token);

            //Console.WriteLine();
            //var users = client.getNewUsers();
            //Console.WriteLine("Total Users in Git.Hub: {0}", users.Count);
            //Console.WriteLine("Id of podrak123: {0}", users.Where(u => u.Login == "podrak123").FirstOrDefault().Id);


            Console.WriteLine();
            Console.WriteLine("Repositories of {0}?", userName);
            List<string> repositoryList = client.getRepositories(userName).Select(x => x.Name).ToList();
            repositoryList.ForEach(repo => Console.WriteLine("  {0}", repo));

            foreach (var repoName in repositoryList)
            {
                Console.WriteLine();
                Console.WriteLine(string.Format("Branches of {0}/{1}?", userName, repoName));
                var repository = client.getRepository(userName, repoName);
                if (repository != null)
                {
                    List<User> colloborators = repository.GetCollaborators().ToList();
                    //List<PullRequestCommit> commits = repository.GetCommits().ToList();

                    List<PullRequestCommit> commits = repository.GetCommitsByAuthorName("podrak123").Items;

                    //List<Issue> issues = repository.GetIssues().ToList();

                    List<Issue> issues = repository.GetIssuesByAuthorName("podrak123").Items;

                    List<Branch> branches = repository.GetBranches().ToList();
                    if (branches != null)
                    {
                        foreach (var branch in branches)
                        {
                            Console.WriteLine("  {0} at {1}", branch.Name, branch.Commit.Sha);

                            List<PullRequest> pullRequests = repository.GetPullRequests().ToList();
                            if (pullRequests != null)
                            {
                                Console.WriteLine();
                                Console.WriteLine("Pull Requests of " + repoName);
                                pullRequests.ForEach(pr => Console.WriteLine("  #{0}: {1} by {2}", pr.Number, pr.Title, pr.User.Login));
                            }
                        }  
                    }

                    if (commits != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Commits to {0}/{1}", userName, repoName);

                        foreach (var prc in commits)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Author Name: {0} with comments: \"{1}\"", prc.Commit.Author.Name, prc.Commit.Message);
                        }
                    }

                    if (issues != null)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Issues in {0}/{1}", userName, repoName);

                        foreach (var issue in issues)
                        {
                            Console.WriteLine();
                            Console.WriteLine("{0}. Issue title: {1}",issue.Number, issue.Title);
                        }
                    }
                    
                }                
            }

            Console.ReadLine();
        }
    }
}
