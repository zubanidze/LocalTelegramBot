using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot
{
    class BotMarkupKeyboard // Unique to a specific Telegram chat
    {
        List<IBotCommand[]> Keyboard { get; }

        public static ReplyKeyboardMarkup SetReplyKeyboardMarkup()
        {
            var rkm = new ReplyKeyboardMarkup();

            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            cols.Add(new KeyboardButton("Sample text that triggers command one"));
            rows.Add(cols.ToArray());
            cols = new List<KeyboardButton>();
            cols.Add(new KeyboardButton("Sample text that triggers command two"));
            rows.Add(cols.ToArray());
            rkm.Keyboard = rows.ToArray();


            return rkm;
        }
        public static ReplyKeyboardRemove KeyboardRemover()
        {
            //discard custom keyboard and return default one
            ReplyKeyboardRemove replyKeyboardRemove = new ReplyKeyboardRemove();
            return replyKeyboardRemove;
        }
    }



}
