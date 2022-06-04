using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using Telegram.Bot.Types.Enums;

namespace TelegramBot
{
    partial class TgBot
    {
        public ITelegramBotClient BotClient { get; }
        public Dictionary<TelegramChat, Thread> ActiveChats { get; }
        public List<TelegramChat> AllChats { get; }
        public List<TelegramBotUser> ActiveUsers { get; }
        public List<IBotCommand> CommonCommands { get; }
        public ConcurrentDictionary<string, string> CallbackDataDictionary { get; }
        public TgBot(ITelegramBotClient client)
        {
            BotClient = client;
            ActiveChats = new Dictionary<TelegramChat, Thread>();
            AllChats = new List<TelegramChat>();
            ActiveUsers = new List<TelegramBotUser>();
            CommonCommands = new List<IBotCommand>();
            CallbackDataDictionary = new ConcurrentDictionary<string, string>();
            BotClient.OnMessage += OnMessageRecieved;
            BotClient.OnCallbackQuery += OnCallBackQuery;
            BotClient.OnInlineQuery += OnInlineQuery;
            BotClient.OnInlineResultChosen += OnInlineResultChosen;           
        }

        public void Start()
        {
            RegisterMainCommands();
            RegisterInternalCommands();
            BotClient.StartReceiving();
        }

        async void RegisterInternalCommands()
        {
            List<BotCommand> internalBotCommands = new List<BotCommand>();
            internalBotCommands.Add(new BotCommand { Command = "start", Description = "starts interaction with bot" });
            internalBotCommands.Add(new BotCommand { Command = "info", Description = "info about this bot" });

            await BotClient.SetMyCommandsAsync(internalBotCommands);
        }

        void RegisterMainCommands()
        {
            BotCommands.StartCommand startCommand = new BotCommands.StartCommand(this);
            BotCommands.InfoAndHelpCommand infoAndHelpCommand = new BotCommands.InfoAndHelpCommand(this);

            InlineQueryCommands.SampleInlineCommand sampleInlineCommand = new InlineQueryCommands.SampleInlineCommand(this);

            CallbackQueryCommands.SampleCallBackCommand sampleCallBackCommand = new CallbackQueryCommands.SampleCallBackCommand(this);
            CallbackQueryCommands.EditCallBackCommand editCallBackCommand = new CallbackQueryCommands.EditCallBackCommand(this);

            CommonCommands.Add(startCommand);
            CommonCommands.Add(infoAndHelpCommand);
            CommonCommands.Add(sampleInlineCommand);
            CommonCommands.Add(sampleCallBackCommand);
            CommonCommands.Add(editCallBackCommand);
        



        }

        
        public TelegramChat HandleChat(Chat chat, Message message)
        {
            Console.WriteLine("handle chat");
            TelegramChat tgChat = ActiveChats.Keys.FirstOrDefault(x => x.TgChatId.Identifier == chat.Id);
            if (tgChat == null)
            {
                Console.WriteLine("chat is not active");
                tgChat = AllChats.FirstOrDefault(x => x.TgChatId.Identifier == chat.Id);
            }
            if (tgChat == null)
            {
                Console.WriteLine("chat is not registered");
                tgChat = new TelegramChat( chat, this);
                RegisterNewChat(tgChat,message);
            }
            
            
            return tgChat;
        }

        async public void ActivateChat(TelegramChat chat, Message message)
        {
            chat.StartProcessing();
            ThreadStart chatStart = new ThreadStart(chat.Process);
            Thread chatThread = new Thread(chatStart);
            chatThread.Start();
            ActiveChats.Add(chat, chatThread);         
        }

        public void RegisterNewChat(TelegramChat tgChat,Message msg)
        {
            AllChats.Add(tgChat);
            ActivateChat(tgChat,msg);
        }        

        private async void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            Console.WriteLine("OnMessage: type = " + e.Message.Type + ", " + e.Message.Text);
            var chat = HandleChat(e.Message.Chat, e.Message);
            if (chat != null)
            {
                Console.WriteLine("OnMessage for chat: " + e.Message.Chat.Id);
                chat.PostAction(UserActionType.MessageSent, e.Message);
            }
        }

        private async void OnCallBackQuery(object sender, CallbackQueryEventArgs e)
        {
            Console.WriteLine("OnCallback for chat: " + e.CallbackQuery.From.Username + ", callback data = " + e.CallbackQuery.Data + ", callback id = " + e.CallbackQuery.Id);
            try
            {
                string callBackData = "";
                if (!CallbackDataDictionary.TryGetValue(e.CallbackQuery.Data, out callBackData))
                {
                    Console.WriteLine("OnCallback for chat: " + e.CallbackQuery.From.Username + ", invalid callback data");
                    await BotClient.AnswerCallbackQueryAsync(e.CallbackQuery.Id, "invalid callback data", true);
                    return;
                }
              
                string[] queryArgs = callBackData.Split(':');
                string query = queryArgs[0];
                var command = CommonCommands.FirstOrDefault(x => x.CommandQuery == query);
                await command.Process(null, e.CallbackQuery.From, e.CallbackQuery, queryArgs);
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine(ex.Message);
                Console.WriteLine("Callback query is too old for chat: " + e.CallbackQuery.From.Username + ", callback data = " + e.CallbackQuery.Data + ", callback id = " + e.CallbackQuery.Id);
                return;
            }
        }

        public async void OnInlineQuery(object sender, InlineQueryEventArgs e)
        {
            if (!e.InlineQuery.Query.Contains(':'))
                return;

            string[] queryArgs = e.InlineQuery.Query.Split(':');
            string query = queryArgs[0];
            var command = CommonCommands.FirstOrDefault(x => x.CommandQuery == query);

            if (command == null)
                return;

            Console.WriteLine("OnInlineQuery: command found");
            await command.Process(null, e.InlineQuery.From, e.InlineQuery, queryArgs);


            return;           
        }

       
        private async void OnInlineResultChosen(object sender, ChosenInlineResultEventArgs e)
        {

        }
    }
}
