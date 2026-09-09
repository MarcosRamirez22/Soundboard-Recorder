using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Soundboard_Recorder
{
    public class GlobalKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private readonly LowLevelKeyboardProc hookProc;
        private IntPtr hookId = IntPtr.Zero;

        public event Action<Keys>? KeyDown;
        public event Action<Keys>? KeyUp;

        public GlobalKeyboardHook()
        {
            hookProc = HookCallback;
            hookId = SetHook(hookProc);
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using ProcessModule? currentModule = currentProcess.MainModule;

            return SetWindowsHookEx(
                WH_KEYBOARD_LL,
                proc,
                GetModuleHandle(currentModule?.ModuleName),
                0
            );
        }

        private IntPtr HookCallback(
            int nCode,
            IntPtr wParam,
            IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int virtualKeyCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)virtualKeyCode;

                if (wParam == (IntPtr)WM_KEYDOWN ||
                    wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    KeyDown?.Invoke(key);
                }

                if (wParam == (IntPtr)WM_KEYUP ||
                    wParam == (IntPtr)WM_SYSKEYUP)
                {
                    KeyUp?.Invoke(key);
                }
            }

            return CallNextHookEx(
                hookId,
                nCode,
                wParam,
                lParam
            );
        }

        public void Dispose()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }

            GC.SuppressFinalize(this);
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            IntPtr hMod,
            uint dwThreadId
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(
            IntPtr hhk
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam
        );

        [DllImport(
            "kernel32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern IntPtr GetModuleHandle(
            string? lpModuleName
        );
    }
}