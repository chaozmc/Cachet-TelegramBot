using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Cachet_TelegramBot
{
	public static class CVars
	{
		public const string newLine = "\r\n";
	}
}

namespace Helpers
{
	
	public class HelperFunctions
	{
		

		public static string AnalyzeMessageAndReturnAnswer(string msg, CachetInformation.CachetInstance cachetInstance)
		{
			string[] MsgArray = msg.Split(" ");
			switch (MsgArray[0])
			{
				case "/list":
					return cachetInstance.ReturnListOfComponentsAndStatus();

				case "/say":
					if (MsgArray.Length > 1)
					{
						return msg.Replace("/say ", "");
					}
					return "Nothing to say";
				
				default:
					return "Command not understood";

			}
		}
	}

}
