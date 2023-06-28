# xsf (Xoen's Simple game Framework)
`xsf (Xoen's Simple game Framework) is an open source, minimalist game development framework. For many small businesses or organizations, a game framework is the last thing they need to spend their time on. I wanted to provide a simple infrastructure for game development, both client-side and server-side.`

xsf（Xoen's Simple Framework）是一个开源的简约的游戏开发框架。对于很多小型企业或者小组织来说，游戏框架是最不值得花时间去做的一个轮子，我期望可以提供一个简单的游戏开发基础框架，包含客户端和服务端双端的框架。

`The client is based on Unity3D (2021.3) and xlua(v2.1.16 with ndk r21b) lua-protobuf(0.4.0), while the server is based on C++ and lua. You can adjust and delete the corresponding modules as needed. I may also provide a Go-based server framework in the future if I have time.`

客户端是建立在Unity3D（2021.3）和[xlua(v2.1.16 with ndk r21b)](https://github.com/Tencent/xLua)以及[lua-protobuf(0.4.0)](https://github.com/starwing/lua-protobuf)的基础上，而服务器是建立在C++和lua的基础上。你可以根据需要自行调整和删减对应的模块。后续有时间的话，我可能还会提供一个基于go的服务器框架。

`In retrospect, I have been in the industry for more than 10 years, and so far, no project has been successful, but I still have not given up and continue to move forward. So that's one of the reasons I open source in the first place, so that you can spend more time making game content, rather than building the underlying framework. In comparison, I think game content development is worth spending more time on.`

回想起来，我从业10多年，至今未有项目能大成，不过我仍未放弃，仍在往前不停。所以这也是我开源的一个初衷，期望你能有更多的时间花在游戏内容制作上，而不是在底层框架构建上。相比较而言，我认为游戏内容开发更值得花费更多的时间。

`It should be noted that since there are now pure C# hot update solutions such as hybridclr, I have made a separate branch of lua support: xlua, and the main branch switches to the pure C# version. This requires you to integrate your hybridclr.`

需要注意的是，因为现在有[hybridclr](https://github.com/focus-creative-games/hybridclr)这样的纯C#的热更新解决方案，所以我把支持lua的方式单独做了一个分支：xlua，main分支就切换纯C#版本了。这需要你自行集成hybridclr。



`The whole project is based on MIT open source protocol, please feel free to eat.`

整个项目基于MIT开源协议，请放心食用。

`All English instructions are translated by Youdao Translator.Take your time.`

所有英文说明由有道翻译翻译而来，将就看。
