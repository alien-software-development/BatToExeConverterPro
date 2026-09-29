# 👽 Alien BAT to EXE Converter Pro Studio v3.5

[![.NET Version](https://img.shields.io/badge/.NET-7.0%20Windows-blue.svg)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Proprietary-00ffaa.svg)](LICENSE)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20%7C%20Server-00f0ff.svg)](https://microsoft.com/windows)
[![Release](https://img.shields.io/badge/Release-v3.5.0%20Pro%20Studio-green.svg)](Release_Build/Installer/BatToExePro_Setup.exe)

> **Enterprise-grade Native C# PE Executable Compiler & Armor Suite for Windows Batch (`.bat` / `.cmd`) scripts.**  
> Developed with futuristic Alien obsidian cyberpunk aesthetics, AES-256 dynamic encryption, in-memory deobfuscation, UAC administrator privilege management, and standalone offline deployment.

---

## ⚡ Core Features

- 🔒 **Dynamic AES-256 Memory Encryption**: Protect batch source code, passwords, database credentials, and internal API keys. Decryption occurs solely inside RAM—never writing unencrypted scripts to disk.
- 🛡 **Quantum Polymorphic Armor (Levels 1–4)**:
  - **Level 1 (Basic)**: Strips comments, redundant whitespaces, and normalizes execution.
  - **Level 2 (Advanced)**: Variable fragmentation, environment token slicing (`%var:~0,0%`), and randomized dead-code injection.
  - **Level 3 (Ultra Armor)**: Base64 tokenized memory decompression via PowerShell execution vector.
  - **Level 4 (Quantum Armor)**: Dynamic XOR byte ciphering with polymorphic decryption keys calculated at runtime.
- ⚡ **Native C# PE Pipeline**: Generates authentic Windows Portable Executable (PE) 32-bit (`x86`) and 64-bit (`x64`) binaries using the native Windows C# compiler (`csc.exe`).
- 🛡 **UAC Administrator Elevation**: Configure executables to enforce `requireAdministrator` or `asInvoker` via embedded application manifests.
- 🎨 **12+ Cyber Preset Icons**: Choose from high-resolution application icons (Alien Cyber, Shield Security, Terminal Prompt, Setup Package, Database Server, Folder Sync, Lock Crypto, etc.) or load your own `.ico`.
- 📦 **Embedded Dependency Bundler**: Embed secondary files, DLLs, PowerShell scripts, images, and config files into the executable container.
- 🖥 **Multiple Execution Modes**: Run in visible console mode, completely invisible background daemon mode (`winexe`), minimized, or maximized.
- 🔑 **Cryptographic Machine Licensing**: Built-in hardware-bound key verification system with `AlienKeygen.exe`.
- 📦 **Offline Setup Wizard (`BatToExePro_Setup.exe`)**:
  - Full administrator privilege detection and UAC elevation support.
  - Installs cleanly to `Program Files` or `%LocalAppData%`.
  - Integrates `"👽 Convert to EXE with BatToExe Pro"` directly into the Windows Explorer right-click context menu.
  - Creates Desktop and Start Menu program entries with a full uninstaller.

---

## 🏗 Solution Architecture

```
bat_to_exe/
├── BatToExeConverter/           # Main WinForms Pro Studio Application
│   ├── MainForm.cs             # Obsidian cyber UI, multi-tab views, terminal log
│   ├── BatCompilerEngine.cs    # C# PE compilation pipeline & manifest injector
│   ├── BatchObfuscator.cs      # Polymorphic XOR & fragmentation armor engine
│   ├── UITheme.cs              # Modern Alien dark styling system
│   ├── LicenseManager.cs       # Cryptographic hardware-locked licensing
│   ├── KeygenForm.cs           # Keygen UI component
│   ├── PresetIconPicker.cs     # 12+ Pro cyber icon selector
│   └── ScriptTemplates.cs      # Curated enterprise script templates
├── BatToExeSetup/               # Offline Setup Wizard with UAC Elevation
│   ├── app.manifest            # UAC requireAdministrator manifest
│   ├── InstallerForm.cs        # Offline payload extractor, shell registrar
│   └── Program.cs              # Entry point with silent flags
├── AlienKeygen/                 # Standalone Admin Keygen Tool
├── Assets/                      # Icons, presets & branding graphics
│   └── PresetIcons/            # 12 ready-to-use high-res .ico assets
├── Release_Build/               # Automated production release outputs
│   ├── BatToExePro_Standalone/ # Standalone compiled Studio
│   ├── AlienKeygen_Standalone/ # Standalone keygen binary
│   └── Installer/              # BatToExePro_Setup.exe
├── index.html                   # Ultra-modern product web showcase
├── build_release.ps1            # Automated PowerShell release builder
└── build_release.bat            # 1-click batch build launcher
```

---

## 🚀 1-Click Automated Build

To build the entire suite (Standalone Studio, Standalone Keygen, and the Offline Setup Wizard with embedded binaries):

```powershell
# Run using PowerShell
.\build_release.ps1
```
Or double-click `build_release.bat` in Windows Explorer.

---

## 📦 Offline Installation

1. Run `Release_Build\Installer\BatToExePro_Setup.exe`.
2. Windows will automatically prompt for Administrator privileges (UAC) to grant installation permissions for `C:\Program Files`.
3. Choose the destination folder and click **INSTALL NOW**.
4. The setup wizard extracts all required binaries, registers the Windows Explorer context menu, and generates Desktop shortcuts.

---

## 🔒 Security & Intellectual Property

Compiled batch applications are shielded with dynamic runtime key generation. Anti-debugging and anti-analysis modules detect common reverse-engineering tools (`x64dbg`, `ida64`, `wireshark`, `processhacker`) and terminate execution if tampering is detected.

---

## 👨‍💻 Author & Maintainer

**Alien Software Development**  
- Website: [aliensoftwaredevelopment.com](https://aliensoftwaredevelopment.com)  
- WhatsApp Support: [+8801710978997](https://wa.me/8801710978997)  
- GitHub: [@alien-software-development](https://github.com/alien-software-development)  

&copy; 2026 Alien Software Development. All rights reserved.
