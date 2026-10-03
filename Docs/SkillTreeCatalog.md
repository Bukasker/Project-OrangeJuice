# Katalog gwiezdnego drzewka umiejętności

Plik danych: `Assets/Main/Code/Scripts/Combat/SkillTreeCatalog.cs`.
To autorska propozycja perków dopasowana do obecnych statystyk gry, z układem konstelacji inspirowanym pomysłem gwiezdnego drzewka.

## Utworzenie katalogu w Unity

1. W oknie Project wybierz **Create → Orange Juice → Skill Tree Catalog**.
2. Powstały asset zawiera domyślne 24 perki. Nazwy, opisy, rangi, poziomy, koszty i pozycje gwiazd edytujesz w Inspectorze.
3. Przyszły kontroler drzewka powinien odczytywać ten asset. Sam asset nie jest komponentem do dodania na gracza.

Każda konstelacja ma własny lokalny układ: podstawa `(0, 0)`, odnogi `(-100, 100)` i `(100, 100)`, finał `(0, 230)`. `prerequisites` definiuje zarówno wymagane perki, jak i linie łączące gwiazdy. Przełączanie konstelacji, wygląd nieba i świecenie gwiazd wymagają osobnego interfejsu.

## Domyślne perki

| Konstelacja | Perki |
| --- | --- |
| Wojownik | Siła wojownika, Potężne uderzenie, Otwarta rana, Mistrz ostrza |
| Łucznik | Pewna ręka, Przebijająca strzała, Strzała unieruchomienia, Mistrz łuku |
| Mag | Magiczne skupienie, Jadowita iskra, Odnowa, Arcymag |
| Strażnik | Żelazna skóra, Osłona przed strzałami, Niewzruszony, Ostatni bastion |
| Witalność | Witalność, Puls życia, Duchowa tarcza, Drugi oddech |
| Alchemia | Sztuka leczenia, Trwała odnowa, Odporność na jad, Oczyszczenie |

Pełne efekty, wartości i wymagania są zapisane przy każdym perku w katalogu. Podstawowe premie mają zwykle trzy rangi na poziomach 2, 8 i 16. Gwiazdy końcowe wymagają poziomu 20 lub 25. Każda ranga domyślnie kosztuje jeden punkt.

## Zasady integracji

- Jeden punkt za każdy zdobyty poziom, począwszy od poziomu 2. To konfiguracja, którą przyszły system progresji musi obsłużyć; obecne pole `CharacterStats.Lvl` nie przyznaje punktów automatycznie.
- `CanUnlock(id, level, points, unlockedRanks, out reason)` sprawdza zakup kolejnej rangi. Nie kupuje perka ani nie odejmuje punktów. Słownik przechowuje liczbę kupionych rang; brak wpisu oznacza zero.
- Zakup wymaga odpowiedniego poziomu, punktów i wszystkich poprzedników. Poziom otwiera możliwość zakupu, a nie automatyczne odblokowanie.
- `statBonus` jest łączną premią z danego perka na wybranej randze. Przejście z +3 na +6 daje dodatkowe +3. Premie z różnych perków sumują się.
- `BonusStat` wskazuje istniejące pola `CharacterStats`: obrażenia broni białej, łuku, magii, trzy rodzaje obrony i maksymalne HP. Katalog sam ich nie modyfikuje.
- `unlockedAbility` wskazuje nową aktywną lub pasywną umiejętność. Jej zachowanie, odnowienie i parametry opisane w katalogu trzeba zaimplementować w systemie walki lub efektów. Nie są jeszcze działającymi zaklęciami.
- Zapisuj stabilne `id` i kupione rangi, a nie indeksy w tablicy. Pozycje i nazwy można zmieniać; istniejących identyfikatorów w zapisanej grze nie należy zmieniać bez migracji.
- XP, zapis gry, zakup, stosowanie premii i interfejs gwiazd są osobnymi etapami. Ten plik jest katalogiem i sprawdzaniem wymagań.

Sprawdzono kompilację z biblioteką Unity oraz 24 definicje: unikalne identyfikatory, sześć konstelacji, prawidłowe rangi i poprzedników, osiągalność gwiazd oraz reguły zakupu kolejnych rang.
