using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BatToExeConverter;

public enum ObfuscationLevel
{
    None = 0,
    Basic = 1,          // Strips comments and blanks, adds random noise
    Advanced = 2,       // Variable fragmentation & character slicing
    UltraArmor = 3,     // Dynamic Base64 / Hex memory tokenization
    QuantumArmor = 4    // Dynamic XOR Polymorphic Tokenization & In-Memory Execution
}

public static class BatchObfuscator
{
    private static readonly Random Rnd = new();

    public static string Obfuscate(string batchScript, ObfuscationLevel level)
    {
        if (string.IsNullOrWhiteSpace(batchScript) || level == ObfuscationLevel.None)
        {
            return batchScript;
        }

        return level switch
        {
            ObfuscationLevel.Basic => ObfuscateBasic(batchScript),
            ObfuscationLevel.Advanced => ObfuscateAdvanced(batchScript),
            ObfuscationLevel.UltraArmor => ObfuscateUltra(batchScript),
            ObfuscationLevel.QuantumArmor => ObfuscateQuantum(batchScript),
            _ => batchScript
        };
    }

    private static string ObfuscateBasic(string input)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@echo off");

        using var reader = new StringReader(input);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            if (trimmed.StartsWith("::") || trimmed.StartsWith("rem ", StringComparison.OrdinalIgnoreCase))
            {
                continue; // Strip comments
            }

            sb.AppendLine(line);
        }

        return sb.ToString();
    }

    private static string ObfuscateAdvanced(string input)
    {
        var sb = new StringBuilder();
        sb.AppendLine("@echo off");
        string junkVar = GetRandomVarName(6);
        sb.AppendLine($"set {junkVar}={GetRandomHex(8)}");

        using var reader = new StringReader(input);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            string trimmed = line.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;
            if (trimmed.StartsWith("::") || trimmed.StartsWith("rem ", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // Fragment common keywords with empty %junkVar:~0,0%
            string fragmented = FragmentLine(trimmed, junkVar);
            sb.AppendLine(fragmented);
        }

        return sb.ToString();
    }

    private static string FragmentLine(string line, string varName)
    {
        if (line.StartsWith("@") || line.StartsWith(":") || line.StartsWith("set ", StringComparison.OrdinalIgnoreCase))
            return line;

        var sb = new StringBuilder();
        int charCount = 0;
        foreach (char c in line)
        {
            sb.Append(c);
            charCount++;
            if (charCount % 4 == 0 && c != '%' && c != '^' && c != '"' && c != '!' && char.IsLetterOrDigit(c))
            {
                sb.Append($"%{varName}:~0,0%");
            }
        }
        return sb.ToString();
    }

    private static string ObfuscateUltra(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        string base64 = Convert.ToBase64String(bytes);

        string tempVar = GetRandomVarName(7);
        string psVar = GetRandomVarName(6);

        var sb = new StringBuilder();
        sb.AppendLine("@echo off");
        sb.AppendLine($"rem :: Alien Obfuscation Armor v3.5");
        sb.AppendLine($"set {tempVar}={base64}");
        sb.AppendLine($"set {psVar}=[System.Text.Encoding]::UTF8.GetString([System.Convert]::FromBase64String(\"%{tempVar}%\"))");
        sb.AppendLine($"powershell.exe -NoProfile -ExecutionPolicy Bypass -Command \"& {{ Invoke-Expression (%{psVar}%) }}\"");
        sb.AppendLine($"set {tempVar}=");
        sb.AppendLine($"set {psVar}=");

        return sb.ToString();
    }

    private static string ObfuscateQuantum(string input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte key = (byte)Rnd.Next(1, 255);
        byte[] encrypted = new byte[bytes.Length];
        for (int i = 0; i < bytes.Length; i++)
        {
            encrypted[i] = (byte)(bytes[i] ^ key);
        }

        string hexCipher = BitConverter.ToString(encrypted).Replace("-", "");
        string varKey = GetRandomVarName(6);
        string varHex = GetRandomVarName(8);
        string varDeob = GetRandomVarName(7);

        var sb = new StringBuilder();
        sb.AppendLine("@echo off");
        sb.AppendLine("rem :: Alien Quantum Armor v3.5 - Polymorphic Memory Execution");
        sb.AppendLine($"set {varKey}={key}");
        sb.AppendLine($"set {varHex}={hexCipher}");
        sb.AppendLine($"powershell.exe -NoProfile -ExecutionPolicy Bypass -Command \"& {{ $h='%{varHex}%'; $k=[byte]%{varKey}%; $b=for($i=0;$i -lt $h.Length;$i+=2){{[Convert]::ToByte($h.Substring($i,2),16) -bxor $k}}; [System.Text.Encoding]::UTF8.GetString([byte[]]$b) | Invoke-Expression }}\"");
        sb.AppendLine($"set {varKey}=");
        sb.AppendLine($"set {varHex}=");

        return sb.ToString();
    }

    private static string GetRandomVarName(int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(chars[Rnd.Next(chars.Length)]);
        }
        return sb.ToString();
    }

    private static string GetRandomHex(int length)
    {
        const string hexChars = "0123456789ABCDEF";
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append(hexChars[Rnd.Next(hexChars.Length)]);
        }
        return sb.ToString();
    }
}
