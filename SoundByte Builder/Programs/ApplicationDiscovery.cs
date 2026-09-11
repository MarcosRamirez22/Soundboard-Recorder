using SoundByte_Builder.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace SoundByte_Builder.Programs
{
    public static class ApplicationDiscovery
    {
        private delegate bool EnumWindowsProc(
            IntPtr hWnd,
            IntPtr lParam
        );

        private const int GWL_EXSTYLE = -20;
        private const long WS_EX_TOOLWINDOW = 0x00000080L;

        private const int DWMWA_CLOAKED = 14;

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(
            EnumWindowsProc lpEnumFunc,
            IntPtr lParam
        );

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(
            IntPtr hWnd
        );

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(
            IntPtr hWnd,
            uint uCmd
        );

        [DllImport("user32.dll")]
        private static extern long GetWindowLongPtr(
            IntPtr hWnd,
            int nIndex
        );

        [DllImport("user32.dll")]
        private static extern int GetWindowTextLength(
            IntPtr hWnd
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Unicode
        )]
        private static extern int GetWindowText(
            IntPtr hWnd,
            StringBuilder lpString,
            int nMaxCount
        );

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(
            IntPtr hWnd,
            out uint lpdwProcessId
        );

        [DllImport("dwmapi.dll")]
        private static extern int DwmGetWindowAttribute(
            IntPtr hwnd,
            int dwAttribute,
            out int pvAttribute,
            int cbAttribute
        );

        public static List<RunningApplication> ResolveSavedApplications(
            IEnumerable<SavedApplication> savedApplications)
        {
            var running =
                GetRunningApplications();

            var resolved =
                new List<RunningApplication>();

            foreach (SavedApplication saved in savedApplications)
            {
                RunningApplication? match =
                    running.FirstOrDefault(app =>
                        ApplicationMatches(saved, app)
                    );

                if (match != null)
                {
                    resolved.Add(match);
                }
            }

            return resolved;
        }

        private static bool ApplicationMatches(
            SavedApplication saved,
            RunningApplication running)
        {
            if (!string.IsNullOrWhiteSpace(saved.ExecutablePath) &&
                !string.IsNullOrWhiteSpace(running.ExecutablePath))
            {
                return string.Equals(
                    saved.ExecutablePath,
                    running.ExecutablePath,
                    StringComparison.OrdinalIgnoreCase
                );
            }

            return string.Equals(
                saved.ProcessName,
                running.ProcessName,
                StringComparison.OrdinalIgnoreCase
            );
        }

        public static List<RunningApplication>
            GetRunningApplications()
        {
            var applications =
                new Dictionary<int, RunningApplication>();

            EnumWindows(
                (hWnd, lParam) =>
                {
                    if (!ShouldShowWindow(hWnd))
                    {
                        return true;
                    }

                    GetWindowThreadProcessId(
                        hWnd,
                        out uint processId
                    );

                    if (processId == 0 ||
                        applications.ContainsKey(
                            (int)processId
                        ))
                    {
                        return true;
                    }

                    try
                    {
                        using Process process =
                            Process.GetProcessById(
                                (int)processId
                            );

                        string windowTitle =
                            GetWindowTitle(hWnd);

                        string? executablePath =
                            GetExecutablePath(
                                process
                            );

                        string displayName =
                            GetFriendlyName(
                                process,
                                executablePath,
                                windowTitle
                            );

                        applications.Add(
                            (int)processId,
                            new RunningApplication
                            {
                                ProcessId =
                                    (int)processId,

                                ProcessName =
                                    process.ProcessName,

                                DisplayName =
                                    displayName,

                                WindowTitle =
                                    windowTitle,

                                ExecutablePath =
                                    executablePath
                            }
                        );
                    }
                    catch
                    {
                    }

                    return true;
                },
                IntPtr.Zero
            );

            return applications.Values
                .OrderBy(
                    application =>
                        application.DisplayName
                )
                .ToList();
        }

        private static bool ShouldShowWindow(
            IntPtr hWnd)
        {
            if (!IsWindowVisible(hWnd))
            {
                return false;
            }

            const uint GW_OWNER = 4;

            if (GetWindow(
                    hWnd,
                    GW_OWNER
                ) != IntPtr.Zero)
            {
                return false;
            }

            long extendedStyle =
                GetWindowLongPtr(
                    hWnd,
                    GWL_EXSTYLE
                );

            if ((extendedStyle &
                 WS_EX_TOOLWINDOW) != 0)
            {
                return false;
            }

            if (DwmGetWindowAttribute(
                    hWnd,
                    DWMWA_CLOAKED,
                    out int cloaked,
                    sizeof(int)
                ) == 0 &&
                cloaked != 0)
            {
                return false;
            }

            return true;
        }

        private static string GetWindowTitle(
            IntPtr hWnd)
        {
            int length =
                GetWindowTextLength(hWnd);

            if (length <= 0)
            {
                return "";
            }

            var builder =
                new StringBuilder(
                    length + 1
                );

            GetWindowText(
                hWnd,
                builder,
                builder.Capacity
            );

            return builder.ToString();
        }

        private static string? GetExecutablePath(
            Process process)
        {
            try
            {
                return process.MainModule?.FileName;
            }
            catch
            {
                return null;
            }
        }

        private static string GetFriendlyName(
            Process process,
            string? executablePath,
            string windowTitle)
        {
            if (!string.IsNullOrWhiteSpace(
                    executablePath))
            {
                try
                {
                    FileVersionInfo info =
                        FileVersionInfo.GetVersionInfo(
                            executablePath
                        );

                    if (!string.IsNullOrWhiteSpace(
                            info.FileDescription))
                    {
                        return info.FileDescription;
                    }

                    if (!string.IsNullOrWhiteSpace(
                            info.ProductName))
                    {
                        return info.ProductName;
                    }
                }
                catch
                {
                }
            }

            if (!string.IsNullOrWhiteSpace(
                    windowTitle))
            {
                return windowTitle;
            }

            return process.ProcessName;
        }
    }
}