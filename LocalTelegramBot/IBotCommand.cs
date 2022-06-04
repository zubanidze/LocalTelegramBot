using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot
{
    interface IBotCommand
    {
        TgBot OwnerBot { get; }
        string CommandQuery { get; }
        string UserFriendlyNameOfCommand { get; }
        string CommandDescription { get; }
        BotCommandType CommandType { get; }
        List<BotUserRole> CommandAvailableTo { get; }
        bool CanBeExecutedInGroupChat { get; }
        Task<BotCommandProcessResult> Process(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args);
        Task<bool> CanBeExecuted(TelegramChat chat, Telegram.Bot.Types.User user, object e, object[] args);
    }

    enum BotCommandType
    {
        MessageCommand = 0, SlashCommand = 1, InlineCommand = 2, CallbackCommand = 3, Hybrid = 4
    }

    enum BotCommandProcessResult
    {
        Succeeded = 0, IncorrectArguments = 1, ErrorOccurred = 2, Canceled = 3, Abort = 4, CannotBeExecuted = 5, SucceededAndPromptLeavingChain = 6, RetryRequired = 7, NoPermission = 8
    }
}
