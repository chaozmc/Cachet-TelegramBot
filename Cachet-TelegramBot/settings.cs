namespace mySettings
{
    public class ConfigurationSettings
    {

        public BotSettings bot;
        public CachetSettings cachet;

        public ConfigurationSettings(BotSettings botSettings, CachetSettings cachetSettings)
        {
            this.bot = botSettings;
            this.cachet = cachetSettings;
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
            private string CachetHost;
            private string CachetAPIToken;
            private bool UseSSL;

            public CachetSettings(string Hostname, string APIToken, bool UseSSL = true)
            {
                this.CachetHost = Hostname;
                this.CachetAPIToken = APIToken;
                this.UseSSL = UseSSL;
            }

            public string CachetHost { get => this.CachetHost; set => this.CachetHost = value; }
            public string CachetAPIToken { get => this.CachetAPIToken; set => this.CachetAPIToken = value; }
            public bool UseSSL { get => this.UseSSL; set => this.UseSSL = value; }
        }

    }



}
