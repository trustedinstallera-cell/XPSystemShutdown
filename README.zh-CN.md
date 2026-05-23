# XP_SystemShutdown
[英文]()

基于 C#/.NET 构建的 XP 风格系统关机对话框。模仿旧版 Windows 用户界面并执行真实的关机命令。
## 先决条件
.NET framework 4.0 or 或更高， 在Windows 8 及以上的操作系统中内置. 最低系统要求是 Windows XP 但无必要。
请保证您的账户拥有关机权限。
> [!Note]
> 若要更改显示内容，修改 release 文件中随附的 lang.ini 即可

## 用法
参见 shutdown.exe的[文档](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/shutdown)。程序语法与前者完全一致。 也可以查看 [快速开始](# 快速开始) 以查看简单示例。

## 快速开始
### 下载
前往 [release 页面](https://github.com/trustedinstallera-cell/XPSystemShutdown/releases) 或直接从 [最新版本](https://github.com/trustedinstallera-cell/XPSystemShutdown/releases/latest) 中下载内容.
### 运行
使用与 shutdown 命令相同的语法执行程序。

以下所有命令均可以单独执行。:: 符号之后的内容不必输入。
所有 `-?` 参数均可以被 `/?` 替代 (? 指单个字符)。
``` bat
shutdown -s -t 3600  :: 1小时 (3600 秒) 后关闭计算机。
shutdown -r -t 0     :: 立刻重新启动计算机。将不会显示任何内容。
shutdown -a          :: 取消关机进程。
```
> [!Important]
> 反病毒软件应该不会对该程序作出反应。如果有反应，您应该检查文件来源或添加到反病毒软件排除项。

## 编译提示
您需要 Visual Studio 2019 或以上的编译器和带有 4.0 版本或以上的 .NET Framework 开发工具包。您可以仅下载 /Code/ 中的文件。
本地化无需重编译。您可以使用随附于二进制文件的 lang.ini 进行修改。
## 许可证信息
MIT 许可证. 未经任何修改。参见 [这里](https://github.com/trustedinstallera-cell/XPSystemShutdown/blob/main/LICENSE) 以查看细节部分。
