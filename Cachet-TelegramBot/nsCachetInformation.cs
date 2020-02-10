using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CachetInformation
{
	public class CachetInstance
	{
		IList<CachetComponent> cachetComponents;

		public CachetInstance() {
			cachetComponents = new List<CachetComponent>();
		}

		public void AddComponent(CachetComponent cachetComponent)
		{
			this.cachetComponents.Add(cachetComponent);
		}

		public void RemoveComponent(CachetComponent cachetComponent)
		{
			this.cachetComponents.Remove(cachetComponent);
		}

		public CachetComponent ReturnComponentByName(string Name)
		{
			foreach (CachetComponent c in cachetComponents)
			{
				if (c.Name.Contains(Name)) { return c; }
			}
			return null;
		}

		public CachetComponent ReturnComponentById(int id)
		{
			foreach (CachetComponent c in cachetComponents)
			{
				if (c.GetId() == id) { return c; }
			}
			return null;
		}

		public int CountComponents()
		{
			return this.cachetComponents.Count();
		}

		public string ReturnListOfComponentsAndStatus()
		{
			string temp = "";
			if (this.CountComponents() > 0)
			{
				foreach (CachetComponent cachetComponent in this.cachetComponents)
				{
					temp += "<u>" + cachetComponent.Name + "</u>: <i>" + cachetComponent.Status_Name + "</i>" + Cachet_TelegramBot.CVars.newLine;
				}
			} 
			else
			{
				return "No components found!";
			}
			
			return temp;
		}

		public void UpdateComponentsFromSource(mySettings.CachetSettings CachetConnectionSettings)
		{
			this.cachetComponents.Clear();
			try
			{
				string prot = "";
				if (CachetConnectionSettings.UseSSL) { prot = "https"; } else { prot = "http"; }
				System.Uri requestUri = new System.Uri(prot + "://" + CachetConnectionSettings.CachetHost + "/api/v1/components");

				System.Net.HttpWebRequest web = System.Net.WebRequest.CreateHttp(requestUri);
				web.ContentType = "application/json; charset=utf-8";
				web.Method = System.Net.WebRequestMethods.Http.Get;
				web.Accept = "application/json";

				System.IO.StreamReader sr = new System.IO.StreamReader(web.GetResponse().GetResponseStream());

				string responseBody = sr.ReadToEnd();
				Newtonsoft.Json.Linq.JObject jObjectResponse = Newtonsoft.Json.Linq.JObject.Parse(responseBody);

				IList<Newtonsoft.Json.Linq.JToken> tokens = jObjectResponse["data"].Children().ToList();

				foreach (Newtonsoft.Json.Linq.JToken jToken in tokens)
				{
					CachetInformation.CachetComponent cachetComponent = jToken.ToObject<CachetInformation.CachetComponent>();
					this.AddComponent(cachetComponent);
				}

			}
			catch (System.Exception ex)
			{
				throw(ex);
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

		public int GetId()
		{
			return this.Id;
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
	}
