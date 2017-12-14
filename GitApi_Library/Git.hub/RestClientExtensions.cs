namespace Git.hub
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using RestSharp;
    using System.Configuration;
    using System.Collections.Specialized;
    using System.Text;

    internal static class RestClientExtensions
    {
        private static readonly Regex LinkHeaderFormat = new Regex(@"<(?<Link>[^>]*)>; rel=""(?<Rel>\w*)""", RegexOptions.Compiled);
        private static NameValueCollection paginationSetting = (NameValueCollection)ConfigurationManager.GetSection("PaginationSettings");

        public static List<T> GetList<T>(this IRestClient client, IRestRequest request)
        {
            List<T> result = new List<T>();

            AppendParameters(request);

            while (true)
            {
                IRestResponse<List<T>> pageResponse = client.Get<List<T>>(request);
                if (pageResponse.Data == null)
                    return null;

                result.AddRange(pageResponse.Data);

                Parameter linkHeader = pageResponse.Headers.FirstOrDefault(i => string.Equals(i.Name, "Link", StringComparison.OrdinalIgnoreCase));
                if (linkHeader == null)
                    break;

                bool hasNext = false;
                foreach (Match match in LinkHeaderFormat.Matches(linkHeader.Value.ToString()))
                {
                    if (string.Equals(match.Groups["Rel"].Value, "next", StringComparison.OrdinalIgnoreCase))
                    {
                        request = new RestRequest(new Uri(match.Groups["Link"].Value));
                        hasNext = true;
                        break;
                    }
                }

                if (!hasNext)
                    break;
            }

            return result;
        }

        private static void AppendParameters(IRestRequest request)
        {
            StringBuilder sbParameters = new StringBuilder();
            if (paginationSetting.Count > 0 && !request.Resource.Contains("?"))
            {
                sbParameters.Append("?");
            }
            for (int i = 0; i < paginationSetting.Count; i++)
            {
                sbParameters.AppendFormat("&{0}={1}", paginationSetting.GetKey(i), paginationSetting[i]);
            }
            request.Resource = request.Resource + sbParameters.ToString();
        }
    }
}
