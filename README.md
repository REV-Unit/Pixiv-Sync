# PixivSync

用Aria2快速下载Pixiv收藏并保存信息到SQLite数据库的工具。

## 为什么？（个人意见）

* 有时插画会因为不明原因被删除，然后这个插画的信息就只能到其他地方获取了。
* 其他工具的速度不够在合理时间内处理完大量收藏（而且数据库不全）。
* Pixiv的收藏管理蛋疼。

## 配置

第一次使用时，运行一次程序会生成空配置文件`config.yml`。

```yaml
Log:
  Dir: # 日志目录，方便查错
  Level: Information # 日志级别
Auth:
  Id: 114514 # 你的Pixiv ID，用于获取收藏列表
  Cookie: PHPSESSID=xxxxxx # PHPSESSID可以打开浏览器用F12查找
  AuxCookie: PHPSESSID=xxxxxx # 副账号PHPSESSID，用于下载以及爬取插画信息。如果你有小号可以填上小号的PHPSESSID，防止大号因为访问过多被Pixiv警告或者封了。
Aria2:
  JsonRpcUrl: http://127.0.0.1:6800 # Aria2 JsonRpc地址
  RpcSecret: # 如果用了RpcSecret授权就填上
StoragePath: '' # 下载目录
DbPath: '' # 数据库保存路径
UsePrivateBookmarks: false # 使用私密收藏？
```

配置好之后再运行即可。

## 命令行

`-m` 合并模式。将会爬取所有收藏并把最新的信息合并入数据库，以记录画师改名，插画增加页等情况。会花一点时间。

## 数据库

保存了插画标签、画师名称、限制级、插画各页的各种尺寸的图片链接等信息。

## 可能问题

* 暂不支持动图下载。
* 下载路径格式暂时固定为`{画师名}_{画师ID}/{插画ID}_{插画页号}.{扩展名}`。如果你已经在本地有了下载，请确保相对下载目录的路径是这种格式。

## License

Copyright (C) 2022 RcINS

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License Version 3 as published by
the Free Software Foundation.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
