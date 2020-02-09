using System.Linq;

namespace mySettings
{
    public class ConfigurationSettings
    {

        public BotSettings TelegramBot;
        public CachetSettings CachetInstance;

        public ConfigurationSettings(BotSettings botSettings, CachetSettings cachetSettings)
        {
            this.TelegramBot = botSettings;
            this.CachetInstance = cachetSettings;
        }

        public static ConfigurationSettings LoadConfigurationSettings(string SettingsFilePath)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader(SettingsFilePath);

            Newtonsoft.Json.JsonTextReader textReader = new Newtonsoft.Json.JsonTextReader(sr);
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            return serializer.Deserialize<mySettings.ConfigurationSettings>(textReader);

            //string content = sr.ReadToEnd();
            //sr.Close();
            //Newtonsoft.Json.Linq.JObject jObjectResponse = Newtonsoft.Json.Linq.JObject.Parse(content);
            //System.Collections.Generic.IList<Newtonsoft.Json.Linq.JToken> tokens = jObjectResponse["data"].Children().ToList();

            //foreach (Newtonsoft.Json.Linq.JToken jToken in tokens)
            //{
            //    BotSettings botSettings = jToken.ToObject<BotSettings>();
            //    CachetComponent cachetComponent = jToken.ToObject<CachetComponent>();
            //}
        }



        public void SaveSettingsToFile(string SettingsFilePath)
        {
            
            System.IO.StreamWriter sw = new System.IO.StreamWriter(SettingsFilePath);
            Newtonsoft.Json.JsonTextWriter textWriter = new Newtonsoft.Json.JsonTextWriter(sw);
            Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
            serializer.Serialize(textWriter, this);
            
            sw.Flush();
            sw.Close();

        }


    }

public class BotSettings
        {

            private string[] adminIds;
            private string botId;


            public BotSettings(string BotId, string[] AdminIds)
            {
                this.adminIds = AdminIds;
                this.botId = BotId;
            }

            public void AddAdminId(string IdToAdd)
            {
                System.Collections.Generic.List<string> TempList = new System.Collections.Generic.List<string>(adminIds);
                TempList.Add(IdToAdd);
                this.adminIds = TempList.ToArray();
            }

            public void RemoveAdminId(string IdToRemove)
            {
                System.Collections.Generic.List<string> TempList = new System.Collections.Generic.List<string>(adminIds);
                TempList.Remove(IdToRemove);
                this.adminIds = TempList.ToArray();
            }

            public string[] ReturnAdminIds()
            {
                return adminIds;
            }

            public string ReturnAdminIdsAsCsv()
            {
                string temp = "";
                for (int i = 0; i < adminIds.Length; i++)
                {
                    temp = temp + adminIds[i] + ", ";
                }
                temp = temp.Substring(0, temp.LastIndexOf(",") - 3);
                return temp;
            }

        public bool IsChatIdInAdminIds(string chatId)
        {
            foreach (string AdminId in adminIds)
            {
                if (chatId == AdminId)
                {
                    return true;
                }
            }
            return false;
        }

            public string BotId { get => botId; set => botId = value; }
            public string[] AdminIds { get => adminIds; }
        }

        public class CachetSettings
        {
            private string cachetHost;
            private string cachetAPIToken;
            private bool useSSL;

            public CachetSettings(string Hostname, string APIToken, bool UseSSL = true)
            {
                this.cachetHost = Hostname;
                this.cachetAPIToken = APIToken;
                this.useSSL = UseSSL;
            }

            public string CachetHost { get => this.cachetHost; set => this.cachetHost = value; }
            public string CachetAPIToken { get => this.cachetAPIToken; set => this.cachetAPIToken = value; }
            public bool UseSSL { get => this.useSSL; set => this.useSSL = value; }
        }

    }
