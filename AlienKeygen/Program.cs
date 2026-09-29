using System;
using System.Windows.Forms;
using BatToExeConverter;

namespace AlienKeygen;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.Run(new KeygenForm());
    }
}
