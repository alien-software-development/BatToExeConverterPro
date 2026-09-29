using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BatToExeConverter;

public enum VisibilityMode
{
    VisibleConsole = 0,
    Hidden = 1,
    Minimized = 2,
    Maximized = 3
}

public enum WorkingDirectoryMode
{
    CurrentDirectory = 0,
    TempDirectory = 1
}

public enum TargetArchitecture
{
    AnyCPU = 0,
    x64 = 1,
    x86 = 2
}

public enum ProcessPriorityMode
{
    Normal = 0,
    AboveNormal = 1,
    High = 2,
    RealTime = 3,
    BelowNormal = 4,
    Idle = 5
}

public class VersionInformation
{
    public string Title { get; set; } = "Batch Executable";
    public string Description { get; set; } = "Compiled Batch Application";
    public string Company { get; set; } = "Alien Software Development";
    public string Product { get; set; } = "Alien Batch Suite";
    public string Copyright { get; set; } = $"Copyright © {DateTime.Now.Year} Alien Software Development";
    public string FileVersion { get; set; } = "1.0.0.0";
    public string ProductVersion { get; set; } = "1.0.0.0";
    public string Comments { get; set; } = "Built with Alien BAT to EXE Studio Pro";
}

public class CompilerOptions
{
    public string BatFilePath { get; set; } = string.Empty;
    public string? InlineScriptContent { get; set; }
    public string OutputExePath { get; set; } = string.Empty;
    public string? IconPath { get; set; }
    public bool RequireAdministrator { get; set; } = false;
    public bool SingleInstanceOnly { get; set; } = false;
    public WorkingDirectoryMode WorkingDirectory { get; set; } = WorkingDirectoryMode.CurrentDirectory;
    public bool DeleteTempFilesOnExit { get; set; } = true;
    public bool EncryptScript { get; set; } = true;
    public string? Password { get; set; }
    public VisibilityMode Visibility { get; set; } = VisibilityMode.VisibleConsole;
    public TargetArchitecture Architecture { get; set; } = TargetArchitecture.AnyCPU;
    public ObfuscationLevel Obfuscation { get; set; } = ObfuscationLevel.None;
    public ProcessPriorityMode Priority { get; set; } = ProcessPriorityMode.Normal;
    public VersionInformation VersionInfo { get; set; } = new();
    public List<string> AdditionalFiles { get; set; } = new();

    // Advanced Pro Features
    public bool AntiAnalysis { get; set; } = false;
    public int ExecutionTimeoutSeconds { get; set; } = 0; // 0 = unlimited
    public string? FakeErrorMessage { get; set; }
    public string? DefaultCommandLineArgs { get; set; }
    public bool AutoRunOnStartup { get; set; } = false;
    public bool EnableIntegrityCheck { get; set; } = true;
    public string? CustomConsoleTitle { get; set; }
    public string? SplashMessage { get; set; }
    public bool LogErrorsToFile { get; set; } = false;
    public Dictionary<string, string> CustomEnvVars { get; set; } = new();
}

public class CompilerResult
{
    public bool Success { get; set; }
    public string OutputPath { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public List<string> Logs { get; set; } = new();
}

public static class BatCompilerEngine
{
    public static async Task<CompilerResult> CompileAsync(CompilerOptions options, Action<string>? logCallback = null)
    {
        var result = new CompilerResult();
        void Log(string msg)
        {
            result.Logs.Add(msg);
            logCallback?.Invoke(msg);
        }

        return await Task.Run(() =>
        {
            string tempDir = Path.Combine(Path.GetTempPath(), "AlienBatCompiler_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(tempDir);

            try
            {
                Log("[Alien Studio Engine] Initializing compilation pipeline v3.5 Pro Ultra...");

                string rawScript;
                if (!string.IsNullOrEmpty(options.InlineScriptContent))
                {
                    rawScript = options.InlineScriptContent;
                }
                else if (File.Exists(options.BatFilePath))
                {
                    rawScript = File.ReadAllText(options.BatFilePath, Encoding.Default);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = "Source Batch script not found: " + options.BatFilePath;
                    Log("✖ Error: " + result.ErrorMessage);
                    return result;
                }

                string? outDir = Path.GetDirectoryName(options.OutputExePath);
                if (!string.IsNullOrEmpty(outDir) && !Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }

                if (options.Obfuscation != ObfuscationLevel.None)
                {
                    Log($"🛡 Applying Batch Obfuscation Armor [{options.Obfuscation}]...");
                    rawScript = BatchObfuscator.Obfuscate(rawScript, options.Obfuscation);
                    Log("✔ Script obfuscated and armored.");
                }

                byte[] rawBytes = Encoding.Default.GetBytes(rawScript);

                // Compute SHA256 integrity hash
                string rawSha256;
                using (var sha = SHA256.Create())
                {
                    rawSha256 = Convert.ToHexString(sha.ComputeHash(rawBytes));
                }

                string payloadBase64;
                string aesKeyBase64 = "";
                string aesIvBase64 = "";

                if (options.EncryptScript)
                {
                    Log("🔒 Encrypting Batch payload with dynamic AES-256 cipher...");
                    using var aes = Aes.Create();
                    aes.KeySize = 256;
                    aes.GenerateKey();
                    aes.GenerateIV();

                    using var ms = new MemoryStream();
                    using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(rawBytes, 0, rawBytes.Length);
                        cs.FlushFinalBlock();
                    }

                    payloadBase64 = Convert.ToBase64String(ms.ToArray());
                    aesKeyBase64 = Convert.ToBase64String(aes.Key);
                    aesIvBase64 = Convert.ToBase64String(aes.IV);
                    Log("✔ AES-256 payload generated successfully.");
                }
                else
                {
                    payloadBase64 = Convert.ToBase64String(rawBytes);
                }

                string csharpCode = GenerateSourceCode(options, payloadBase64, aesKeyBase64, aesIvBase64, rawSha256);
                string sourceFile = Path.Combine(tempDir, "Program.cs");
                File.WriteAllText(sourceFile, csharpCode, Encoding.UTF8);

                string manifestFile = Path.Combine(tempDir, "app.manifest");
                File.WriteAllText(manifestFile, GenerateManifest(options.RequireAdministrator), Encoding.UTF8);

                string cscPath = FindCscCompiler();
                if (string.IsNullOrEmpty(cscPath) || !File.Exists(cscPath))
                {
                    result.Success = false;
                    result.ErrorMessage = "System C# Compiler (csc.exe) not found on this machine.";
                    Log("✖ Error: " + result.ErrorMessage);
                    return result;
                }

                Log($"🔨 Invoking native C# compiler: {cscPath}");
                var args = new StringBuilder();
                string targetType = (options.Visibility == VisibilityMode.Hidden) ? "winexe" : "exe";
                args.Append($"/target:{targetType} ");

                string plat = options.Architecture switch
                {
                    TargetArchitecture.x64 => "x64",
                    TargetArchitecture.x86 => "x86",
                    _ => "anycpu"
                };
                args.Append($"/platform:{plat} ");
                args.Append($"/optimize+ ");
                args.Append($"/win32manifest:\"{manifestFile}\" ");

                if (!string.IsNullOrEmpty(options.IconPath) && File.Exists(options.IconPath))
                {
                    args.Append($"/win32icon:\"{options.IconPath}\" ");
                }

                args.Append($"/out:\"{options.OutputExePath}\" ");
                args.Append("/r:System.dll /r:System.Core.dll /r:System.Windows.Forms.dll /r:System.Drawing.dll ");
                args.Append($"\"{sourceFile}\"");

                var psi = new ProcessStartInfo
                {
                    FileName = cscPath,
                    Arguments = args.ToString(),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var proc = Process.Start(psi);
                if (proc == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Failed to launch csc.exe compiler process.";
                    return result;
                }

                string stdOut = proc.StandardOutput.ReadToEnd();
                string stdErr = proc.StandardError.ReadToEnd();
                proc.WaitForExit();

                if (proc.ExitCode == 0 && File.Exists(options.OutputExePath))
                {
                    result.Success = true;
                    result.OutputPath = options.OutputExePath;
                    Log("✔ Native PE Binary successfully compiled & protected!");
                    Log($"📦 Output: {options.OutputExePath}");
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = string.IsNullOrWhiteSpace(stdErr) ? stdOut : stdErr;
                    Log("✖ Compilation failed with errors:");
                    Log(result.ErrorMessage);
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                Log("✖ Exception: " + ex.Message);
                return result;
            }
            finally
            {
                try
                {
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                }
                catch { }
            }
        });
    }

    private static string FindCscCompiler()
    {
        string winDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        string[] candidates = new[]
        {
            Path.Combine(winDir, @"Microsoft.NET\Framework64\v4.0.30319\csc.exe"),
            Path.Combine(winDir, @"Microsoft.NET\Framework\v4.0.30319\csc.exe"),
            @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe",
            @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe",
            @"C:\Program Files (x86)\Microsoft Visual Studio\2022\BuildTools\MSBuild\Current\Bin\Roslyn\csc.exe",
            @"C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
        };

        foreach (var p in candidates)
        {
            if (File.Exists(p)) return p;
        }
        return "csc.exe";
    }

    private static string SanitizeVersion(string? ver)
    {
        if (string.IsNullOrWhiteSpace(ver)) return "1.0.0.0";
        var match = System.Text.RegularExpressions.Regex.Match(ver, @"\b\d+(\.\d+){1,3}\b");
        if (match.Success)
        {
            var parts = match.Value.Split('.');
            int major = parts.Length > 0 && int.TryParse(parts[0], out int p0) ? p0 : 1;
            int minor = parts.Length > 1 && int.TryParse(parts[1], out int p1) ? p1 : 0;
            int build = parts.Length > 2 && int.TryParse(parts[2], out int p2) ? p2 : 0;
            int rev = parts.Length > 3 && int.TryParse(parts[3], out int p3) ? p3 : 0;
            return $"{major}.{minor}.{build}.{rev}";
        }
        return "1.0.0.0";
    }

    private static string GenerateManifest(bool requireAdmin)
    {
        string requestedLevel = requireAdmin ? "requireAdministrator" : "asInvoker";
        return $@"<?xml version=""1.0"" encoding=""utf-8""?>
<assembly manifestVersion=""1.0"" xmlns=""urn:schemas-microsoft-com:asm.v1"">
  <assemblyIdentity version=""1.0.0.0"" name=""AlienSoftware.BatchApp""/>
  <trustInfo xmlns=""urn:schemas-microsoft-com:asm.v2"">
    <security>
      <requestedPrivileges xmlns=""urn:schemas-microsoft-com:asm.v3"">
        <requestedExecutionLevel level=""{requestedLevel}"" uiAccess=""false"" />
      </requestedPrivileges>
    </security>
  </trustInfo>
  <compatibility xmlns=""urn:schemas-microsoft-com:compatibility.v1"">
    <application>
      <!-- Windows 10 & Windows 11 -->
      <supportedOS Id=""{{8e0f7a12-bfb3-4fe8-b9a5-48fd50a15a9a}}"" />
      <!-- Windows 8.1 -->
      <supportedOS Id=""{{1f676c76-80e1-4239-95bb-83d0f6d0da78}}"" />
      <!-- Windows 8 -->
      <supportedOS Id=""{{4a2f28e3-53b9-4441-ba9c-d69d4a4a6e38}}"" />
      <!-- Windows 7 -->
      <supportedOS Id=""{{35138b9a-5d96-4fbd-8e2d-a2440225f93a}}"" />
    </application>
  </compatibility>
</assembly>";
    }

    private static string GenerateSourceCode(CompilerOptions options, string payloadBase64, string aesKeyBase64, string aesIvBase64, string payloadSha256)
    {
        string pwdHash = "";
        if (!string.IsNullOrEmpty(options.Password))
        {
            using var sha = SHA256.Create();
            byte[] h = sha.ComputeHash(Encoding.UTF8.GetBytes(options.Password));
            pwdHash = BitConverter.ToString(h).Replace("-", "");
        }

        string windowStyle = options.Visibility switch
        {
            VisibilityMode.Hidden => "ProcessWindowStyle.Hidden",
            VisibilityMode.Minimized => "ProcessWindowStyle.Minimized",
            VisibilityMode.Maximized => "ProcessWindowStyle.Maximized",
            _ => "ProcessWindowStyle.Normal"
        };

        string priorityCode = options.Priority switch
        {
            ProcessPriorityMode.High => "proc.PriorityClass = ProcessPriorityClass.High;",
            ProcessPriorityMode.AboveNormal => "proc.PriorityClass = ProcessPriorityClass.AboveNormal;",
            ProcessPriorityMode.RealTime => "proc.PriorityClass = ProcessPriorityClass.RealTime;",
            ProcessPriorityMode.BelowNormal => "proc.PriorityClass = ProcessPriorityClass.BelowNormal;",
            ProcessPriorityMode.Idle => "proc.PriorityClass = ProcessPriorityClass.Idle;",
            _ => ""
        };

        var extraFilesCode = new StringBuilder();
        if (options.AdditionalFiles != null && options.AdditionalFiles.Count > 0)
        {
            extraFilesCode.AppendLine("            // Unpack auxiliary embedded assets");
            foreach (var file in options.AdditionalFiles)
            {
                if (File.Exists(file))
                {
                    string fname = Path.GetFileName(file);
                    string b64 = Convert.ToBase64String(File.ReadAllBytes(file));
                    extraFilesCode.AppendLine($@"            try {{ File.WriteAllBytes(Path.Combine(runDir, ""{EscapeString(fname)}""), Convert.FromBase64String(""{b64}"")); }} catch {{ }}");
                }
            }
        }

        string splashCode = "";
        if (!string.IsNullOrWhiteSpace(options.SplashMessage))
        {
            splashCode = $@"            try
            {{
                MessageBox.Show(""{EscapeString(options.SplashMessage)}"", ""{EscapeString(options.VersionInfo.Title)}"", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }} catch {{ }}";
        }

        string fakeErrCode = "";
        if (!string.IsNullOrWhiteSpace(options.FakeErrorMessage))
        {
            fakeErrCode = $@"            try
            {{
                MessageBox.Show(""{EscapeString(options.FakeErrorMessage)}"", ""{EscapeString(options.VersionInfo.Title)}"", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }} catch {{ }}";
        }

        string antiAnalysisCode = "";
        if (options.AntiAnalysis)
        {
            antiAnalysisCode = @"            // Anti-Analysis & Debugger Guard
            if (Debugger.IsAttached) return;
            try
            {
                string[] suspicious = new[] { ""x64dbg"", ""x32dbg"", ""ida64"", ""ida"", ""wireshark"", ""processhacker"", ""procmon"", ""pestudio"", ""fiddler"", ""cheatengine"" };
                foreach (var p in Process.GetProcesses())
                {
                    string pName = p.ProcessName.ToLowerInvariant();
                    foreach (var s in suspicious)
                    {
                        if (pName.Contains(s)) return;
                    }
                }
            }
            catch { }";
        }

        string autoStartupCode = "";
        if (options.AutoRunOnStartup)
        {
            autoStartupCode = @"            try
            {
                using (var k = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@""Software\Microsoft\Windows\CurrentVersion\Run"", true))
                {
                    if (k != null)
                    {
                        k.SetValue(Assembly.GetExecutingAssembly().GetName().Name ?? ""AlienApp"", Assembly.GetExecutingAssembly().Location);
                    }
                }
            } catch { }";
        }

        string envVarsCode = "";
        if (options.CustomEnvVars != null && options.CustomEnvVars.Count > 0)
        {
            var sbEnv = new StringBuilder();
            foreach (var kvp in options.CustomEnvVars)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Key))
                {
                    sbEnv.AppendLine($@"                psi.EnvironmentVariables[""{EscapeString(kvp.Key)}""] = ""{EscapeString(kvp.Value)}"";");
                }
            }
            envVarsCode = sbEnv.ToString();
        }

        string consoleTitleCode = "";
        if (!string.IsNullOrWhiteSpace(options.CustomConsoleTitle) && options.Visibility == VisibilityMode.VisibleConsole)
        {
            consoleTitleCode = $@"                try {{ Console.Title = ""{EscapeString(options.CustomConsoleTitle)}""; }} catch {{ }}";
        }

        string timeoutWaitCode = options.ExecutionTimeoutSeconds > 0
            ? $@"if (!proc.WaitForExit({options.ExecutionTimeoutSeconds * 1000})) {{ try {{ proc.Kill(); }} catch {{ }} }}"
            : "proc.WaitForExit();";

        string logErrorWrap = "";
        if (options.LogErrorsToFile)
        {
            logErrorWrap = @"                    if (proc.ExitCode != 0)
                    {
                        try
                        {
                            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""error_"" + DateTime.Now.ToString(""yyyyMMdd_HHmmss"") + "".log"");
                            File.WriteAllText(logPath, ""Process exited with code: "" + proc.ExitCode);
                        } catch { }
                    }";
        }

        string integrityCheckCode = options.EnableIntegrityCheck ? $@"
                using (var sha = SHA256.Create())
                {{
                    byte[] calcHash = sha.ComputeHash(scriptBytes);
                    string calcHex = BitConverter.ToString(calcHash).Replace(""-"" , """");
                    if (!string.Equals(calcHex, ""{payloadSha256}"", StringComparison.OrdinalIgnoreCase))
                    {{
                        MessageBox.Show(""Binary integrity check failed. The file may be corrupt or tampered with."", ""Security Error"", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }}
                }}" : "";

        return $@"using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Drawing;

[assembly: AssemblyTitle(""{EscapeString(options.VersionInfo.Title)}"")]
[assembly: AssemblyDescription(""{EscapeString(options.VersionInfo.Description)}"")]
[assembly: AssemblyCompany(""{EscapeString(options.VersionInfo.Company)}"")]
[assembly: AssemblyProduct(""{EscapeString(options.VersionInfo.Product)}"")]
[assembly: AssemblyCopyright(""{EscapeString(options.VersionInfo.Copyright)}"")]
[assembly: AssemblyFileVersion(""{SanitizeVersion(options.VersionInfo.FileVersion)}"")]
[assembly: AssemblyVersion(""{SanitizeVersion(options.VersionInfo.ProductVersion)}"")]
[assembly: AssemblyConfiguration(""{EscapeString(options.VersionInfo.Comments)}"")]

namespace AlienSoftware.CompiledApp
{{
    internal static class Program
    {{
        private static readonly string PayloadCipher = ""{payloadBase64}"";
        private static readonly string AesKey = ""{aesKeyBase64}"";
        private static readonly string AesIv = ""{aesIvBase64}"";
        private static readonly string PwdHash = ""{pwdHash}"";
        private static readonly bool IsEncrypted = {(options.EncryptScript ? "true" : "false")};
        private static readonly bool SingleInstance = {(options.SingleInstanceOnly ? "true" : "false")};
        private static readonly bool UseTempDir = {(options.WorkingDirectory == WorkingDirectoryMode.TempDirectory ? "true" : "false")};
        private static readonly bool CleanOnExit = {(options.DeleteTempFilesOnExit ? "true" : "false")};
        private static readonly string DefaultArgs = ""{EscapeString(options.DefaultCommandLineArgs ?? "")}"";

        [STAThread]
        private static void Main(string[] args)
        {{
{antiAnalysisCode}
{splashCode}
{fakeErrCode}
{autoStartupCode}

            Mutex mutex = null;
            if (SingleInstance)
            {{
                bool createdNew;
                mutex = new Mutex(true, ""AlienMutex_"" + Assembly.GetExecutingAssembly().GetType().GUID.ToString(), out createdNew);
                if (!createdNew)
                {{
                    return;
                }}
            }}

            if (!string.IsNullOrEmpty(PwdHash))
            {{
                if (!AuthenticateUser()) return;
            }}

            string runDir = UseTempDir ? Path.Combine(Path.GetTempPath(), ""AlienApp_"" + Guid.NewGuid().ToString(""N"")) : AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(runDir)) Directory.CreateDirectory(runDir);

{extraFilesCode}

            string scriptPath = Path.Combine(runDir, ""script_"" + Guid.NewGuid().ToString(""N"") + "".bat"");

            try
            {{
                byte[] scriptBytes;
                byte[] rawPayload = Convert.FromBase64String(PayloadCipher);

                if (IsEncrypted && !string.IsNullOrEmpty(AesKey))
                {{
                    using (var aes = Aes.Create())
                    {{
                        aes.Key = Convert.FromBase64String(AesKey);
                        aes.IV = Convert.FromBase64String(AesIv);
                        using (var ms = new MemoryStream())
                        {{
                            using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                            {{
                                cs.Write(rawPayload, 0, rawPayload.Length);
                                cs.FlushFinalBlock();
                            }}
                            scriptBytes = ms.ToArray();
                        }}
                    }}
                }}
                else
                {{
                    scriptBytes = rawPayload;
                }}

{integrityCheckCode}

                File.WriteAllBytes(scriptPath, scriptBytes);

{consoleTitleCode}

                var psi = new ProcessStartInfo
                {{
                    FileName = ""cmd.exe"",
                    WorkingDirectory = runDir,
                    WindowStyle = {windowStyle},
                    CreateNoWindow = {(options.Visibility == VisibilityMode.Hidden ? "true" : "false")},
                    UseShellExecute = false
                }};

{envVarsCode}

                string passedArgs = (args != null && args.Length > 0) ? "" "" + string.Join("" "", args) : """";
                if (!string.IsNullOrEmpty(DefaultArgs))
                {{
                    passedArgs = "" "" + DefaultArgs + passedArgs;
                }}
                string fullCommand = ""\"""" + scriptPath + ""\"""" + passedArgs;
                psi.Arguments = ""/c \"""" + fullCommand + ""\"""";

                using (var proc = Process.Start(psi))
                {{
                    if (proc != null)
                    {{
                        try {{ {priorityCode} }} catch {{ }}
                        {timeoutWaitCode}
{logErrorWrap}
                    }}
                }}
            }}
            catch {{ }}
            finally
            {{
                if (CleanOnExit)
                {{
                    try
                    {{
                        if (File.Exists(scriptPath)) File.Delete(scriptPath);
                        if (UseTempDir && Directory.Exists(runDir)) Directory.Delete(runDir, true);
                    }}
                    catch {{ }}
                }}

                if (mutex != null)
                {{
                    mutex.ReleaseMutex();
                    mutex.Dispose();
                }}
            }}
        }}

        private static bool AuthenticateUser()
        {{
            using (var form = new Form())
            {{
                form.Text = ""Authentication Required"";
                form.Size = new Size(380, 190);
                form.StartPosition = FormStartPosition.CenterScreen;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.MaximizeBox = false;
                form.MinimizeBox = false;
                form.BackColor = Color.FromArgb(15, 23, 42);
                form.ForeColor = Color.White;

                var lbl = new Label {{ Text = ""Enter Secret Password to Execute Application:"", Location = new Point(20, 20), AutoSize = true, Font = new Font(""Segoe UI"", 9.5f) }};
                var txt = new TextBox {{ Location = new Point(20, 50), Size = new Size(325, 26), UseSystemPasswordChar = true, Font = new Font(""Segoe UI"", 10f), BackColor = Color.FromArgb(13, 20, 36), ForeColor = Color.White }};
                var btn = new Button {{ Text = ""Unlock & Run"", Location = new Point(20, 90), Size = new Size(325, 34), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(16, 185, 129), ForeColor = Color.White, DialogResult = DialogResult.OK }};

                form.Controls.AddRange(new Control[] {{ lbl, txt, btn }});
                form.AcceptButton = btn;

                if (form.ShowDialog() == DialogResult.OK)
                {{
                    using (var sha = SHA256.Create())
                    {{
                        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(txt.Text));
                        string inputHash = BitConverter.ToString(hash).Replace(""-"" , """");
                        return string.Equals(inputHash, PwdHash, StringComparison.OrdinalIgnoreCase);
                    }}
                }}
                return false;
            }}
        }}
    }}
}}";
    }

    private static string EscapeString(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";
        return input.Replace("\"", "\\\"").Replace("\r", "").Replace("\n", "");
    }
}
