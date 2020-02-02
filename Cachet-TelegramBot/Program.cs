﻿using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace Cachet_TelegramBot
{
    class Program
    {
        private static Telegram.Bot.TelegramBotClient bot;
        private static BotSettings botSettings = new BotSettings();
        private static readonly string mySettingsFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\settings.json";
        private const string newLine = "\r\n";

        static void Main(string[] args)
        {
            Console.WriteLine(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name + " is running.");
            Console.WriteLine("");
            if (args.Length > 0) {

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
                            StartUp();

                        }

                        break;

                    default:
                        StartUp();
                        break;
                }

            } 
            else
            {
                StartUp();
            }

            Console.Clear();
            WriteMessageSlowly("Loading menue");
            DrawMainMenue();
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

        private static void StartUp() {

            WriteMessageSlowly("Try to read config from json file");
            Console.WriteLine();

            if (System.IO.File.Exists(mySettingsFilePath))
            {
                try
                {
                    botSettings = Helpers.HelperFunctions.InitializeSettingsObject(mySettingsFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: " + ex.Message);
                    Console.WriteLine("");
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
                bot = new Telegram.Bot.TelegramBotClient(botSettings.StrToken);
                bot.OnMessage += ReceiveMessage;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                Console.WriteLine("Press <Enter> to start the setup-screen");
                Console.WriteLine("Will now exit");
                Environment.Exit(2);

            }

        }



        private static void SetNewParameters(bool permanent) {

            Console.Clear();

            Console.Write("Enter the Token: ");
            string strToken = Console.ReadLine();

            Console.WriteLine("");

            Console.Write("\r\nEnter the ChatId: ");
            string chatId = Console.ReadLine();

            botSettings.SetToken(strToken);
            botSettings.SetChadId(chatId);

            Console.WriteLine("");
            Console.WriteLine("Token set to: " + botSettings.StrToken);
            Console.WriteLine("ChatId set to: " + botSettings.StrChatId);
            Console.WriteLine("");

            if (permanent)
            {
                Helpers.HelperFunctions.WriteSettingsToFile(botSettings, mySettingsFilePath);
                Console.WriteLine("Written to file.");
            } else
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
                "* (3) => Send a test message to the configured chatid                            *" + newLine +
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
                    PrintMessageInputWindow();
                    DrawMainMenue();
                    break;
                case "4":
                    DrawMainMenue();
                    break;
                case "5":
                    PrintReceiveScreen();
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
                "   Token : " + botSettings.StrToken + newLine +
                "   ChatId: " + botSettings.StrChatId + newLine +
                "   Bot-ID: " + bot.GetMeAsync().Result.Id +  newLine +
                "   Status: " + bot.GetMeAsync().Result.FirstName + " is ready to serve, master!" + newLine +
                "" + newLine;
            Console.Write(displayMenue);
            Console.Write(newLine + "<Press enter to return> ");
            Console.ReadLine();
            DrawMainMenue();

        }

        private static async void PrintMessageInputWindow()
        {
            Console.WriteLine("Enter the Message you want to send, and press <Enter>");
            Console.WriteLine("");
            Console.Write("\r\nMessage: ");
            string msgToSend = Console.ReadLine();
            await SendMessageAsync(msgToSend);
        }

        private static async System.Threading.Tasks.Task SendMessageAsync(string msg)
        {

            Message message = await bot.SendTextMessageAsync(
                chatId: botSettings.StrChatId,
                text: msg,
                parseMode: ParseMode.MarkdownV2,
                disableNotification: false
                );

        }

        private static async System.Threading.Tasks.Task SendMessageAsync(string msg, string chatid)
        {

            Message message = await bot.SendTextMessageAsync(
                chatId: chatid,
                text: msg,
                parseMode: ParseMode.MarkdownV2,
                disableNotification: false
                );

        }

        private static void PrintReceiveScreen()
        {

            Console.Clear();
            WriteMessageSlowly("Switching to message view");
            Console.Clear();
            Console.WriteLine("Press <ESC> if you want to return to main menu");
            bot.StartReceiving();
            while (Console.ReadKey().Key != ConsoleKey.Escape) {
                Console.ReadKey();
            }
            bot.StopReceiving();
            Console.Clear();
            DrawMainMenue();
        }

        private static void ReceiveMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            string senderChatId = e.Message.Chat.Id.ToString();
            string senderName = e.Message.Chat.FirstName;
            string senderLastName = e.Message.Chat.LastName;
            string msg = e.Message.Text;

            Console.WriteLine(senderName + " " + senderLastName + "(" + senderChatId + ") said: " + msg);

        }

        #endregion

    }
}