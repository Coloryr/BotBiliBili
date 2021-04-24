# BotBiliBili

一个用[ColorMirai](https://github.com/Coloryr/ColorMirai) 的B站机器人

## 启动
下载压缩包，解压到一个地方  
运行`BotBiliBili.exe`生成默认配置  
修改`config.json`中的`RunQQ`(运行的QQ号)、`RunGroup`(运行的群)  
重启程序

## 指令
控制台：
- `stop`：正常关闭应用
- `reload`：重读配置文件
- `test video [视频号]`：生成一个视频图片
- `test dynamic [动态号]`：生成一个动态图片
- `test duser [UID]`：生成UP主的最新动态图片
- `test live [直播间]`：生成直播间的图片

群里：(这里是默认指令，指令可以在配置文件里面修改)
- `#bili help`：获取帮助指令
- `#bili video [视频号]`：生成视频图片，AV号BV号均可
- `#bili nvideo [视频名]`：生成搜索后的视频图片
- `#bili dynmaic [动态号]`：生成动态图片
- `#bili duser [UID]`：生成UP主最新动态图片
- `#bili nuser [UP名字]`：生成UP主最新动态图片
- `#bili live [房间号]`：生成直播间图片
- `#bili nlive [UP名字]`：生成UP主的直播间图片
- `#bili ulive [UID]`：生成UP主的直播间图片
- `#bili suid [UID]`：订阅UP主的动态
- `#bili slive [UID]`：订阅UP主的直播

## 配置文件
1. `config.json`
```JSON
{
  "IP": "127.0.0.1",
  "Port": 23333,
  "RunQQ": 0,
  "RunGroup": [],
  "SESSDATA": "",
  "bili_jct": "",
  "Command": {
    "Head": "#bili",
    "Help": "help",
    "Video": "video",
    "VideoName": "nvideo",
    "Dynamic": "dynamic",
    "DynamicUser": "duser",
    "DynamicName": "nuser",
    "Live": "live",
    "LiveName": "nlive",
    "LiveUid": "ulive",
    "SubscribeUid": "suid",
    "SubscribeLive": "slive"
  },
  "CheckDelay": 1000,
  "TimeOut": 10,
  "AdminSubscribeOnly": true,
  "WaitTime": 60,
  "RequestHeaders": {
    "user-agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.72 Safari/537.36 Edg/90.0.818.42"
  }
}
```