# AGENTS.md

Orientações para agentes (e pessoas) que alteram este projeto.

## Projeto

Track Your Meals é um app desktop WPF para montar um plano alimentar e registrar o consumo diário. Os dados ficam em SQLite local.

- .NET 8 (`net8.0-windows`), WPF, MVVM com CommunityToolkit.Mvvm, EF Core 8 + SQLite.
- Textos da interface e mensagens em português (pt-BR).

## Como rodar

```sh
cd src/TrackYourMeals
dotnet run
```

Para validar alterações, use `dotnet build` (o projeto não tem testes automatizados).

## Estrutura (`src/TrackYourMeals`)

- `Models/`: entidades do EF Core e enums.
- `Data/`: `DbContext`, factory e inicialização do banco. Arquivos em `%LocalAppData%\TrackYourMeals\`.
- `Migrations/`: migrations do EF Core.
- `Services/`: regras de acesso a dados (`FoodService`, `FoodLogService`, `DietDayService`) e preferências do app (`SettingsService`, que grava `settings.json`; `ThemeService`, que troca o tema).
- `ViewModels/`: `MainViewModel` e view models das linhas.
- `Themes/`: `LightTheme.xaml` e `DarkTheme.xaml`.
- `MainWindow.xaml`: janela principal. `HelpWindow.xaml`: guia de uso.

## Convenções

- Cores sempre via `{DynamicResource ...Brush}`. Toda chave nova precisa existir em **ambos** os arquivos de `Themes/`. Não use cores fixas no XAML.
- Preferências do usuário (tema, zoom etc.) ficam em `AppSettings` e são salvas pelo `SettingsService`, não no banco.
- O layout precisa funcionar com zoom entre 50% e 200%. O conteúdo da janela é escalado por um `LayoutTransform` (veja `MainWindow.xaml.cs`).

## Guia de uso: revisão obrigatória

O app tem um guia de uso em `src/TrackYourMeals/HelpWindow.xaml`, aberto pelo botão "Ajuda" ou pela tecla F1.

**Sempre que houver alteração de funcionalidade, interface, fluxo ou atalhos de teclado, revise o guia de uso e atualize-o na mesma alteração.** Inclua a revisão do guia ao concluir qualquer mudança, mesmo que a conclusão seja "nada a alterar".
