using Newtonsoft.Json;

namespace Git.hub
{
    public class User
    {
        /// <summary>
        /// The GitHub username
        /// </summary>
        [JsonProperty("Login")]
        public string Login { get; internal set; }

        [JsonProperty("Id")]
        public int Id { get; internal set; }

        /// <summary>
        /// Gets the site_admin flag.
        /// </summary>
        /// <value>
        /// The site_admin.
        /// </value>
        //public bool Site_admin { get; internal set; }

        public User()
        {
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            return GetType().GetHashCode() + Login.GetHashCode();
        }

        public override string ToString()
        {
            return Login;
        }
    }
}

