; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "Sound Explorers Audio Archive"
#define MyAppVersion "1.0.4"
#define MyAppPublisher "Sound and Light Exploration Society"
#define MyAppURL "https://sourceforge.net/projects/sound-explorers-audio-archive/"
#define MyAppExeName "SoundExplorers.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application. Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{D20A07F3-1837-4E9E-83F9-6CA96A7B9A69}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={commonpf64}\Sound Explorers\Audio Archive
;DefaultDirName={commonpf64}\{#MyAppName}
DisableProgramGroupPage=yes
LicenseFile=..\Source\SoundExplorers\bin\X64\Release\net6.0-windows\Licence.txt
; Uncomment the following line to run in non administrative install mode (install for current user only.)
;PrivilegesRequired=lowest
OutputDir=.\
OutputBaseFilename=SoundExplorers {#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
SetupIconFile=..\Source\SoundExplorers\Kettle Drum (multi-size).ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "..\Source\SoundExplorers\bin\X64\Release\net6.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\Source\SoundExplorers\bin\X64\Release\net6.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Data\DatabaseConfig.xml"; DestDir: "{app}"; Flags: onlyifdoesntexist
Source: "Data\Initialised Database\*"; DestDir: "{app}\Initialised Database"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "Data\Documentation\*"; DestDir: "{app}\Documentation"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon