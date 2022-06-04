using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using System.IO;
using System.Threading;

namespace TelegramBot
{
    class TelegramChat
    {
        public ChatId TgChatId { get; }
        public Chat TgChat { get; }
        TgBot Bot { get; }
        bool ActionAwaits { get; set; }
        object ActionAwaitsSyncObject = new object();
        UserActionType ActionType { get; set; }
        object ActionArg { get; set; }
        public bool IsProcessing { get; set; }
        bool DoProcess { get; set; }
        object DoProcessSyncObject { get; set; }
        AutoResetEvent ThreadWaitHandler { get; set; }



        public TelegramChat(Chat chat, TgBot bot)
        {
            TgChatId = chat.Id;
            TgChat = chat;
            Bot = bot;
            DoProcessSyncObject = new object();
            Console.WriteLine("Chat constructed");
            ThreadWaitHandler = new AutoResetEvent(false);
        }


        public void StartProcessing()
        {
            lock (DoProcessSyncObject)
            {
                DoProcess = true;
            }
        }

        public void Process()
        {
            while (DoProcess)
            {
                ThreadWaitHandler.WaitOne();

                Console.WriteLine("process action " + ActionType);

                switch (ActionType)
                {
                    case UserActionType.MessageSent:
                        HandleMessageRecieved(ActionArg as Message);
                        break;
                    case UserActionType.CallbackQuery:
                        HandleCallbackQuery(ActionArg as CallbackQuery);
                        break;

                }

                SetActionAwaits(false);
            }

            IsProcessing = false;
        }

        public void StopProcess()
        {
            lock (DoProcessSyncObject)
            {
                DoProcess = false;
            }
        }


        void SetActionAwaits(bool awaits)
        {
            Console.WriteLine("set action awaits");

            lock (ActionAwaitsSyncObject)
            {
                if (awaits)
                    ThreadWaitHandler.Set();
                else ThreadWaitHandler.Reset();
                ActionAwaits = awaits;
            }
        }

        public void PostAction(UserActionType actionType, object arg)
        {
            Console.WriteLine("post action chat");
            ActionType = actionType;
            ActionArg = arg;
            SetActionAwaits(true);
        }

        private async void HandleMessageRecieved(Message message)
        {
            string[] commandArgs = null;

            if (message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
            {
                Console.WriteLine("Wrong format of message");
                return;
            }
            if (message.Chat.Type != Telegram.Bot.Types.Enums.ChatType.Private)
                commandArgs = message.Text.Split('@');
            else
                commandArgs = message.Text.Split(' ');


            Console.WriteLine("command");
            IBotCommand command = Bot.CommonCommands.FirstOrDefault(x => x.CommandQuery == commandArgs[0]);


            if (command != null)
            {
                Console.WriteLine("command: " + command.CommandQuery);
                await command.Process(this, message.From, message, commandArgs);
            }
            else
            {

                Console.WriteLine("singletone command not found");
                await Bot.BotClient.SendTextMessageAsync(message.Chat.Id, "This command doesn't exist, use /info");

            }

        }

        private void HandleCallbackQuery(CallbackQuery callbackQuery)
        {

        }

        private void HandleMessageEdit(Message message)
        {

        }

    }

    enum UserActionType
    {
        MessageSent = 1, MessageEdit = 2, InlineQuery = 3, CallbackQuery = 4, InlineResultChoose = 5, NotificationPending = 6
    }
}
