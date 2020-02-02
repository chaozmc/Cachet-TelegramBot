using Newtonsoft.Json;
using System.IO;

namespace Helpers {
	public class HelperFunctions { 

		public static BotSettings InitializeSettingsObject (string fromFile)
		{

			StreamReader sr = new StreamReader(fromFile);
			JsonTextReader jsonTextReader = new JsonTextReader(sr)
			{
				SupportMultipleContent = true
			};

			while (jsonTextReader.Read())
			{

				JsonSerializer serializer = new JsonSerializer();
				return serializer.Deserialize<BotSettings>(jsonTextReader);

			}
			sr.Close();
			return new BotSettings();

		}

		public static void WriteSettingsToFile(BotSettings settingsObject, string targetPath) {

			StreamWriter sw = new StreamWriter(targetPath);
			JsonTextWriter textWriter = new JsonTextWriter(sw);
			JsonSerializer serializer = new JsonSerializer();
			serializer.Serialize(textWriter, settingsObject);
			sw.Flush();
			sw.Close();

		}

	}

}


public class BotSettings {

	public string StrToken { get; set; }
	public string StrChatId { get; set; }

	public BotSettings(string tokenString, string chatIdString)
	{

		StrToken = tokenString;
		StrChatId = chatIdString;
	
	}

	public BotSettings() {

		StrToken = "";
		StrChatId = "";

	}

	public string GetToken()
	{
		return StrToken;
	}

	public string GetChatId() {

		return StrChatId;
	}

	public void SetToken(string newTokenId) {

		StrToken = newTokenId;

	}

	public void SetChadId(string newChatId) {

		StrChatId = newChatId;
		
	}


}
