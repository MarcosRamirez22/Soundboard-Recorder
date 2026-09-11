using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SoundByte_Builder.Input
{
    public class GlobalKeyboardHook : IDisposable
    {
        private const int WH_KEYBOARD_LL = 13;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private readonly LowLevelKeyboardProc hookProc;
        private nint hookId = nint.Zero;

        public event Action<Keys>? KeyDown;
        public event Action<Keys>? KeyUp;

        public GlobalKeyboardHook()
        {
            hookProc = HookCallback;
            hookId = SetHook(hookProc);
        }

        private nint SetHook(LowLevelKeyboardProc proc)
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

        private nint HookCallback(
            int nCode,
            nint wParam,
            nint lParam)
        {
            if (nCode >= 0)
            {
                int virtualKeyCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)virtualKeyCode;

                if (wParam == WM_KEYDOWN ||
                    wParam == WM_SYSKEYDOWN)
                {
                    KeyDown?.Invoke(key);
                }

                if (wParam == WM_KEYUP ||
                    wParam == WM_SYSKEYUP)
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
            if (hookId != nint.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = nint.Zero;
            }

            GC.SuppressFinalize(this);
        }

        private delegate nint LowLevelKeyboardProc(
            int nCode,
            nint wParam,
            nint lParam
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern nint SetWindowsHookEx(
            int idHook,
            LowLevelKeyboardProc lpfn,
            nint hMod,
            uint dwThreadId
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(
            nint hhk
        );

        [DllImport(
            "user32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern nint CallNextHookEx(
            nint hhk,
            int nCode,
            nint wParam,
            nint lParam
        );

        [DllImport(
            "kernel32.dll",
            CharSet = CharSet.Auto,
            SetLastError = true)]
        private static extern nint GetModuleHandle(
            string? lpModuleName
        );
    }
}