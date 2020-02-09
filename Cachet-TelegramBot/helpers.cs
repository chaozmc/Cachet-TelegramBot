using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Helpers
{
	
	public class HelperFunctions
	{
		public static IList<string> AnalyzeMessageForCommands(string msg, mySettings.CachetSettings cachetSettings)
		{
			switch (msg)
			{
				case "/list":
					return ListOfComponents(cachetSettings);

				case "/shutdown":
					if (Cachet_TelegramBot.Program.IShouldRun) {
						IList<string> temp2 = new List<string>();
						temp2.Add("Good bye. Program will exit now");
						Cachet_TelegramBot.Program.IShouldRun = false; 
						return temp2;
					}
					return null;
				
				default:
					IList<string> temp = new List<string>();
					temp.Add("Command not understood");
					return temp;

			}
		}


		public static IList<string> ListOfComponents(mySettings.CachetSettings cachetSettings)
		{

			IList<string> temp = new List<string>();
			IList<CachetComponent> components = new List<CachetComponent>();

			try
			{
				string prot = "";
				if (cachetSettings.UseSSL) { prot = "https"; } else { prot = "http"; }
				System.Uri requestUri = new System.Uri(prot + "://" + cachetSettings.CachetHost + "/api/v1/components");


				HttpWebRequest web = WebRequest.CreateHttp(requestUri);
				web.ContentType = "application/json; charset=utf-8";
				web.Method = WebRequestMethods.Http.Get;
				web.Accept = "application/json";

				StreamReader sr = new StreamReader(web.GetResponse().GetResponseStream());

			string responseBody = sr.ReadToEnd();
			Newtonsoft.Json.Linq.JObject jObjectResponse = Newtonsoft.Json.Linq.JObject.Parse(responseBody);

			IList<Newtonsoft.Json.Linq.JToken> tokens = jObjectResponse["data"].Children().ToList();

			foreach (Newtonsoft.Json.Linq.JToken jToken in tokens)
			{
				CachetComponent cachetComponent = jToken.ToObject<CachetComponent>();
				components.Add(cachetComponent);
			}

			}
			catch (System.Exception ex)
			{
				temp.Add(ex.Message);
				return temp;
			}

			if (components.Count > 0)
			{
				foreach (CachetComponent cachetComponent in components)
				{
					temp.Add("<u>" + cachetComponent.Name + "</u>: <i>" + cachetComponent.Status_Name + "</i>");
				}
			}
			else
			{
				temp.Add("No components found");
			}

			return temp;
		}

	}

}

public class CachetComponent
{
	protected int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public string Link { get; set; }
	public int Status { get; set; }
	public bool Enabled { get; set; }
	public string Status_Name { get; set; }

	public const int STATUS_OPERATIONAL = 1;
	public const int STATUS_PERFORMANCE_ISSUES = 2;
	public const int STATUS_MINOR_OUTTAGE = 3;
	public const int STATUS_MAJOR_OUTTAGE = 4;

	public CachetComponent(int Id, string Name, string Description, string Link, int Status, bool Enabled, string StatusName)
	{
		this.Id = Id;
		this.Name = Name;
		this.Description = Description;
		this.Link = Link;
		this.Status = Status;
		this.Enabled = Enabled;
		this.Status_Name = StatusName;
	}

	public void SetName(string NewComponentName)
	{
		this.Name = NewComponentName;
	}

	public void SetStatus(int newStatusId)
	{
		this.Status = newStatusId;

	}
}
