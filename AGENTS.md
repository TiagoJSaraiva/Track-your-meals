# AGENTS.md

Orientações para agentes (e pessoas) que alteram este projeto.

## Projeto

Track Your Meals é um app desktop multiplataforma (Windows, Linux e macOS) para montar um plano alimentar e registrar o consumo diário. Os dados ficam em SQLite local.

- .NET 8 (`net8.0`), Avalonia 11 (tema Fluent), MVVM com CommunityToolkit.Mvvm, EF Core 8 + SQLite.
- Textos da interface e mensagens em português (pt-BR).

## Como rodar

```sh
cd src/TrackYourMeals
dotnet run
```

Para validar alterações, use `dotnet build` (o projeto não tem testes automatizados).

## Estrutura (`src/TrackYourMeals`)

- `Program.cs`: ponto de entrada do Avalonia. `App.axaml`: tema Fluent e estilos globais (botões, campos de texto).
- `Models/`: entidades do EF Core e enums.
- `Data/`: `DbContext`, factory e inicialização do banco. Arquivos na pasta de dados locais do usuário (`Environment.SpecialFolder.LocalApplicationData`): `%LocalAppData%\TrackYourMeals\` no Windows, `~/.local/share/TrackYourMeals/` no Linux e `~/Library/Application Support/TrackYourMeals/` no macOS.
- `Migrations/`: migrations do EF Core.
- `Services/`: regras de acesso a dados (`FoodService`, `FoodLogService`, `DietDayService`) e preferências do app (`SettingsService`, que grava `settings.json`; `ThemeService`, que troca o tema).
- `ViewModels/`: `MainViewModel` e view models das linhas.
- `Themes/`: `LightTheme.axaml` e `DarkTheme.axaml`, ligados às variantes `Light`/`Dark` em `App.axaml`.
- `MainWindow.axaml`: janela principal. `HelpWindow.axaml`: guia de uso. `MessageWindow.axaml`: caixa de mensagem (o Avalonia não tem `MessageBox`).

## Convenções

- Cores sempre via `{DynamicResource ...Brush}`. Toda chave nova precisa existir em **ambos** os arquivos de `Themes/`. Não use cores fixas no XAML.
- Estilos usam seletores e classes do Avalonia (ex.: `Classes="primary"` para o botão principal). Estados do tema Fluent (hover, foco) são sobrescritos nas partes do template (`/template/ ...`), como em `App.axaml`.
- Bindings são compilados (`AvaloniaUseCompiledBindingsByDefault`): toda janela e todo `DataTemplate` com binding precisa de `x:DataType`.
- Nada específico de um sistema operacional (registro do Windows, P/Invoke etc.): o app precisa rodar em Windows, Linux e macOS. Atalhos usam o modificador da plataforma (Ctrl, ou Cmd no macOS).
- Preferências do usuário (tema, zoom etc.) ficam em `AppSettings` e são salvas pelo `SettingsService`, não no banco.
- O layout precisa funcionar com zoom entre 50% e 200%. O conteúdo da janela é escalado por um `LayoutTransformControl` (veja `MainWindow.axaml.cs`).

## Releases

- O workflow `.github/workflows/release.yml` gera os instaladores quando uma tag `v*` é enviada e cria a Release no GitHub. Executado manualmente, ele só compila e deixa os arquivos como artifacts.
- A versão vem da tag (`v1.2.3` vira `1.2.3`). Mantenha `<Version>` do csproj igual à última tag.
- Arquivos de empacotamento em `installer/`: `windows/TrackYourMeals.iss` (Inno Setup), `linux/` (`control` e `.desktop` do pacote `.deb`) e `macos/Info.plist`. O texto da Release vem de `.github/release-notes.md`.
- **Não mude os nomes dos arquivos gerados** (`TrackYourMeals-Setup-win-x64.exe`, `trackyourmeals_linux_amd64.deb`, `TrackYourMeals-macOS-AppleSilicon.dmg`, `TrackYourMeals-macOS-Intel.dmg`): links externos dependem deles.
- Não altere o `AppId` do `.iss`; ele liga as versões instaladas no Windows.
- O ícone fica em `src/TrackYourMeals/Assets/` (`icon.png` de 1024 px e `icon.ico`). O `.icns` do macOS é gerado no workflow a partir do PNG.

## Guia de uso: revisão obrigatória

O app tem um guia de uso em `src/TrackYourMeals/HelpWindow.axaml`, aberto pelo botão "Ajuda" ou pela tecla F1.

**Sempre que houver alteração de funcionalidade, interface, fluxo ou atalhos de teclado, revise o guia de uso e atualize-o na mesma alteração.** Inclua a revisão do guia ao concluir qualquer mudança, mesmo que a conclusão seja "nada a alterar".
