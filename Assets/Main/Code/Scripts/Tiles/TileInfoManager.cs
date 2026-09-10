using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileInfoManager : MonoBehaviour
{
    public Tilemap infoTilemap;

    public List<SoilTileMapping> tileMappings;

    private Dictionary<Vector3Int, SoilType> soilData = new Dictionary<Vector3Int, SoilType>();
    private Dictionary<TileBase, SoilType> tileToSoilTypeMap = new Dictionary<TileBase, SoilType>();

    void Awake()
    {
        InitializeTileTypeMap();
        CacheSoilData();
    }

    private void InitializeTileTypeMap()
    {
        foreach (var mapping in tileMappings)
        {
            if (!tileToSoilTypeMap.ContainsKey(mapping.tile))
            {
                tileToSoilTypeMap.Add(mapping.tile, mapping.soilType);
            }
        }
    }

    private void CacheSoilData()
    {
        soilData.Clear();
        foreach (var pos in infoTilemap.cellBounds.allPositionsWithin)
        {
            if (!infoTilemap.HasTile(pos)) continue;

            TileBase tile = infoTilemap.GetTile(pos);
            if (tileToSoilTypeMap.TryGetValue(tile, out SoilType soilType))
            {
                soilData[pos] = soilType;
            }
        }
    }

    public SoilType GetSoilType(Vector3Int cellPosition)
    {
        if (soilData.TryGetValue(cellPosition, out SoilType soilType))
        {
            return soilType;
        }
        return SoilType.Blocked; // lub dowolny domyœlny typ
    }

    public void SetSoilType(Vector3Int cellPosition, SoilType newSoilType)
    {
        soilData[cellPosition] = newSoilType;
        // Mo¿esz tutaj dodaæ kod, który zmienia tile na odpowiedni jeœli chcesz.
    }

    public bool IsSoilType(Vector3Int cellPosition, SoilType typeToCheck)
    {
        return GetSoilType(cellPosition) == typeToCheck;
    }
}
