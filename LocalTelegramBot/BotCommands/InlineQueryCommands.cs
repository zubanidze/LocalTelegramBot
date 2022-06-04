using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types;
using System.Drawing;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramBot.InlineQueryCommands
{
    class SampleInlineCommand : IBotCommand
    {
        public TgBot OwnerBot { get; }
        public string CommandQuery { get; } = "inline";
        public string UserFriendlyNameOfCommand { get; } = "@YourBotName inline:";
        public string CommandDescription { get; } = "This is inline command you can trigger it in any chat using @YourBotName";

        public BotCommandType CommandType { get; } = BotCommandType.InlineCommand;

        public List<BotUserRole> CommandAvailableTo { get; } = new List<BotUserRole> { BotUserRole.User, BotUserRole.Admin, BotUserRole.Moderator };

        public bool CanHaveMultipleInstances { get; }

        public bool CanBeExecutedInGroupChat { get; } = true;

        public SampleInlineCommand(TgBot bot)
        {
            OwnerBot = bot;
        }

        public async Task<bool> CanBeExecuted(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            return true;
        }

        public async Task<BotCommandProcessResult> Process(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args)
        {
            InlineQuery query = e as InlineQuery;
            string guid = args[1] as string;

            List<InlineQueryResultArticle> inlineArticles = new List<InlineQueryResultArticle>();
            InputTextMessageContent contentBase = new InputTextMessageContent("sample msg content");
            contentBase.DisableWebPagePreview = false;
            InlineQueryResultArticle art = new InlineQueryResultArticle("1", "title", contentBase);
            art.Description = "Description";
            art.HideUrl = false;

            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton openButton = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton();
            openButton.Text = "Open smth";
            openButton.Url = "https://www.google.ru/webhp?client=opera&sourceid=opera";
            art.ReplyMarkup = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(openButton);

            inlineArticles.Add(art);

            await OwnerBot.BotClient.AnswerInlineQueryAsync(query.Id, inlineArticles.ToArray(), 10, switchPmText: "Switch to bot", switchPmParameter: "start");
            return BotCommandProcessResult.Succeeded;

        }
    }

}
