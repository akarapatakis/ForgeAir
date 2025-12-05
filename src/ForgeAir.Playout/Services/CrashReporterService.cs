using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.Services
{
    public class CrashReporterService
    {
        private readonly string TelegramChatID;
        private readonly string TelegramBotToken;
        private readonly HttpClient client = new();
        public CrashReporterService()
        {
            DotNetEnv.Env.TraversePath().Load();
            TelegramChatID = System.Environment.GetEnvironmentVariable("CRASH_REPORTER_TELEGRAM_CHATID");
            TelegramBotToken = System.Environment.GetEnvironmentVariable("CRASH_REPORTER_TELEGRAM_BOT_TOKEN");
        }

        public async void Report(Exception ex)
        {
            // change the station name later
            string messageModel = $"""
            📟 *Crash Report from Flash 99.2 Chania*

            🆔 *App Version:* 1.0.0  
            💻 *Platform:* {Environment.OSVersion}  
            🕒 *Time (UTC):* {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}

            📌 *Exception Type:* {ex.GetType().Name}  
            ⚠️ *Message:* {ex.Message}

            🔍 *Location:*  
            • Method: `{ex.TargetSite}`  
            • Source: `{ex.Source}`

            🧠 *Stack Trace:*  
            ```{ex.StackTrace}```

            📎 *HResult:* {ex.HResult}  
            🧩 *Inner Exception:* {(ex.InnerException?.Message ?? "None")}  
            🔗 *Help Link:* {(ex.HelpLink ?? "N/A")}  
            🧾 *Additional Data:* {(ex.Data?.Count > 0 ? string.Join("\n", ex.Data.Cast<DictionaryEntry>().Select(e => $"• {e.Key.GetType().Name}: {e.Value}")) : "None")}
            """;


            var response = await client.PostAsync($"https://api.telegram.org/bot{TelegramBotToken}/sendMessage?chat_id={TelegramChatID}&text={messageModel}&parse_mode=Markdown", null).ConfigureAwait(false);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                new ToastContentBuilder()
                .AddText("ForgeAir ran into an unexpected error!")
                .AddText("The error has been reported to the developers to imporve the app! \nThanks for your help!")
                .Show();
            }

        }
    }
}
