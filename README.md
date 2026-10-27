# Track Your Meals

App desktop para montar um plano alimentar e registrar o consumo do dia: metas por alimento, calorias e macronutrientes consumidos, e histórico de registros. Os dados ficam salvos localmente em SQLite.

Roda em Windows, Linux e macOS. Feito com .NET 8, Avalonia, MVVM (CommunityToolkit.Mvvm) e EF Core.

## Download

Baixe a versão mais recente em **[Releases](https://github.com/TiagoJSaraiva/Track-your-meals/releases/latest)**. A página tem os instaladores de cada sistema e as instruções de instalação.

## Rodar a partir do código

Requer o [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```sh
cd src/TrackYourMeals
dotnet run
```

## Publicar uma versão nova

1. Atualize `<Version>` em `src/TrackYourMeals/TrackYourMeals.csproj`.
2. Crie e envie a tag correspondente:

   ```sh
   git tag v1.0.1
   git push origin v1.0.1
   ```

O workflow [`release.yml`](.github/workflows/release.yml) compila os instaladores e cria a Release. Para testar o build sem publicar, rode o workflow manualmente pela aba Actions; os arquivos ficam disponíveis como artifacts.
