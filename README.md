# PboSpy

PboSpy is graphical tool for exploring [BI PBO files](https://community.bistudio.com/wiki/PBO_File_Format)

It allows you to walk through ArmA or DayZ (and mods) PBOs to check for configuration and files.
It also allows you to quickly check if packaging and macros have worked the expected way.

PboSpy can be used to unpack PBOs or extract individual files.
**In that case, please check the licensing of files to ensure you are allowed to extract any data.**

![PboSpy](images/PboSpy.png)

In case of bugs or feature requests, please send them via [Issues](https://github.com/rvost/PboSpy/issues). 
Any feedback is highly appreciated!

Please give this project a ⭐ if you find it useful!

## Download

You can get [latest](https://github.com/rvost/PboSpy/releases/latest) version on [Releases page](https://github.com/rvost/PboSpy/releases). 
Pick one `.exe` as described below. 
Other files required for updates and you don't need to download them manually.

## Installation

Since v0.6.0 PboSpy comes in two version. 
`PboSpyPortable.exe` is ready-to-run portable executable that however will not receive updates. 
`PboSpySetup.exe` is one-click installer for distribution with auto-updates. 

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details

## Acknowledgments

This project is fork of  [Julien Etelain (GrueArbre)](https://github.com/jetelain) [PboExplorer](https://github.com/jetelain/PboExplorer) with most of the core code remained intact.

PboSpy is powered by [Braini01's bis-file-formats](https://github.com/Braini01/bis-file-formats), an indispensable library for working with BI formats in .NET

The [armake2](https://github.com/KoffeinFlummi/armake2) source code is used as a reference for understanding the BI signature (`.bisign`) and public key (`.bikey`) formats.

GUI built with [Tim Jones's Gemini framework](https://github.com/tgjones/gemini), released under the Apache 2.0 license.

Preview of  signatures, keys and other binary files implemented with [Derek Tremblay's WPF Hexeditor](https://github.com/abbaye/WpfHexEditorControl), released under the Apache 2.0 license.

Implementation of config search is based on [article](https://www.codeproject.com/Articles/1213031/Advanced-WPF-TreeViews-in-Csharp-VB-Net-Part-of-n) by Dirk Bahle and Alaa Ben Fatma and uses Dirk Bahle's [TreeLib](https://github.com/Dirkster99/TreeLib).
