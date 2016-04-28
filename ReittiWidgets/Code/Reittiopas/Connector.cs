using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace ReittiWidgets.Code.Reittiopas
{
    /// <summary>
    /// Handles communication with Reittiopas API
    /// </summary>
    class Connector
    {
        public string Url { get; set; }

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
    }
}