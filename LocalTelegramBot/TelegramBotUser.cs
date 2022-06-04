using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    class TelegramBotUser
    {
        public Telegram.Bot.Types.User TgUser { get; }
        public BotUserRole BotUserRole { get; }

        public TelegramBotUser(Telegram.Bot.Types.User tgUser, BotUserRole role)
        {
            TgUser = tgUser;
            BotUserRole = role;
        }
    }

    enum BotUserRole
    {
        Admin = 0, User = 1, Moderator = 2
    }
}
