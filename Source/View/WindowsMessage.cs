using System.Diagnostics.CodeAnalysis;

namespace SoundExplorers.View; 

[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "IdentifierTypo")]
internal enum WindowsMessage {
  WM_CLOSE = 0x0010,
  WM_SETFOCUS = 0x0007,
  WM_KEYDOWN = 0x100
}