using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Katalog danych drzewka. Nie przyznaje XP, nie wydaje punktow ani nie stosuje bonusow.
/// Efekty rang sa LACZNE: awans rangi z +3 do +6 oznacza dodatkowe +3, nie +6.
/// Identyfikatory perkow pozostaja stale, aby mozna bylo zapisac je w save gry.
/// </summary>
[CreateAssetMenu(fileName = "SkillTreeCatalog", menuName = "Orange Juice/Skill Tree Catalog")]
public sealed class SkillTreeCatalog : ScriptableObject
{
    public enum Constellation { Warrior, Archer, Mage, Guardian, Vitality, Alchemy }

    // Pierwsze siedem pozycji odpowiada polom istniejacego CharacterStats.
    public enum BonusStat { None, AttackDamage, ArrowDamage, MagicDamage, ArmorMelee, ArmorRange, MagicResist, MaxHealth }

    // To identyfikatory do przyszlego systemu umiejetnosci, a nie gotowe zachowania.
    public enum Ability
    {
        None, PowerStrike, BleedingStrike, PiercingArrow, SlowingArrow,
        PoisonSpell, RegenerationSpell, Guard, LastStand, HealingPulse,
        SecondWind, StrongerHealing, LongerRegeneration, PoisonResistance, Detox
    }

    [Serializable]
    public sealed class Requirement
    {
        public string perkId;
        [Min(1)] public int rank = 1;
    }

    [Serializable]
    public sealed class Rank
    {
        [Min(1)] public int characterLevel = 2;
        [Min(1)] public int pointCost = 1;
        [Tooltip("Laczna premia na tej randze. Nie dodawaj ponownie wartosci poprzednich rang.")]
        public int statBonus;
        [TextArea(2, 4)] public string description;
    }

    [Serializable]
    public sealed class Perk
    {
        public string id;
        public string displayName;
        public Constellation constellation;
        [TextArea(2, 5)] public string description;
        [Tooltip("Pozycja gwiazdy w lokalnym ukladzie konstelacji; jednostki UI.")]
        public Vector2 starPosition;
        public BonusStat bonusStat;
        public Ability unlockedAbility;
        [Tooltip("Wszystkie wymagania musza byc spelnione. Kazde jest tez polaczeniem gwiazd.")]
        public Requirement[] prerequisites = new Requirement[0];
        public Rank[] ranks = new Rank[0];
    }

    [Header("Zasady progresji (do wykorzystania przez system gracza)")]
    [Min(1)] public int pointsPerLevel = 1;
    [Min(1)] public int firstPointLevel = 2;
    [Tooltip("Katalog zawiera autorska propozycje 24 perkow. Mozesz zmieniac dane w Inspectorze.")]
    public Perk[] perks = CreateDefaultPerks();

    public Perk FindPerk(string id)
    {
        if (perks == null) return null;
        foreach (Perk perk in perks)
            if (perk != null && perk.id == id) return perk;
        return null;
    }

    /// <summary>Sprawdza zakup kolejnej rangi. Nie zmienia stanu ani nie wydaje punktow.</summary>
    public bool CanUnlock(string id, int characterLevel, int availablePoints,
        IDictionary<string, int> unlockedRanks, out string reason)
    {
        Perk perk = FindPerk(id);
        if (perk == null) { reason = "Nieznany perk."; return false; }
        int currentRank = GetRank(unlockedRanks, id);
        if (perk.ranks == null || currentRank < 0 || currentRank >= perk.ranks.Length)
        { reason = "Brak kolejnej rangi."; return false; }
        Rank next = perk.ranks[currentRank];
        if (next == null) { reason = "Niepoprawne dane rangi."; return false; }
        if (characterLevel < next.characterLevel)
        { reason = "Wymagany poziom: " + next.characterLevel; return false; }
        if (availablePoints < next.pointCost)
        { reason = "Za mało punktów umiejętności."; return false; }
        if (perk.prerequisites != null)
            foreach (Requirement requirement in perk.prerequisites)
            {
                if (requirement == null || FindPerk(requirement.perkId) == null)
                { reason = "Niepoprawne wymaganie perka."; return false; }
                if (GetRank(unlockedRanks, requirement.perkId) < requirement.rank)
                { reason = "Wymagany perk: " + FindPerk(requirement.perkId).displayName + ", ranga " + requirement.rank; return false; }
            }
        reason = string.Empty;
        return true;
    }

    private static int GetRank(IDictionary<string, int> ranks, string id)
    {
        return ranks != null && ranks.TryGetValue(id, out int rank) ? rank : 0;
    }

    [ContextMenu("Przywroc domyslne 24 perki (nadpisuje katalog)")]
    private void ResetToDefaults() { perks = CreateDefaultPerks(); }

    // Kazda konstelacja: podstawa (3 rangi), dwie odnogi i gwiazda koncowa.
    private static Perk[] CreateDefaultPerks()
    {
        return new[]
        {
            StatPerk("warrior.strength", "Siła wojownika", Constellation.Warrior, BonusStat.AttackDamage,
                "Zwiększa obrażenia broni białej.", 0, 0, new[] { 2, 8, 16 }, new[] { 3, 6, 10 }),
            SkillPerk("warrior.power_strike", "Potężne uderzenie", Constellation.Warrior, Ability.PowerStrike,
                "Odblokowuje aktywny mocny cios: 150% obrażeń broni białej, odnowienie 8 s.", -100, 100, 5, "warrior.strength"),
            SkillPerk("warrior.bleeding", "Otwarta rana", Constellation.Warrior, Ability.BleedingStrike,
                "Potężne uderzenie nakłada krwawienie: 2 HP/s przez 4 s.", 100, 100, 10, "warrior.power_strike"),
            StatPerk("warrior.master", "Mistrz ostrza", Constellation.Warrior, BonusStat.AttackDamage,
                "Dodatkowa premia do obrażeń broni białej.", 0, 230, new[] { 20 }, new[] { 12 }, "warrior.bleeding"),

            StatPerk("archer.training", "Pewna ręka", Constellation.Archer, BonusStat.ArrowDamage,
                "Zwiększa obrażenia łuku.", 0, 0, new[] { 2, 8, 16 }, new[] { 3, 6, 10 }),
            SkillPerk("archer.piercing", "Przebijająca strzała", Constellation.Archer, Ability.PiercingArrow,
                "Odblokowuje strzał ignorujący 30% pancerza dystansowego celu, odnowienie 8 s.", -100, 100, 5, "archer.training"),
            SkillPerk("archer.slow", "Strzała unieruchomienia", Constellation.Archer, Ability.SlowingArrow,
                "Odblokowuje strzał spowalniający cel o 35% przez 3 s, odnowienie 10 s.", 100, 100, 10, "archer.training", 2),
            StatPerk("archer.master", "Mistrz łuku", Constellation.Archer, BonusStat.ArrowDamage,
                "Dodatkowa premia do obrażeń łuku.", 0, 230, new[] { 20 }, new[] { 12 }, "archer.slow", "archer.piercing"),

            StatPerk("mage.focus", "Magiczne skupienie", Constellation.Mage, BonusStat.MagicDamage,
                "Zwiększa obrażenia magiczne.", 0, 0, new[] { 2, 8, 16 }, new[] { 3, 6, 10 }),
            SkillPerk("mage.poison", "Jadowita iskra", Constellation.Mage, Ability.PoisonSpell,
                "Odblokowuje zaklęcie trucizny: 3 HP/s przez 5 s, odnowienie 10 s.", -100, 100, 5, "mage.focus"),
            SkillPerk("mage.regeneration", "Odnowa", Constellation.Mage, Ability.RegenerationSpell,
                "Odblokowuje zaklęcie regeneracji: 4 HP/s przez 5 s, odnowienie 20 s.", 100, 100, 10, "mage.focus", 2),
            StatPerk("mage.master", "Arcymag", Constellation.Mage, BonusStat.MagicDamage,
                "Dodatkowa premia do obrażeń magicznych.", 0, 230, new[] { 20 }, new[] { 12 }, "mage.poison", "mage.regeneration"),

            StatPerk("guardian.armor", "Żelazna skóra", Constellation.Guardian, BonusStat.ArmorMelee,
                "Zwiększa pancerz przeciw obrażeniom w zwarciu.", 0, 0, new[] { 2, 8, 16 }, new[] { 2, 4, 7 }),
            StatPerk("guardian.arrows", "Osłona przed strzałami", Constellation.Guardian, BonusStat.ArmorRange,
                "Zwiększa pancerz przeciw obrażeniom dystansowym.", -100, 100, new[] { 5, 12 }, new[] { 3, 6 }, "guardian.armor"),
            SkillPerk("guardian.guard", "Niewzruszony", Constellation.Guardian, Ability.Guard,
                "Odblokowuje gardę: redukcja otrzymywanych obrażeń o 30% przez 4 s, odnowienie 15 s.", 100, 100, 10, "guardian.armor", 2),
            SkillPerk("guardian.last_stand", "Ostatni bastion", Constellation.Guardian, Ability.LastStand,
                "Poniżej 25% HP otrzymywane obrażenia są mniejsze o 15%. Efekt pasywny.", 0, 230, 20, "guardian.guard"),

            StatPerk("vitality.health", "Witalność", Constellation.Vitality, BonusStat.MaxHealth,
                "Zwiększa maksymalne zdrowie. Zakup nie leczy automatycznie.", 0, 0, new[] { 2, 8, 16 }, new[] { 10, 20, 35 }),
            SkillPerk("vitality.heal", "Puls życia", Constellation.Vitality, Ability.HealingPulse,
                "Odblokowuje natychmiastowe leczenie 20 HP, odnowienie 20 s.", -100, 100, 5, "vitality.health"),
            StatPerk("vitality.resist", "Duchowa tarcza", Constellation.Vitality, BonusStat.MagicResist,
                "Zwiększa odporność na obrażenia magiczne.", 100, 100, new[] { 10, 18 }, new[] { 3, 7 }, "vitality.health"),
            SkillPerk("vitality.second_wind", "Drugi oddech", Constellation.Vitality, Ability.SecondWind,
                "Przy spadku poniżej 20% HP regeneruje 5 HP/s przez 4 s. Odnowienie 60 s; nie wskrzesza.", 0, 230, 25, "vitality.heal", "vitality.resist"),

            SkillPerk("alchemy.healing", "Sztuka leczenia", Constellation.Alchemy, Ability.StrongerHealing,
                "Mikstury przywracające HP leczą o 20% więcej. Efekt pasywny.", 0, 0, 2),
            SkillPerk("alchemy.regeneration", "Trwała odnowa", Constellation.Alchemy, Ability.LongerRegeneration,
                "Regeneracja z mikstur trwa o 30% dłużej. Efekt pasywny.", -100, 100, 5, "alchemy.healing"),
            SkillPerk("alchemy.resistance", "Odporność na jad", Constellation.Alchemy, Ability.PoisonResistance,
                "Trucizna zadaje postaci o 30% mniej obrażeń. Efekt pasywny.", 100, 100, 10, "alchemy.healing"),
            SkillPerk("alchemy.detox", "Oczyszczenie", Constellation.Alchemy, Ability.Detox,
                "Odblokowuje usunięcie wszystkich aktywnych efektów trucizny z postaci, odnowienie 30 s.", 0, 230, 20, "alchemy.regeneration", "alchemy.resistance")
        };
    }

    private static Perk StatPerk(string id, string name, Constellation tree, BonusStat stat,
        string description, float x, float y, int[] levels, int[] bonuses, string parent = null, string secondParent = null)
    {
        var ranks = new Rank[levels.Length];
        for (int i = 0; i < ranks.Length; i++)
            ranks[i] = new Rank { characterLevel = levels[i], statBonus = bonuses[i], description = "+" + bonuses[i] + " " + stat + " łącznie." };
        return new Perk { id = id, displayName = name, constellation = tree, description = description,
            starPosition = new Vector2(x, y), bonusStat = stat, ranks = ranks, prerequisites = Parents(parent, 1, secondParent) };
    }

    private static Perk SkillPerk(string id, string name, Constellation tree, Ability ability,
        string description, float x, float y, int level, string parent = null, int parentRank = 1)
    {
        return new Perk { id = id, displayName = name, constellation = tree, description = description,
            starPosition = new Vector2(x, y), unlockedAbility = ability, prerequisites = Parents(parent, parentRank),
            ranks = new[] { new Rank { characterLevel = level, description = description } } };
    }

    private static Perk SkillPerk(string id, string name, Constellation tree, Ability ability,
        string description, float x, float y, int level, string parent, string secondParent)
    {
        Perk perk = SkillPerk(id, name, tree, ability, description, x, y, level, parent);
        perk.prerequisites = Parents(parent, 1, secondParent);
        return perk;
    }

    private static Requirement[] Parents(string parent, int rank, string secondParent = null)
    {
        if (parent == null) return new Requirement[0];
        if (secondParent == null) return new[] { new Requirement { perkId = parent, rank = rank } };
        return new[] { new Requirement { perkId = parent, rank = rank }, new Requirement { perkId = secondParent, rank = 1 } };
    }
}
