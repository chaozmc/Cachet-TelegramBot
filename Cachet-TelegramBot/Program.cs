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
        public static bool IShouldRun = false;
        private static CachetInformation.CachetInstance myCachetInstance = new CachetInformation.CachetInstance();

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
                            SetupDialog(true);
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
                SetupDialog(true);
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

        private static void SetupDialog(bool permanent)
        {
            Console.Clear();
            string setupMessage = "" +
                    "**********************************************************************************" + CVars.newLine +
                    "***********************************    Setup    **********************************" + CVars.newLine +
                    "**********************************************************************************" + CVars.newLine +
                    "*                                                                                *" + CVars.newLine;

            Console.Write(setupMessage);
            Console.Write("* Please enter the Token-ID: ");
            string NewBotToken = Console.ReadLine();
            Console.WriteLine("*                                                                                *");
            Console.Write("* Please enter your first administrative ChatId: ");
            string NewFirstAdminChatId = Console.ReadLine();
            Console.WriteLine("*                                                                                *");
            Console.WriteLine("* OK, next we'll setup your cachet instance.                                     *");
            Console.Write("* Enter your Cachet's dns name (no http or https url): ");
            string NewCachetHost = Console.ReadLine();
            Console.WriteLine("*                                                                                *");
            Console.Write("* Your auth-token for Cachet: ");
            string NewCachetToken = Console.ReadLine();
            Console.WriteLine("*                                                                                *");
            Console.Write("* Does your Cachetserver is reachable over https? <Y/n> ");
            ConsoleKeyInfo answer = Console.ReadKey(false);
            while (!(answer.Key == ConsoleKey.Y || answer.Key == ConsoleKey.Enter || answer.Key == ConsoleKey.N)) {
                Console.Write(CVars.newLine + "* Does your Cachetserver is reachable over https? <Y/n> ");
                answer = Console.ReadKey(false);
            }
            bool useHTTPs = true;
            if (answer.Key == System.ConsoleKey.Y || answer.Key == System.ConsoleKey.Enter)
            {
                useHTTPs = true;
            }
            else
            {
                useHTTPs = false;
            }

            Console.WriteLine("*                                                                                *");
            Console.WriteLine("* Congratulation. Setup completed.                                               *");


            mySettings.BotSettings NewBotSettings = new mySettings.BotSettings(NewBotToken, new string[] { NewFirstAdminChatId });
            mySettings.CachetSettings NewCachetSettings = new mySettings.CachetSettings(NewCachetHost, NewCachetToken, useHTTPs);
            ConfigurationSettings = new mySettings.ConfigurationSettings(NewBotSettings, NewCachetSettings);

            if (permanent) { 
                ConfigurationSettings.SaveSettingsToFile(mySettingsFilePath);
                Console.WriteLine("*                                                                                *");
                Console.WriteLine("* File settings.json saved.                                                      *");
            } else
            {
                Console.WriteLine("*                                                                                *");
                Console.WriteLine("* Settings saved.                                                                *");
            }
        }

        private static void DrawMainMenue()
        {
            Console.Clear();


            string consoleMainMenue = "" +
                "**********************************************************************************" + CVars.newLine +
                "*********************************** Main Menue ***********************************" + CVars.newLine +
                "**********************************************************************************" + CVars.newLine +
                "*                                                                                *" + CVars.newLine +
                "* (1) => Show current settings                                                   *" + CVars.newLine +
                "* (2) => Call Setupdialog (in memory only, not written to file)                  *" + CVars.newLine +
                "* (3) => Send a test message to the configured Admin-ChatIds                     *" + CVars.newLine +
                "* (4) => Send a test message to a specific ChatId                                *" + CVars.newLine +
                "* (5) => Start message view mode                                                 *" + CVars.newLine +
                "* (q) => Quit                                                                    *" + CVars.newLine +
                "*                                                                                *" + CVars.newLine +
                "*                                                                                *" + CVars.newLine +
                "*                                                                                *" + CVars.newLine +
                "*                                                                                *" + CVars.newLine +
                "**********************************************************************************" + CVars.newLine +
                "" + CVars.newLine;
            Console.Write(consoleMainMenue);
            Console.Write(CVars.newLine + "Select: ");

            switch (Console.ReadLine())
            {
                case "1":
                    PrintParameters();
                    break;
                case "2":
                    SetupDialog(false);
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
                "**********************************************************************************" + CVars.newLine +
                "*  =>  Display current settings                                                  *" + CVars.newLine +
                "**********************************************************************************" + CVars.newLine +
                "   BotToken.........: " + ConfigurationSettings.TelegramBot.BotId + CVars.newLine +
                "   AdminChat-IDs....: " + ConfigurationSettings.TelegramBot.ReturnAdminIdsAsCsv() + CVars.newLine +
                "   Bot-ID...........: " + bot.GetMeAsync().Result.Id + CVars.newLine +
                "   Status...........: " + bot.GetMeAsync().Result.FirstName + " is ready to serve, master!" + CVars.newLine +
                "   CachetHost.......: " + ConfigurationSettings.CachetInstance.CachetHost + CVars.newLine + 
                "   Cachet-Token.....: " + ConfigurationSettings.CachetInstance.CachetAPIToken + CVars.newLine +
                "   SSL for Cachet...: " + ConfigurationSettings.CachetInstance.UseSSL.ToString() + CVars.newLine + 
                "" + CVars.newLine;
            Console.Write(displayMenue);
            Console.Write(CVars.newLine + "<Press enter to return> ");
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
                myCachetInstance.UpdateComponentsFromSource(ConfigurationSettings.CachetInstance);
                string answerConcatenated = Helpers.HelperFunctions.AnalyzeMessageAndReturnAnswer(msg, myCachetInstance);
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
                myCachetInstance.UpdateComponentsFromSource(ConfigurationSettings.CachetInstance);
                string answerConcatenated = Helpers.HelperFunctions.AnalyzeMessageAndReturnAnswer(msg, myCachetInstance);
                await SendMessageAsync(answerConcatenated, senderChatId);
            }
        }

        #endregion

    }
}
