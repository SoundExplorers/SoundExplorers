; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Sound Explorers Audio Archive"
#define MyAppVersion "2021.0.0"
#define MyAppPublisher "Sound and Light Exploration Society"
#define MyAppURL "https://sourceforge.net/projects/sound-explorers-audio-archive/"
#define MyAppExeName "SoundExplorers.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D3E520A5-4C56-48AC-B30F-03E6D0166BC6}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName=C:\Program Files\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\Licence.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
PrivilegesRequiredOverridesAllowed=dialog
OutputDir=E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\Installer
OutputBaseFilename=SoundExplorers 2021.0.0
SetupIconFile=E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\Fred Flintstone.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; 

[Files]
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\JetBrains.Annotations.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\JetBrains.Profiler.Api.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\LumenWorks.Framework.IO.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\Microsoft.Data.Edm.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\Microsoft.Data.OData.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\Microsoft.Data.Services.Client.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\Newtonsoft.Json.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\SoundExplorers.Common.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\SoundExplorers.Controller.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\SoundExplorers.Data.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\SoundExplorers.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\SoundExplorers.Model.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\SoundExplorers.View.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\System.Spatial.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\VelocityDb.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\VelocityDBExtensionsCore.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\Documentation\*"; DestDir: "{app}\Documentation"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

