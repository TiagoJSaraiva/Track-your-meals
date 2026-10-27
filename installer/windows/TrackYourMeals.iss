; Instalador do Windows (Inno Setup 6). Gerado pelo workflow .github/workflows/release.yml:
;   ISCC.exe /DAppVersion=1.0.0 /DPublishDir=<pasta do dotnet publish> /O<pasta de saída> TrackYourMeals.iss
; O AppId identifica o app para atualizações e desinstalação: não altere.

#ifndef AppVersion
  #define AppVersion "1.0.0"
#endif

#ifndef PublishDir
  #define PublishDir "..\..\src\TrackYourMeals\bin\Release\net8.0\win-x64\publish"
#endif

#define AppName "Track Your Meals"
#define AppExeName "TrackYourMeals.exe"

[Setup]
AppId={{0A965FAE-65DD-4C35-9C2B-6B2A6D8557CC}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher=Tiago Saraiva
AppPublisherURL=https://github.com/TiagoJSaraiva/Track-your-meals
AppSupportURL=https://github.com/TiagoJSaraiva/Track-your-meals/issues
; Instalação por usuário, sem pedir administrador: {autopf} vira %LocalAppData%\Programs.
PrivilegesRequired=lowest
DefaultDirName={autopf}\TrackYourMeals
DisableProgramGroupPage=yes
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
SetupIconFile=..\..\src\TrackYourMeals\Assets\icon.ico
UninstallDisplayIcon={app}\{#AppExeName}
UninstallDisplayName={#AppName}
OutputBaseFilename=TrackYourMeals-Setup-win-x64
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern

[Languages]
Name: "brazilianportuguese"; MessagesFile: "compiler:Languages\BrazilianPortuguese.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#PublishDir}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#AppName}"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\{#AppName}"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppExeName}"; Description: "{cm:LaunchProgram,{#AppName}}"; Flags: nowait postinstall skipifsilent

; O banco e o settings.json ficam em %LocalAppData%\TrackYourMeals (fora de {app})
; e são mantidos na desinstalação.
