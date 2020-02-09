using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Cachet_TelegramBot
{
    internal class Program
    {
        private static Telegram.Bot.TelegramBotClient bot;
        private static mySettings.ConfigurationSettings ConfigurationSettings;
        private static readonly string mySettingsFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + System.IO.Path.DirectorySeparatorChar + "settings.json";
        private const string newLine = "\r\n";
        public static bool IShouldRun = false;

        private static void Main(string[] args)
        {
            Console.CancelKeyPress += Console_CancelKeyPress;
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " is running.");
            Console.WriteLine("");
            if (args.Length > 0)
            {

                switch (args[0])
                {
                    case "-resetJson":
                        Console.WriteLine("OK. We're going to reset the settings.json file. Is this what you want? [y,N]");
                        if (Console.ReadKey().KeyChar == "y".ToCharArray()[0])
                        {
                            Console.WriteLine("");
                            Console.WriteLine("");
                            Console.WriteLine("OK. Resetting...");
                            Console.WriteLine("");
                            SetNewParameters(true);
                        }
                        else
                        {
                            Console.WriteLine("");
                            Console.WriteLine("OK. Resuming normal operation. If this was unexpected, check your startup parameters");
                            Console.WriteLine("");
                            StartUpInteractive();

                        }
                        break;

                    case "-backgrounded":
                        StartUpNonInteractive();
                        Environment.Exit(0);
                        break;


                    default:
                        StartUpInteractive();
                        break;
                }

            }
            else
            {
                StartUpInteractive();
            }

            Console.Clear();
            WriteMessageSlowly("Loading menue");
            DrawMainMenue();
        }

        private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("Application will exit now.");
            Environment.Exit(0);
        }

        #region "Support-Functions"

        private static void WriteMessageSlowly(string msg)
        {

            Console.Write("\r\n" + msg);
            for (int i = 0; i < 4; i++)
            {
                System.Threading.Thread.Sleep(500);
                System.Console.Write(".");

            }

        }

        private static void StartUpInteractive()
        {

            WriteMessageSlowly("Try to read config from json file");
            Console.WriteLine("");

            if (System.IO.File.Exists(mySettingsFilePath))
            {
                try
                {
                    ConfigurationSettings = mySettings.ConfigurationSettings.LoadConfigurationSettings(mySettingsFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.WriteLine("Will now exit");
                    Console.ReadLine();
                    Environment.Exit(1);
                    throw ex;
                }
            }
            else
            {
                SetNewParameters(true);
            }

            try
            {
                bot = new Telegram.Bot.TelegramBotClient(ConfigurationSettings.TelegramBot.BotId);
                bot.OnMessage += ReceiveMessageInteractive;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Will now exit");
                Console.ReadLine();
                Environment.Exit(2);
            }

        }

        private static void StartUpNonInteractive()
        {
            //Check if settingsfile exists, if not unattend mode doesn't make sense
            if (!System.IO.File.Exists(mySettingsFilePath))
            {
                Console.WriteLine("Error. settings.json file not found. Unattend mode doesn't work");
                Console.WriteLine("Location where it is expected: " + mySettingsFilePath);
                Environment.Exit(100);
            }

            //Try to load the Bot-Settings from the file. On error, exit the application and write out the exception message
            try
            {
                ConfigurationSettings = mySettings.ConfigurationSettings.LoadConfigurationSettings(mySettingsFilePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while loading settings.json file. Consider reacreating it.");
                Console.WriteLine("Location where it is expected: " + mySettingsFilePath);
                Console.WriteLine(ex.Message);
                Environment.Exit(101);
            }

            //Try to initialize the bot object. On error, write the error to the console and exit.
            try
            {
                bot = new Telegram.Bot.TelegramBotClient(ConfigurationSettings.TelegramBot.BotId);
                bot.OnMessage += ReceiveMessageNonInteractive;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while initializing bot.");
                Console.WriteLine(ex.Message);
                Environment.Exit(102);
            }

            IShouldRun = true;
            PrintReceiveScreenNonInteractive();


        }


        private static void SetNewParameters(bool permanent)
        {

            Console.Clear();

            Console.Write("Enter the Token: ");
            string strToken = Console.ReadLine();

            Console.WriteLine("");

            Console.Write("\r\nEnter an AdminId: ");
            string adminId = Console.ReadLine();

            ConfigurationSettings.TelegramBot.BotId = strToken;
            ConfigurationSettings.TelegramBot.AddAdminId(adminId);

            Console.WriteLine("");
            Console.WriteLine("Token set to: " + ConfigurationSettings.TelegramBot.BotId);
            Console.Write("Admin-IDs set to: ");
            foreach (string ID in ConfigurationSettings.TelegramBot.AdminIds)
            {
                Console.Write(ID + ", ");
            }

            Console.WriteLine("");
            Console.WriteLine("");

            if (permanent)
            {
                ConfigurationSettings.SaveSettingsToFile(mySettingsFilePath);
                Console.WriteLine("Written to file.");
            }
            else
            {
                Console.WriteLine("Saved only to memory. It will be lost on next startup");
            }


            System.Threading.Thread.Sleep(2000);
        }

        private static void DrawMainMenue()
        {
            Console.Clear();


            string consoleMainMenue = "" +
                "**********************************************************************************" + newLine +
                "*********************************** Main Menue ***********************************" + newLine +
                "**********************************************************************************" + newLine +
                "*                                                                                *" + newLine +
                "* (1) => Show status                                                             *" + newLine +
                "* (2) => Set new token and chatid (in memory only, not written to file)          *" + newLine +
                "* (3) => Send a test message to the configured Admin-ChatId                      *" + newLine +
                "* (4) => Send a test message to a specific ChatId                                *" + newLine +
                "* (5) => Start message view mode                                                 *" + newLine +
                "* (q) => Quit                                                                    *" + newLine +
                "*                                                                                *" + newLine +
                "*                                                                                *" + newLine +
                "*                                                                                *" + newLine +
                "*                                                                                *" + newLine +
                "**********************************************************************************" + newLine +
                "" + newLine;
            Console.Write(consoleMainMenue);
            Console.Write(newLine + "Select: ");

            switch (Console.ReadLine())
            {
                case "1":
                    PrintParameters();
                    break;
                case "2":
                    SetNewParameters(false);
                    DrawMainMenue();
                    break;
                case "3":
                    PrintMessageInputWindow(false);
                    DrawMainMenue();
                    break;
                case "4":
                    DrawMainMenue();
                    break;
                case "5":
                    PrintReceiveScreenInteractive();
                    break;
                case "q":
                    Console.Clear();
                    Console.WriteLine("Good bye!");
                    return;
                default:
                    DrawMainMenue();
                    break;
            }

        }

        private static void PrintParameters()
        {
            Console.Clear();
            string displayMenue = "" +
                "**********************************************************************************" + newLine +
                "*  =>  Display Token and ChatId                                                  *" + newLine +
                "**********************************************************************************" + newLine +
                "   Token : " + ConfigurationSettings.TelegramBot.BotId + newLine +
                "   ChatId: " + ConfigurationSettings.TelegramBot.ReturnAdminIdsAsCsv() + newLine +
                "   Bot-ID: " + bot.GetMeAsync().Result.Id + newLine +
                "   Status: " + bot.GetMeAsync().Result.FirstName + " is ready to serve, master!" + newLine +
                "" + newLine;
            Console.Write(displayMenue);
            Console.Write(newLine + "<Press enter to return> ");
            Console.ReadLine();
            DrawMainMenue();

        }

        private static async void PrintMessageInputWindow(bool customChatId)
        {
            if (customChatId == true)
            {
                Console.WriteLine("Enter the Message you want to send, and press <Enter>");
                Console.WriteLine("");
                Console.Write("\r\nMessage: ");
                string msgToSend = Console.ReadLine();
                Console.WriteLine("");
                Console.WriteLine("Enter the ChatId of the User / Group you want to send to and press <Enter>");
                Console.WriteLine("");
                Console.Write("\r\nChatId: ");
                string tempChatId = Console.ReadLine();
                await SendMessageAsync(msgToSend, tempChatId);
            }
            else
            {
                Console.WriteLine("Enter the Message you want to send, and press <Enter>");
                Console.WriteLine("");
                Console.Write("\r\nMessage: ");
                string msgToSend = Console.ReadLine();
                foreach (string AdminId in ConfigurationSettings.TelegramBot.ReturnAdminIds())
                {
                    await SendMessageAsync(msgToSend, AdminId);
                }
            }

        }

        private static async System.Threading.Tasks.Task SendMessageAsync(string msg, string chatid)
        {
            Message message = await bot.SendTextMessageAsync(
                chatId: chatid,
                text: msg,
                parseMode: ParseMode.Html,
                disableNotification: false
                );
        }

        private static void PrintReceiveScreenInteractive()
        {
            Console.Clear();
            WriteMessageSlowly("Switching to message view");
            Console.Clear();
            Console.WriteLine("Press <ESC> if you want to return to main menu");
            bot.StartReceiving();
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                Console.ReadKey();
            }
            bot.StopReceiving();
            Console.Clear();
            DrawMainMenue();
        }

        private static void PrintReceiveScreenNonInteractive()
        {
            Console.Clear();
            bot.StartReceiving();
            while (IShouldRun)
            {
                Console.ReadKey(true);
            }
            Console.WriteLine("Program exiting.");
            Environment.Exit(0);
        }

        private static async void ReceiveMessageNonInteractive(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string senderChatId = e.Message.Chat.Id.ToString();
            
            if (ConfigurationSettings.TelegramBot.IsChatIdInAdminIds(senderChatId))
            {
                string senderName = e.Message.Chat.FirstName;
                string senderLastName = e.Message.Chat.LastName;
                string msg = e.Message.Text;
                Console.WriteLine(senderName + " " + senderLastName + "(" + senderChatId + "): " + msg);
                string answerConcatenated = "";
                foreach (string s in Helpers.HelperFunctions.AnalyzeMessageForCommands(msg, ConfigurationSettings.CachetInstance))
                {
                    answerConcatenated = answerConcatenated + s + newLine;
                }
                await SendMessageAsync(answerConcatenated, senderChatId);
            }
            
        }

        private static async void ReceiveMessageInteractive(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string senderChatId = e.Message.Chat.Id.ToString();
            if (ConfigurationSettings.TelegramBot.IsChatIdInAdminIds(senderChatId))
            {
                string senderName = e.Message.Chat.FirstName;
                string senderLastName = e.Message.Chat.LastName;
                string msg = e.Message.Text;
                Console.WriteLine(senderName + " " + senderLastName + "(" + senderChatId + "): " + msg);
                string answerConcatenated = "";
                foreach (string s in Helpers.HelperFunctions.AnalyzeMessageForCommands(msg, ConfigurationSettings.CachetInstance))
                {
                    Console.WriteLine(s);
                    answerConcatenated = answerConcatenated + s + newLine;
                }
                await SendMessageAsync(answerConcatenated, senderChatId);
            }
        }

        #endregion

    }
}
