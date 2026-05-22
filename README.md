# XP_SystemShutdown
[简体中文]()

XP-inspired system shutdown dialog built in C#/.NET. Mimics old Windows UI and triggers real shutdown commands.

## Prerequisite
.NET framework 4.0 or higher, which is built-in in Windows 8 or higher operating system. The minimum system is Windows XP but not necessary.
Make sure your account has privilege for shutting down your computer.
> [!Note]
> You might need to translate to your own language by editing lang.ini provided with the binary file.

## Usage
See [documents](https://learn.microsoft.com/en-us/windows-server/administration/windows-commands/shutdown) in shutdown.exe. The syntax is completely the same. Or see [quick start](#quick start) for simple commands.

## Quick Start
### Download
Goto [release page](https://github.com/trustedinstallera-cell/XPSystemShutdown/releases) or download directly from [latest release](https://github.com/trustedinstallera-cell/XPSystemShutdown/releases/latest).
### Run
Run XP_Shutdown.exe in command prompt with shutdown syntax.

All the commands below can run individually and no need to type texts after :: symbol.
All the `-?` parameters can be replaced by `/?` (? means a single character).
``` bat
shutdown -s -t 3600  :: Shutdown after an hour (3600 seconds)
shutdown -r -t 0     :: Immediately restart. Nothing will be shown.
shutdown -a          :: Cancel shutdown process.
```
> [!Important]
> Usually anti-virus software shouldn't react on the files. But if it does, you may check the source of the files or add to exclusion items.

## Compilation message
You'll need Visual Studio 2019 with .NET Framework 4.0 Development Kit or a higher version for building. Download files in /Code/ is enough.
No need to recompile if just for localisation. You can use lang.ini provided with the binary file instead.

## Licence
MIT licence. Using unchanged version. See [here](https://github.com/trustedinstallera-cell/XPSystemShutdown/blob/main/LICENSE) for details.
