using UnityEngine.Tilemaps;

[System.Serializable]
public class SoilTileMapping
{
    public TileBase tile;
    public SoilType soilType;
}
public enum SoilType
{
    Blocked,
    Grass,
    Soil,
    DiggedSoil,
    WateredSoil,
    Sand,
    Water
}