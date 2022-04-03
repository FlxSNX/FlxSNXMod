# FlxSNXMod
基于 MelonLoader 的 CoreKeeper(地心护核者) 的作弊Mod  

![预览图](https://files.catbox.moe/wu5y4j.png)

## 功能
* 添加物品
* 召唤Boss
* 修改技能等级
* 增删Buff
* 地图传送 
* 雷霆光束
* 背包物品一键清空
* 背包物品一键999

## 说明
1. 这是我第一次尝试 C# 和 制作Mod
2. 为什么物品ID不使用文本输入框  
    - 不知道为何使用了 `GUILayout.TextField` 或 `GUI.TextField` 后就会报错  

3. 为什么不使用BepInEx  
    - 不懂用 BepInEx6 如何绘制 UI 所以选用了 MelonLoader

## 如何使用
1. 安装 [MelonLoader](https://github.com/LavaGang/MelonLoader) 到 `地心护核者` 的安装目录
2. 从 Steam 启动游戏
3. 使用 Visual Studio 打开项目
4. 从游戏目录下的 `MelonLoader\Managed` 里引入对应的程序集
5. 生成 FlxSNXMod

## 如何使用Mod
使用Mod前需先安装 MelonLoader  
1. 找到 `地心护核者` 的安装目录
2. 将生成的 `dll` 文件 放入 `Mods` 文件夹即可

## 参考
本项目参考的资料:  

[Unity游戏Mod/插件制作教程](https://www.bilibili.com/read/cv8997376?spm_id_from=333.999.0.0)  

[从0开始教你使用BepInEx为unity游戏制作插件Mod](https://mod.3dmgame.com/read/3)

[UnityMod开发教程 11 使用AssetBundle向游戏中加入自制资源](https://www.jianshu.com/p/2794c0c9d84b)

感谢以上各位大佬分享的教程~
