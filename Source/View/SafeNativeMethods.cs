using System;
using System.Runtime.InteropServices;

namespace SoundExplorers.View; 

internal static class SafeNativeMethods {
  [DllImport("user32.dll", SetLastError = true)]
  public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);
}