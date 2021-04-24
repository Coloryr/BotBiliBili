# BotBiliBili

一个用[ColorMirai](https://github.com/Coloryr/ColorMirai) 的B站机器人  
交流群号：[571239090](https://qm.qq.com/cgi-bin/qm/qr?k=85m_MZMJ7BbyZ2vZW4wHVZGGvGnIL2As&jump_from=webapi)

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
1. 主要配置`config.json`
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
- `IP`：机器人IP
- `Port`：端口
- `RunQQ`：运行的QQ
- `RunGroup`：运行群号
- `SESSDATA`：B站cookie
- `bili_jct`：B站cookie
- `Command`：命令
  - `Head`：命令头
  - `Help`：帮助指令
  - `Video`：视频指令
  - `VideoName`：视频指令
  - `Dynamic`：动态指令
  - `DynamicUser`：动态指令
  - `DynamicName`：动态指令
  - `Live`：直播指令
  - `LiveName`：直播指令
  - `LiveUid`：直播指令
  - `SubscribeUid`：订阅动态指令
  - `SubscribeLive`：订阅直播指令
- `CheckDelay`：爬虫延迟（毫秒）
- `TimeOut`：爬虫超时时间（秒）
- `AdminSubscribeOnly`：是否管理员才能订阅
- `WaitTime`：爬虫冷却时间（秒）
- `RequestHeaders`：爬虫请求头

2. 图片配置  
视频图片`video.json`
```JSON
{
  "TitlePos": {
    "X": 20.0,
    "Y": 155.0
  },
  "TitleSize": 20.0,
  "TitleColor": "#000000",
  "TitleLim": 20,
  "StatePos": {
    "X": 30.0,
    "Y": 195.0
  },
  "StateSize": 15.0,
  "StateColor": "#000000",
  "PicPos": {
    "X": 20.0,
    "Y": 230.0
  },
  "PicWidth": 560,
  "PicHeight": 560,
  "InfoPos": {
    "X": 20.0,
    "Y": 600.0
  },
  "InfoSize": 20.0,
  "InfoColor": "#000000",
  "InfoLim": 20,
  "InfoDeviation": 40,
  "InfoLeft": 20,
  "BackGround": "#F5F5F5",
  "Font": "微软雅黑",
  "Width": 600,
  "Height": 800,
  "QBack": "#F8F8FF",
  "QPoint": "#0000CD",
  "QPos": {
    "X": 460.0,
    "Y": 20.0
  },
  "QSize": 120,
  "HeadPic": {
    "X": 20.0,
    "Y": 20.0
  },
  "HeadPicSize": 120.0,
  "NamePos": {
    "X": 155.0,
    "Y": 20.0
  },
  "NameSize": 20.0,
  "NameColor": "#FF6A6A",
  "UidPos": {
    "X": 160.0,
    "Y": 75.0
  },
  "UidSize": 15.0,
  "UidColor": "#363636"
}
```
直播图片`live.json`
```JSON
{
  "StatePos": {
    "X": 30.0,
    "Y": 190.0
  },
  "StateSize": 15.0,
  "StateColor": "#000000",
  "TitlePos": {
    "X": 20.0,
    "Y": 150.0
  },
  "TitleSize": 20.0,
  "TitleColor": "#000000",
  "TitleLim": 20,
  "LivePos": {
    "X": 160.0,
    "Y": 120.0
  },
  "LiveSize": 15.0,
  "LiveColor": "#000000",
  "TextLeft": 30,
  "PicPos": {
    "X": 20.0,
    "Y": 220.0
  },
  "PicWidth": 560,
  "PicHeight": 560,
  "InfoPos": {
    "X": 20.0,
    "Y": 540.0
  },
  "InfoSize": 20.0,
  "InfoColor": "#000000",
  "InfoLim": 20,
  "InfoDeviation": 40,
  "BackGround": "#F5F5F5",
  "Font": "微软雅黑",
  "Width": 600,
  "Height": 800,
  "QBack": "#F8F8FF",
  "QPoint": "#0000CD",
  "QPos": {
    "X": 460.0,
    "Y": 20.0
  },
  "QSize": 120,
  "HeadPic": {
    "X": 20.0,
    "Y": 20.0
  },
  "HeadPicSize": 120.0,
  "NamePos": {
    "X": 155.0,
    "Y": 20.0
  },
  "NameSize": 20.0,
  "NameColor": "#FF6A6A",
  "UidPos": {
    "X": 160.0,
    "Y": 75.0
  },
  "UidSize": 15.0,
  "UidColor": "#363636"
}
```
动态图片`dynamic.json`
```JSON
{
  "StatePos": {
    "X": 30.0,
    "Y": 150.0
  },
  "StateSize": 15.0,
  "StateColor": "#000000",
  "PicStart": {
    "X": 20.0,
    "Y": 180.0
  },
  "PicWidth": 560,
  "PicPid": 10,
  "TextX": 20,
  "TextPid": 20,
  "TextSize": 20,
  "TextLim": 20,
  "TextDeviation": 40,
  "TextLeft": 30,
  "TextColor": "#000000",
  "BackGround": "#F5F5F5",
  "Font": "微软雅黑",
  "Width": 600,
  "Height": 800,
  "QBack": "#F8F8FF",
  "QPoint": "#0000CD",
  "QPos": {
    "X": 460.0,
    "Y": 20.0
  },
  "QSize": 120,
  "HeadPic": {
    "X": 20.0,
    "Y": 20.0
  },
  "HeadPicSize": 120.0,
  "NamePos": {
    "X": 155.0,
    "Y": 20.0
  },
  "NameSize": 20.0,
  "NameColor": "#FF6A6A",
  "UidPos": {
    "X": 160.0,
    "Y": 75.0
  },
  "UidSize": 15.0,
  "UidColor": "#363636"
}
```
关键词：
- `Title`：标题
- `State`：状态
- `Pos`：渲染坐标
- `Size`：字体大小
- `Color`：字体颜色
- `Pic`：图片内容
- `Lim`：文本字数限制(一行)
- `Deviation`：字间距(垂直)
- `Left`：字右边距
- `BackGround`：背景颜色(底色)
- `Font`：字体
- `Width`：图片原始宽度
- `Height`：图片原始大小
- `QBack`：二维码底色
- `QPoint`：二维码点的颜色
- `Head`：头像
- `Name`：名字
- `Uid`：UID
- `Live`：房间号
- `Info`：信息

3. 订阅配置`subscribes.json`
```JSON
{
  "Uids": {
    "xxxx":[
      xxxx
    ]
  },
  "Lives": {
    "xxxx":[
      xxxx
    ]
  }
}
```
- `Uids`：动态订阅
- `Lives`：直播订阅  
格式：
键为`UID`值为一个列表(List)里面填的是群号

4. 信息暂存`temp.json`
```JSON
{
  "Dynamic": {
    "xxxx": "xxxx"
  },
  "Live": {
    "xxxx": true
  }
}
```
- `Dynamic`：动态暂存
- `Live`：直播暂存  
格式：
键为`UID`值为最后信息

## 图片缓存
- `Dynamic`文件夹 动态图片缓存
- `Live`文件夹 直播图片缓存
- `Video`文件夹 视频图片缓存
