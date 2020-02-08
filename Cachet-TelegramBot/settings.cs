namespace mySettings
{
    public class ConfigurationSettings
    {
        public class BotSettings
        {

            public string[] adminIds;
            private string botId;

            public BotSettings()
            {
                this.botId = "";
                this.adminIds = new string[];
            }

            public BotSettings(string BotId, string[] AdminIds)
            {
                this.adminIds = AdminIds;
                this.botId = BotId;
            }

            public void AddAdminId(string IdToAdd)
            {
                this.adminIds.Add(IdToAdd);
            }

            public void RemoveAdminId(string IdToRemove)
            {
                List<string> TempList = new List<string>(adminIds);
                TempList.Remove(IdToRemove);
                adminIds = TempList;
            }

            public string[] ReturnAdminIds()
            {
                return adminIds;
            }

            public string BotId { get => botId; set => botId = value; }
        }


        public class CachetSettings
        {



        }



    }



}
