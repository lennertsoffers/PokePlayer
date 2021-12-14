using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace PokePlayer_Library.Tools {
	public class ApiTools {
		public static JObject GetSpecieData(string url) {
			WebRequest request = WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse) request.GetResponse();
			Stream dataStream = response.GetResponseStream();
			StreamReader reader = new StreamReader(dataStream);
			string responseFromServer = reader.ReadToEnd();
			JObject json = JObject.Parse(responseFromServer);
			reader.Close();
			dataStream.Close();
			response.Close();

			return json;
		}
	}
}
