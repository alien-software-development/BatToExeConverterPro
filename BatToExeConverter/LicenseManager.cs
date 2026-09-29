using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Win32;

namespace BatToExeConverter;

public enum LicenseType
{
    Trial,
    PaidLifetime,
    Expired,
    Invalid
}

public class LicenseStatus
{
    public LicenseType Type { get; set; }
    public string MachineId { get; set; } = string.Empty;
    public string RegisteredName { get; set; } = "Free User";
    public int DaysRemaining { get; set; }
    public int DailyConversionsUsed { get; set; }
    public int DailyConversionsRemaining { get; set; }
    public const int MaxDailyTrialConversions = 3;
    public DateTime? TrialStartDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive => Type == LicenseType.PaidLifetime || (Type == LicenseType.Trial && DaysRemaining > 0);
    public bool CanConvertToday => Type == LicenseType.PaidLifetime || (Type == LicenseType.Trial && DaysRemaining > 0 && DailyConversionsRemaining > 0);
    public string StatusText => Type switch
    {
        LicenseType.PaidLifetime => "PRO LIFETIME (UNLIMITED)",
        LicenseType.Trial => $"FREE TRIAL ({DaysRemaining} Days Left • {DailyConversionsRemaining}/{MaxDailyTrialConversions} Left Today)",
        LicenseType.Expired => "TRIAL EXPIRED (ACTIVATION REQUIRED)",
        _ => "UNLICENSED"
    };
}

public static class LicenseManager
{
    private const string SecretSalt = "AlienSoft_BAT_TO_EXE_SECURE_2026_@_98877!";
    private const string RegKeyPath = @"Software\AlienSoftwareDevelopment\BatToExeConverter";
    private const int TrialDaysDuration = 7;

    public static string GetMachineFingerprint()
    {
        try
        {
            var sb = new StringBuilder();
            sb.Append(Environment.MachineName);
            sb.Append(Environment.UserName);
            sb.Append(Environment.ProcessorCount);
            sb.Append(Environment.SystemPageSize);

            try
            {
                var drive = new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\");
                sb.Append(drive.TotalSize);
            }
            catch { }

            using var sha = SHA256.Create();
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString() + SecretSalt));
            string hex = Convert.ToHexString(hash);
            return $"{hex[0..4]}-{hex[4..8]}-{hex[8..12]}-{hex[12..16]}";
        }
        catch
        {
            return "ALEN-HOST-DEFAULT-0001";
        }
    }

    public static LicenseStatus CheckLicense()
    {
        string machineId = GetMachineFingerprint();
        var status = new LicenseStatus { MachineId = machineId };

        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegKeyPath);
            if (key == null)
            {
                status.Type = LicenseType.Trial;
                status.DaysRemaining = TrialDaysDuration;
                status.DailyConversionsRemaining = LicenseStatus.MaxDailyTrialConversions;
                return status;
            }

            string? licenseKey = key.GetValue("LicenseKey") as string;
            string? licensee = key.GetValue("Licensee") as string;

            if (!string.IsNullOrEmpty(licenseKey) && ValidateLicenseKey(licenseKey, machineId, licensee ?? "Alien Customer"))
            {
                status.Type = LicenseType.PaidLifetime;
                status.RegisteredName = licensee ?? "Registered Customer";
                status.DaysRemaining = 99999;
                status.DailyConversionsUsed = 0;
                status.DailyConversionsRemaining = int.MaxValue;
                return status;
            }

            GetDailyTrialUsage(key, out int usedToday, out int remainingToday);
            status.DailyConversionsUsed = usedToday;
            status.DailyConversionsRemaining = remainingToday;

            string? firstRunEncrypted = key.GetValue("TrialInit") as string;
            string? lastRunEncrypted = key.GetValue("LastCheck") as string;
            DateTime now = DateTime.UtcNow;

            if (string.IsNullOrEmpty(firstRunEncrypted))
            {
                string initEnc = ProtectString(now.ToString("o"));
                key.SetValue("TrialInit", initEnc);
                key.SetValue("LastCheck", ProtectString(now.ToString("o")));
                status.Type = LicenseType.Trial;
                status.TrialStartDate = now;
                status.DaysRemaining = TrialDaysDuration;
                status.ExpiryDate = now.AddDays(TrialDaysDuration);
                return status;
            }

            string initStr = UnprotectString(firstRunEncrypted);
            if (DateTime.TryParse(initStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var firstRun))
            {
                if (!string.IsNullOrEmpty(lastRunEncrypted))
                {
                    string lastStr = UnprotectString(lastRunEncrypted);
                    if (DateTime.TryParse(lastStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var lastRun))
                    {
                        if (now < lastRun.AddMinutes(-5))
                        {
                            status.Type = LicenseType.Expired;
                            status.DaysRemaining = 0;
                            status.DailyConversionsRemaining = 0;
                            return status;
                        }
                    }
                }
                key.SetValue("LastCheck", ProtectString(now.ToString("o")));

                var elapsed = (now - firstRun).TotalDays;
                int remaining = TrialDaysDuration - (int)Math.Floor(elapsed);

                if (remaining <= 0)
                {
                    status.Type = LicenseType.Expired;
                    status.DaysRemaining = 0;
                    status.DailyConversionsRemaining = 0;
                    status.TrialStartDate = firstRun;
                    status.ExpiryDate = firstRun.AddDays(TrialDaysDuration);
                }
                else
                {
                    status.Type = LicenseType.Trial;
                    status.DaysRemaining = remaining;
                    status.TrialStartDate = firstRun;
                    status.ExpiryDate = firstRun.AddDays(TrialDaysDuration);
                }
                return status;
            }

            status.Type = LicenseType.Expired;
            status.DailyConversionsRemaining = 0;
            return status;
        }
        catch
        {
            status.Type = LicenseType.Trial;
            status.DaysRemaining = TrialDaysDuration;
            status.DailyConversionsRemaining = LicenseStatus.MaxDailyTrialConversions;
            return status;
        }
    }

    public static bool CanConvertToday(out int remainingToday, out int usedToday)
    {
        var status = CheckLicense();
        if (status.Type == LicenseType.PaidLifetime)
        {
            remainingToday = int.MaxValue;
            usedToday = 0;
            return true;
        }

        if (status.Type != LicenseType.Trial || status.DaysRemaining <= 0)
        {
            remainingToday = 0;
            usedToday = LicenseStatus.MaxDailyTrialConversions;
            return false;
        }

        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegKeyPath);
            GetDailyTrialUsage(key, out usedToday, out remainingToday);
            return remainingToday > 0;
        }
        catch
        {
            remainingToday = 1;
            usedToday = 0;
            return true;
        }
    }

    public static void RecordConversion()
    {
        var status = CheckLicense();
        if (status.Type == LicenseType.PaidLifetime) return;

        try
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegKeyPath);
            if (key != null)
            {
                string today = DateTime.Now.ToString("yyyy-MM-dd");
                string? storedEnc = key.GetValue("DailyUsage") as string;
                int count = 0;

                if (!string.IsNullOrEmpty(storedEnc))
                {
                    string stored = UnprotectString(storedEnc);
                    var parts = stored.Split(':');
                    if (parts.Length == 2 && parts[0] == today && int.TryParse(parts[1], out int c))
                    {
                        count = c;
                    }
                }

                count++;
                key.SetValue("DailyUsage", ProtectString($"{today}:{count}"));
            }
        }
        catch { }
    }

    private static void GetDailyTrialUsage(RegistryKey? key, out int usedToday, out int remainingToday)
    {
        usedToday = 0;
        remainingToday = LicenseStatus.MaxDailyTrialConversions;

        if (key == null) return;

        try
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string? storedEnc = key.GetValue("DailyUsage") as string;

            if (!string.IsNullOrEmpty(storedEnc))
            {
                string stored = UnprotectString(storedEnc);
                var parts = stored.Split(':');
                if (parts.Length == 2 && parts[0] == today && int.TryParse(parts[1], out int c))
                {
                    usedToday = c;
                }
            }

            remainingToday = Math.Max(0, LicenseStatus.MaxDailyTrialConversions - usedToday);
        }
        catch
        {
            usedToday = 0;
            remainingToday = LicenseStatus.MaxDailyTrialConversions;
        }
    }

    public static bool ActivatePaidLicense(string licenseKey, string licenseeName)
    {
        string machineId = GetMachineFingerprint();
        if (ValidateLicenseKey(licenseKey, machineId, licenseeName))
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegKeyPath);
            if (key != null)
            {
                key.SetValue("LicenseKey", licenseKey.Trim());
                key.SetValue("Licensee", licenseeName.Trim());
                return true;
            }
        }
        return false;
    }

    public static bool RemovePaidLicense()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegKeyPath, true);
            if (key != null)
            {
                key.DeleteValue("LicenseKey", false);
                key.DeleteValue("Licensee", false);
                return true;
            }
        }
        catch { }
        return false;
    }

    public static bool ResetAllLicenseData()
    {
        try
        {
            Registry.CurrentUser.DeleteSubKeyTree(RegKeyPath, false);
            return true;
        }
        catch { }
        return false;
    }

    public static string GenerateLicenseKey(string machineId, string licenseeName)
    {
        string normalizedId = machineId.Trim().ToUpperInvariant();
        string normalizedUser = licenseeName.Trim().ToUpperInvariant();
        string raw = $"{normalizedId}|{normalizedUser}|{SecretSalt}|LIFETIME_PAID";

        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(SecretSalt));
        byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(raw));
        string hex = Convert.ToHexString(hash);
        return $"ALIEN-{hex[0..5]}-{hex[5..10]}-{hex[10..15]}-{hex[15..20]}";
    }

    public static bool ValidateLicenseKey(string licenseKey, string machineId, string licenseeName)
    {
        if (string.IsNullOrWhiteSpace(licenseKey)) return false;
        string expected = GenerateLicenseKey(machineId, licenseeName);
        return string.Equals(licenseKey.Trim(), expected, StringComparison.OrdinalIgnoreCase);
    }

    private static string ProtectString(string plain)
    {
        try
        {
            byte[] plainBytes = Encoding.UTF8.GetBytes(plain);
            byte[] encrypted = ProtectedData.Protect(plainBytes, Encoding.UTF8.GetBytes(SecretSalt), DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }
        catch
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plain));
        }
    }

    private static string UnprotectString(string cipher)
    {
        try
        {
            byte[] cipherBytes = Convert.FromBase64String(cipher);
            byte[] decrypted = ProtectedData.Unprotect(cipherBytes, Encoding.UTF8.GetBytes(SecretSalt), DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(cipher));
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
