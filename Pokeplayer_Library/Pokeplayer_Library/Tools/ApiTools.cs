using System.Net;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json.Linq;

// Static class with method to query api data

namespace PokePlayer_Library.Tools {
	public static class ApiTools {

		private static readonly HttpClient _client = new HttpClient();

		// Static method to convert the api data from certain url to a JObject
		public static JObject GetApiData(string url) {
			// Send api request to the given url
			HttpResponseMessage response = _client.Send(new HttpRequestMessage(HttpMethod.Get, url));
			using (var reader = new StreamReader(response.Content.ReadAsStream())) {
				// Parse the response stream to a JObject
				JObject json = JObject.Parse(reader.ReadToEnd());
				reader.Close();
				return json;
			}
		}
	}
}