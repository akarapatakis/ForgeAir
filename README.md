
# ForgeAir

<p align="center">
  <img src="docs/assets/readme-banner.png" width="100%" alt="ForgeAir Banner">
</p>
<h2>🚧 Due to my Panhellenic exams, I am currently unable to develop ForgeAir
actively. </h2>

**ForgeAir** is a modern, modular and lightweight radio automation system built
with C# and WPF. Designed for my use case mainly but that does not limit you in
choosing ForgeAir.

---

## ✨ Features

- 🎵 High-performance audio engines (BASS & NAudio) with VST Plugin support
 (limited to BASS only) with support for WaveOut, DirectSound and WASAPI (ASIO
 support only on NAudio)
- 🎙️ Sweeper overlays, FX, rebroadcasting, crossfading, and streaming support
- 📞 Audience messages by SMS via Android using [ForgeCrowd](https://github.com/akarapatakis/ForgeCrowd)
- 🛠 Ability to create plugins using an [SDK](https://github.com/akarapatakis/ForgePluginSDK)
- 💾 RDS/RBDS, DAB+ PAD and Internet Text
- 🧠 Clean MVVM architecture using Caliburn.Micro
- ⚡ Friendly configuration through INI
- 👀 Small but handy features like Traffic Announcemnts, Dynamic Jingles,
Temperature etc.

---

## 📸 Screenshots

<p align="center">
  <img src="docs/assets/activitycenter-gr.png" width="100%" alt="ForgeAir Banner">
</p>

<p align="center">
  <img src="docs/assets/multistation.png" width="100%" alt="ForgeAir Banner">
</p>

---

## 🚀 Getting Started

### Requirements

- x64 machine
- .NET 6 SDK
- Visual Studio 2022
- Telegram Bot
- WeatherAPI Account
- MySQL/MariaDB server (for track database)

### Building

- You need to create an account to [WeatherAPI](https://www.weatherapi.com/) in
order to make use of the temperature feature in ForgeAir
- You must provide your own BASS libraries from  
[Un4Seen Developments](https://www.un4seen.com/)
provided you already have a commercial license bought
- For crash reporting, ForgeAir uses a Telegram bot. You must create yourself a
 bot to make use of it

```bash
git clone https://github.com/akarapatakis/ForgeAir.git
cd ForgeAir
- Create a .env file and add your WeatherAPI and Telegram bot secrets
WEATHERAPI_KEY=XXXXXXXXXXXXXXXXXXXXXXXXXXXXX
CRASH_REPORTER_TELEGRAM_BOT_TOKEN=XXXXXXXXXXXXX
CRASH_REPORTER_TELEGRAM_CHATID=XXXXXXXXXX

Open ForgeAir.sln in Visual Studio and build or use MSBuild 
```

---

### Credits

Thanks to these people/groups for helping through ForgeAir's development:

- @andreiv1  for helping in early development and helping me write better code :P

...and to all of you who use ForgeAir (?)
