using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// Handles communication with Reittiopas API
    /// </summary>
    class Connector
    {
        string digiTransitEndpoint = @"https://api.digitransit.fi/routing/v1/routers/hsl/index/graphql";

        public string Url { get; set; }

        /// <summary>
        /// Makes GET request to Reittiopas API.
        /// Request scope is defined in Url property of the class
        /// </summary>
        /// <returns>XML con</returns>
        public async Task<string> GetXmlStringAsync()
        {
            Task<string> response;
            string responseContent = null;

            if (Url == null)
                return null;

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    //client.Timeout = TimeSpan.FromSeconds(3);
                    response = client.GetStringAsync(Url);
                    responseContent = await response;
                }
            }
            catch(WebException ex)
            {
                //TODO
            }
            
            return responseContent;
        }

        public string GetXmlStringSync()
        {
            string responseContent = null;

            if (Url == null)
                return null;

            try
            {
                WebRequest request = WebRequest.Create(Url);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();

                StreamReader streamReader = new StreamReader(stream);
                responseContent = streamReader.ReadToEnd();
                response.Close();
                streamReader.Close();
            }
            catch (WebException ex)
            {
                //TODO
            }

            return responseContent;
        }

        /// <summary>
        /// Makes POST request to Digitransit API.
        /// </summary>
        /// <param name="query">Valid GraphQL query for Digitransit API</param>
        /// <returns>Query result in JSON format</returns>
        public async Task<string> GetGraphQlAsync(string query)
        {
            string responseContent = null;

            try
            { 
                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(query, System.Text.Encoding.UTF8);
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/graphql");

                    var response = client.PostAsync(digiTransitEndpoint, content);
                    var result = response.Result;
                    responseContent = await response.Result.Content.ReadAsStringAsync();
                }
            }
            catch (WebException ex)
            {
                //TODO
            }

            return responseContent;
        }
    }
}