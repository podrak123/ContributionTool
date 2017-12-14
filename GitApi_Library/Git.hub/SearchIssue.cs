using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Git.hub
{
    /// <summary>
    /// class for search issue results
    /// </summary>
    public class SearchIssue
    {
        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        public List<Issue> Items { get; internal set; }
    }
}
