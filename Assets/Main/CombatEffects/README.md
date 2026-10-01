# Efekty postaci

Gotowe definicje: Assets/Main/CombatEffects. Własny plik: Create > Orange Juice > Status Effect.

- Heal: +25 HP raz.
- Regeneration: +5 HP/s przez 6 s.
- Bleeding: -4 HP/s przez 5 s, czerwone opadające drobinki.
- Poison: -3 HP/s przez 8 s, zielone unoszące się drobinki.
- Slow: 50% prędkości przez 4 s, niebieskie drobinki wokół postaci.

## Test w Inspectorze

1. Dodaj Effect Applier na dowolny obiekt.
2. Przypisz postać z CharacterStats/EnemyStats/PlayerStats do Target.
3. Do listy Effects dodaj wybrane pliki .asset.
4. Apply On Start domyślnie nakłada efekty raz po wejściu w Play. Wyłącz tę opcję dla mikstur/ataków, które mają nakładać efekty dopiero na żądanie. Aby nałożyć efekty ponownie (również po zmianie listy w Play), otwórz menu komponentu i wybierz Apply Effects To Target (Play Mode).
   Aby zobaczyć leczenie, najpierw obniż HP postaci.

Alternatywnie dodaj Character Effects na postać i przypisz Starting Effects — uruchomią się raz przy starcie.
Komponent Character Effects dodaje się też automatycznie przy pierwszym wywołaniu CharacterStats.ApplyEffect.

## Użycie z ataku, mikstury lub umiejętności

```csharp
[SerializeField] private StatusEffectDefinition poison;
// Po trafieniu:
targetStats.ApplyEffect(poison);
// Usunięcie jednego efektu lub wszystkich:
targetStats.Effects.Remove(poison);
targetStats.Effects.ClearEffects();
```

EffectApplier.ApplyToTarget() można podpiąć do UnityEvent (np. przycisku).
EffectApplier.ApplyTo(CharacterStats) pozwala przekazać cel z kodu.

## Zasady

Definicja jest współdzielona, ale każda postać ma własny zegar efektu. Ponowne nałożenie tego samego pliku odświeża czas, zachowując postęp do następnego ticka. Różne pliki działają równocześnie; przy wielu slowach wygrywa najniższy Movement Multiplier. Po wygaśnięciu silniejszego wraca słabszy.

Regeneracja/obrażenia następują co sekundę; na końcu niepełnej sekundy naliczana jest proporcjonalna wartość. Czas zależy od Time.timeScale. Heal działa natychmiast. Efekty nie wskrzeszają martwych postaci. Śmierć i wyłączenie komponentu/obiektu usuwają efekty; Starting Effects nie uruchamiają się ponownie przy ponownym włączeniu.

Obrażenia efektów omijają pancerz i poziom przeciwnika. Obrażenia zwykłego ataku nadal korzystają z istniejących obliczeń. Zdrowie jest ograniczone do 0..MaxHealth.

PlayerController uwzględnia slow także na schodach. Każdy przyszły kontroler ruchu NPC powinien mnożyć prędkość przez CharacterStats.MovementSpeedMultiplier.

Show Visuals wyłącza grafikę. Visual Color zmienia kolor. Visual Prefab zastępuje domyślne drobinki własnym prefabem, a Visual Offset określa położenie. Grafika jest dołączona do postaci i usuwana przy końcu efektu (dla Heal po 0,7 s).

Pusty Target w Effect Applier oznacza postać na tym samym obiekcie. Brak celu, pusta lista, martwy lub nieaktywny cel powodują komunikat w Console. Nowe lub podmienione wpisy Starting Effects w trakcie Play są automatycznie nakładane raz. W Effect Applier odpowiada za to domyślnie włączone Apply Changes During Play (niezależne od Apply On Start). Usunięcie wpisu nie anuluje już działającego efektu; jego ponowne dodanie nakłada go ponownie. Przestawianie kolejności nie nakłada efektów ponownie. Efekt po wygaśnięciu nie odnawia się sam tylko dlatego, że nadal jest na liście. Nie dodawaj tych samych efektów jednocześnie do obu list, jeśli nie chcesz dwukrotnego nakładania.

Effect Applier usuwa ze swojej listy Effects nałożone efekty, gdy przestaną działać na odbiorcach (wygaśnięcie, zdjęcie efektu, śmierć lub wyłączenie). Heal znika zaraz po wykonaniu. Nienałożone wpisy pozostają. Przy ApplyTo na kilku postaciach wpis pozostaje, dopóki działa na którejkolwiek z nich. To lista zużywana w czasie gry; aby ponowić efekt po jego usunięciu, dodaj asset ponownie. Pliki assetów nie są usuwane.
