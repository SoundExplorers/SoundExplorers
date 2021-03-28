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
AppId={{FA790242-06B4-4EBF-A9E4-B9D7604218CC}
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
;PrivilegesRequiredOverridesAllowed=dialog
OutputDir=E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\Installer
OutputBaseFilename=SoundExplorers 2021.0.0
SetupIconFile=E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\Grand Piano.ico
Compression=lzma
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}";

[Files]
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion
Source: "E:\Simon\OneDrive\Documents\Visual Studio Projects\SoundExplorers\SoundExplorers\bin\X64\Release\net5.0-windows\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

