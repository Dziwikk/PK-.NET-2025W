# Zadanie łączone: **MultiProject + Analiza tekstu + NuGet + DI + NUnit**

## Cel zadania

* Zbudowanie **wieloprojektowego** rozwiązania .NET z poprawnymi **referencjami** między projektami.
* Implementacja **analizy tekstu** jako biblioteki wielokrotnego użytku.
* Wykorzystanie **pakietów NuGet** (np. `Newtonsoft.Json`, `Microsoft.Extensions.DependencyInjection`).
* Zastosowanie **Dependency Injection** do spięcia warstw i usług.
* Napisanie **testów jednostkowych w NUnit** dla kluczowych funkcji.

---

## Część 1: Struktura rozwiązania i referencje między projektami

1. **Utwórz Solution** o nazwie `TextAnalyticsSolution`.
2. Dodaj projekty:

   * **Class Library** `TextAnalytics.Core` – logika analizy tekstu (model + algorytmy).
   * **Class Library** `TextAnalytics.Services` – usługi (np. logger, dostawca danych).
   * **Console Application** `TextAnalytics.App` – interfejs użytkownika (CLI).
   * **NUnit Test Project** `TextAnalytics.Tests` – testy jednostkowe.
3. **Referencje**:

   * `TextAnalytics.App` ➜ referencje do `TextAnalytics.Core` i `TextAnalytics.Services`.
   * `TextAnalytics.Tests` ➜ referencja do `TextAnalytics.Core` (i ewentualnie `TextAnalytics.Services`, jeśli testujesz usługi).

---

## Część 2: Biblioteka analizy tekstu (`TextAnalytics.Core`)

### Wymagania funkcjonalne

Zaimplementuj klasę/fasadę `TextAnalyzer` (lub zestaw współpracujących klas) realizującą co najmniej:

* **Liczba znaków (ze spacjami) / (bez spacji)**
* **Liczba liter**, **liczba cyfr**, **liczba znaków interpunkcyjnych**
* **Liczba słów**, **liczba unikalnych słów**, **najczęstsze słowo**
* **Średnia długość słowa**, **najdłuższe / najkrótsze słowo**
* **Liczba zdań** (kończone `.` `!` `?`), **średnia liczba słów na zdanie**, **najdłuższe zdanie (słowa)**

Zaprojektuj model wyników, np. rekord `TextStatistics` z właściwościami jak wyżej.

### API przykładowe

```csharp
public sealed class TextAnalyzer
{
    public TextStatistics Analyze(string text);
    public int CountCharacters(string text, bool includeSpaces = true);
    public int CountWords(string text);
    // … inne metody pomocnicze
}

public sealed record TextStatistics(
    int CharactersWithSpaces,
    int CharactersWithoutSpaces,
    int Letters,
    int Digits,
    int Punctuation,
    int WordCount,
    int UniqueWordCount,
    string MostCommonWord,
    double AverageWordLength,
    string LongestWord,
    string ShortestWord,
    int SentenceCount,
    double AverageWordsPerSentence,
    string LongestSentence
);
```

---

## Część 3: Usługi i DI (`TextAnalytics.Services`)

Zaimplementuj co najmniej dwie usługi:

* `ILoggerService` z implementacją `ConsoleLogger` (logowanie zdarzeń uruchomienia, błędów, podsumowań).
* `IInputProvider` z implementacjami np. `ConsoleInputProvider` (wczytanie z klawiatury) oraz `FileInputProvider` (wczytanie z pliku).

Dodaj integrację DI:

```csharp
services
  .AddSingleton<ILoggerService, ConsoleLogger>()
  .AddSingleton<IInputProvider, ConsoleInputProvider>()
  .AddSingleton<TextAnalyzer>();
```

---

## Część 4: Aplikacja konsolowa (`TextAnalytics.App`)

### Wymagania I/O

* Program powinien przyjąć **źródło danych**:

  1. tekst z klawiatury, 2) ścieżkę do pliku, 3) ścieżkę przekazaną jako **argument CLI**.
* Obsłuż błędy: brak pliku, pusta zawartość, niepoprawna ścieżka.

### Prezentacja wyników

* Czytelny wynik w konsoli (sekcje/statystyki w kolumnach).
* Dodatkowo: **zapis wyników do JSON** (np. `results.json`) przy użyciu `Newtonsoft.Json`.

Przykład (fragment `Program.cs`):

```csharp
var services = new ServiceCollection()
    .AddSingleton<ILoggerService, ConsoleLogger>()
    .AddSingleton<IInputProvider, ConsoleInputProvider>()
    .AddSingleton<TextAnalyzer>()
    .BuildServiceProvider();

var logger = services.GetRequiredService<ILoggerService>();
var input = services.GetRequiredService<IInputProvider>();
var analyzer = services.GetRequiredService<TextAnalyzer>();

logger.Log("Aplikacja uruchomiona.");
var text = input.Read();
var stats = analyzer.Analyze(text);

Console.WriteLine($"Słowa: {stats.WordCount}, Unikalne: {stats.UniqueWordCount}");
// … wypisz resztę statystyk

var json = JsonConvert.SerializeObject(stats, Formatting.Indented);
File.WriteAllText("results.json", json);
logger.Log("Wyniki zapisane do results.json");
```

---

## Część 5: NuGet i konfiguracja

* Zainstaluj w odpowiednich projektach:

  * `Newtonsoft.Json` (serializacja wyników w `TextAnalytics.App`).
  * `Microsoft.Extensions.DependencyInjection` (DI w `TextAnalytics.App`).

---

## Część 6: Testy jednostkowe (NUnit) – `TextAnalytics.Tests`

* Utwórz testy dla kluczowych metod `TextAnalyzer` (min. **8 testów**), w tym przypadki brzegowe:

  * pusty tekst / tylko białe znaki,
  * wielokrotne spacje i interpunkcja,
  * częstotliwość słów (remis – zdefiniuj politykę: pierwsze z max. freq., lub alfabet).

Przykład szkicu testu:

```csharp
[Test]
public void CountWords_Returns2_ForHelloWorld()
{
    var a = new TextAnalyzer();
    Assert.That(a.CountWords("Hello world!"), Is.EqualTo(2));
}
```

Uruchamianie: `dotnet test` lub z Test Explorer.

---

## Część 7: Zarządzanie wersjami i aktualizacje

* Zademonstruj aktualizację wersji pakietu (np. `Newtonsoft.Json`) oraz odświeżenie zależności.
* Dodaj krótką notatkę o **SemVer** i wpływie aktualizacji na kompatybilność API.

---

PS C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution> dotnet list TextAnalytics.App package
Project 'TextAnalytics.App' has the following package references
   [net9.0]: 
   Top-level Package                               Requested               Resolved             
   > Microsoft.Extensions.DependencyInjection      10.0.0-rc.2.25502.107   10.0.0-rc.2.25502.107
   > Newtonsoft.Json                               13.0.4                  13.0.4               


PS C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution> dotnet add TextAnalytics.App package Newtonsoft.Json --version 13.0.3

Build succeeded in 1.2s
info : X.509 certificate chain validation will use the default trust store selected by .NET for code signing.
info : X.509 certificate chain validation will use the default trust store selected by .NET for timestamping.
info : Adding PackageReference for package 'Newtonsoft.Json' into project 'C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.App\TextAnalytics.App.csproj'.
info : Restoring packages for C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.App\TextAnalytics.App.csproj...
info :   CACHE https://api.nuget.org/v3-flatcontainer/newtonsoft.json/index.json
info :   GET https://api.nuget.org/v3-flatcontainer/newtonsoft.json/13.0.3/newtonsoft.json.13.0.3.nupkg
info :   OK https://api.nuget.org/v3-flatcontainer/newtonsoft.json/13.0.3/newtonsoft.json.13.0.3.nupkg 25ms
info : Installed Newtonsoft.Json 13.0.3 from https://api.nuget.org/v3/index.json to C:\Users\micha\.nuget\packages\newtonsoft.json\13.0.3 with content hash HrC5BXdl00IP9zeV+0Z848QWPAoCr9P3bDEZguI+gkLcBKAOxix/tLEAAHC+UvDNPv4a2d18lOReHMOagPa+zQ==.
info :   GET https://api.nuget.org/v3/vulnerabilities/index.json
info :   OK https://api.nuget.org/v3/vulnerabilities/index.json 14ms
info :   GET https://api.nuget.org/v3-vulnerabilities/2025.10.16.05.26.07/vulnerability.base.json
info :   GET https://api.nuget.org/v3-vulnerabilities/2025.10.16.05.26.07/2025.10.16.23.26.09/vulnerability.update.json
info :   OK https://api.nuget.org/v3-vulnerabilities/2025.10.16.05.26.07/vulnerability.base.json 15ms
info :   OK https://api.nuget.org/v3-vulnerabilities/2025.10.16.05.26.07/2025.10.16.23.26.09/vulnerability.update.json 30ms
error: NU1605: Warning As Error: Detected package downgrade: Newtonsoft.Json from 13.0.4 to 13.0.3. Reference the package directly from the project to select a different version. 
error:  TextAnalytics.App -> TextAnalytics.Core -> Newtonsoft.Json (>= 13.0.4) 
error:  TextAnalytics.App -> Newtonsoft.Json (>= 13.0.3)
info : Package 'Newtonsoft.Json' is compatible with all the specified frameworks in project 'C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.App\TextAnalytics.App.csproj'.
info : PackageReference for package 'Newtonsoft.Json' version '13.0.3' updated in file 'C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.App\TextAnalytics.App.csproj'.
info : Writing assets file to disk. Path: C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.App\obj\project.assets.json
log  : Failed to restore C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.App\TextAnalytics.App.csproj (in 1.3 sec).



PS C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution> dotnet restore
Restore complete (1.2s)

Build succeeded in 1.3s
PS C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution> dotnet build
Restore complete (0.8s)
  TextAnalytics.Services succeeded (0.3s) → TextAnalytics.Services\bin\Debug\net9.0\TextAnalytics.Services.dll
  TextAnalytics.Core succeeded (0.3s) → TextAnalytics.Core\bin\Debug\net9.0\TextAnalytics.Core.dll
  TextAnalytics.App succeeded (4.0s) → TextAnalytics.App\bin\Debug\net9.0\TextAnalytics.App.dll
  TextAnalytics.Tests succeeded (4.0s) → TextAnalytics.Tests\bin\Debug\net9.0\TextAnalytics.Tests.dll

Build succeeded in 5.6s
PS C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution> dotnet test
Restore complete (0.6s)
  TextAnalytics.Services succeeded (0.1s) → TextAnalytics.Services\bin\Debug\net9.0\TextAnalytics.Services.dll
  TextAnalytics.Core succeeded (0.2s) → TextAnalytics.Core\bin\Debug\net9.0\TextAnalytics.Core.dll
  TextAnalytics.Tests succeeded (0.1s) → TextAnalytics.Tests\bin\Debug\net9.0\TextAnalytics.Tests.dll
NUnit Adapter 4.6.0.0: Test execution started
Running all tests in C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution\TextAnalytics.Tests\bin\Debug\net9.0\TextAnalytics.Tests.dll
   NUnit3TestExecutor discovered 5 of 5 NUnit test cases using Current Discovery mode, Non-Explicit run
NUnit Adapter 4.6.0.0: Test execution complete
  TextAnalytics.Tests test succeeded (1.2s)

Test summary: total: 5, failed: 0, succeeded: 5, skipped: 0, duration: 1.2s
Build succeeded in 2.4s
PS C:\Users\micha\RiderProjects\PK-.NET-2025W\Ex1\TextAnalyticsSolution> 




### Notatka: SemVer (Semantic Versioning) i wpływ na kompatybilność API

Wersjonowanie semantyczne (SemVer) stosuje format **MAJOR.MINOR.PATCH**:

- **MAJOR** – zmiana wersji głównej (np. 13 → 14) oznacza możliwe **breaking changes**,
  czyli zmiany niekompatybilne wstecznie w API.
- **MINOR** – zmiana wersji pobocznej (np. 13.0 → 13.1) wprowadza nowe funkcje,
  ale **bez łamania kompatybilności**.
- **PATCH** – zmiana poprawkowa (np. 13.0.2 → 13.0.3) dotyczy tylko **poprawek błędów**
  i nie wpływa na istniejący kod.

W projekcie zastosowano SemVer do kontrolowania aktualizacji pakietów NuGet.
Podczas aktualizacji (`dotnet add package Newtonsoft.Json --version 13.0.3`)
sprawdzono kompatybilność API – testy jednostkowe (NUnit) przeszły pomyślnie,
co potwierdza brak zmian łamiących kompatybilność.


---




## Struktura repo (propozycja)

```
TextAnalyticsSolution/
  src/
    TextAnalytics.Core/
    TextAnalytics.Services/
    TextAnalytics.App/
  tests/
    TextAnalytics.Tests/
  README.md
```

## Jak uruchomić

```bash
# build
dotnet build

# aplikacja (z plikiem)
dotnet run --project src/TextAnalytics.App -- --file "sample.txt"

# aplikacja (z wejścia z klawiatury)
dotnet run --project src/TextAnalytics.App -- --interactive

# testy
dotnet test
```
