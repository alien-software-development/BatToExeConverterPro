# 👽 Alien BAT to EXE Converter Pro Studio v3.5 (Official Release)

<p align="center">
  <img src="Assets/banner.png" alt="Alien BAT to EXE Converter Pro Studio v3.5 Banner" width="100%">
</p>

[![Release](https://img.shields.io/badge/Release-v3.5.0%20Pro%20Studio-00ffaa.svg?style=for-the-badge)](https://github.com/alien-software-development/BatToExeConverterPro/releases)
[![Platform](https://img.shields.io/badge/Platform-Windows%2010%20%7C%2011%20%7C%20Server-00f0ff.svg?style=for-the-badge)](https://microsoft.com/windows)
[![Offline Installer](https://img.shields.io/badge/Installer-100%25%20Self--Contained-green.svg?style=for-the-badge)](Release_Build/Installer/BatToExePro_Setup.exe)
[![License](https://img.shields.io/badge/License-Commercial%20%2F%20Proprietary-purple.svg?style=for-the-badge)](LICENSE)

> **Official Binary Release Hub for Alien BAT to EXE Converter Pro Studio v3.5.**  
> Transform Windows batch (`.bat` / `.cmd`) scripts into armored, standalone, native Portable Executables (PE) with zero runtime dependencies. Runs instantly on brand-new, clean Windows machines with no additional software or .NET downloads required.

---

## ⚡ Direct Downloads (Zero Dependencies)

| Package | Type | Description | Download Link |
| :--- | :--- | :--- | :--- |
| **Offline Setup Wizard** | `.exe` (Installer) | Full Windows installer with UAC elevation, Start Menu, Desktop shortcuts, and Explorer Context Menu integration | [⬇ Download BatToExePro_Setup.exe](Release_Build/Installer/BatToExePro_Setup.exe) |
| **Portable Suite** | `.zip` (Portable) | 100% standalone portable package. Extract and run anywhere without installing | [⬇ Download AlienBatToExePro_v3.5_Portable.zip](Release_Build/AlienBatToExePro_v3.5_Portable.zip) |

---

## 🚀 Key Pro Capabilities

- 🔒 **Dynamic AES-256 Memory Encryption**: Your scripts, database passwords, and API credentials are encrypted with unique dynamic keys on each build. Decryption occurs purely in RAM and never writes unencrypted files to disk.
- 🛡 **Level 4 Quantum Polymorphic Armor**: Scrambles code structure, fragments variable tokens, and applies dynamic XOR byte-level obfuscation to defeat decompilers and static analysis.
- ⚡ **True Self-Contained Offline Architecture**: Complete Windows desktop runtime bundled inside the executable. No .NET runtime installation required on target client computers.
- 🛡 **UAC Administrator Elevation**: Easily configure executables to automatically request administrator rights (`requireAdministrator`) or run with current user permissions (`asInvoker`).
- 🎨 **12+ Cyber High-Res Preset Icons**: Built-in professional icon gallery (Alien Cyber, Shield Security, Terminal Prompt, Setup Package, Database Server, Folder Sync, Lock Crypto, etc.).
- 📦 **Embedded Asset Bundler**: Pack auxiliary scripts, config files, PowerShell routines, and DLLs into a single self-extracting, self-cleaning container.
- 🖱 **Windows Explorer Context Menu**: Right-click any `.bat` or `.cmd` file in Windows Explorer and choose **"👽 Convert to EXE with BatToExe Pro"** for instant 1-click compilation.

---

## 🔑 Asymmetric Ed25519 Cryptographic Licensing & Client Activation

The studio features an asymmetric cryptographic licensing and machine-binding architecture:
- **Client Application (`BatToExeConverter.exe`)**: Embeds only the Master Ed25519 Public Key for signature validation. Clients can view their hardware-bound machine fingerprint (HWID) and activate lifetime licenses via the built-in Activation Dialog. Client builds contain zero key generation capabilities or private keys.
- **Hardware-Locked Binding**: HWID is dynamically computed from Windows Registry `MachineGuid`, CPU architecture, processor count, and system identifiers.
- **Clock Rollback Protection**: Detects system clock manipulation to prevent trial period evasion.

---

## 🛡️ Windows Defender & Antivirus (How to Allow / Unblock)

Because compiled `.exe` files are generated locally and do not yet possess an Authenticode EV code-signing certificate or global Microsoft SmartScreen cloud reputation, Windows Defender or other antivirus software may occasionally flag them as a **false positive** (e.g. `Trojan:Win32/Wacatac!ml` or "Windows protected your PC").

If Windows Defender blocks the application or your compiled executables, follow these quick steps:

### Method 1: Bypassing Windows SmartScreen ("Windows protected your PC")
1. When the blue SmartScreen prompt appears, click **More info**.
2. Click **Run anyway** at the bottom.

### Method 2: Allowing a Blocked File in Windows Security
1. Open the Windows Start menu and search for **Windows Security**.
2. Go to **Virus & threat protection**.
3. Under *Current threats*, click **Protection history**.
4. Locate the blocked item (e.g., `BatToExeConverter.exe` or your compiled file).
5. Click **Actions** &rarr; select **Allow on device** (or **Restore**).

### Method 3: Adding a Folder Exclusion (Recommended for Development)
To prevent Windows Defender from continuously scanning or deleting executables in your workspace or output folder:
1. Open **Windows Security** &rarr; **Virus & threat protection**.
2. Under *Virus & threat protection settings*, click **Manage settings**.
3. Scroll down to **Exclusions** and select **Add or remove exclusions**.
4. Click **Add an exclusion** &rarr; choose **Folder**.
5. Select the application directory or your output directory.

#### Optional (PowerShell - Administrator):
```powershell
Add-MpPreference -ExclusionPath "C:\Path\To\Your\Application_Or_Output_Folder"
```

---

## 📦 System Requirements

- **Supported Operating Systems**: Windows 11, Windows 10, Windows Server 2016/2019/2022 (64-bit and 32-bit).
- **Prerequisites**: **None!** (The package is 100% self-contained and pre-configured for instant execution).

---

## 💼 Commercial Licensing & Custom Inquiries

For enterprise licensing, white-label distribution, or customized compiler modules:

- 💬 **WhatsApp Support**: [+8801710978997](https://wa.me/8801710978997)
- 🌐 **Official Website**: [aliensoftwaredevelopment.com](https://aliensoftwaredevelopment.com)
- 🏢 **GitHub Organization**: [@alien-software-development](https://github.com/alien-software-development)

&copy; 2026 Alien Software Development. All rights reserved.
