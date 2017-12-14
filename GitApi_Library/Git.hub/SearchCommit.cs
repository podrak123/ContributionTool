using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Git.hub
{
    /// <summary>
    /// class to pull details on search commits
    /// </summary>
    public class SearchCommit
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<PullRequestCommit> Items { get; internal set; }
    }
}
