using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapDepth : MonoBehaviour
{
 // Lista Tilemap przypisanych przez Inspektor
    public List<Tilemap> tilemaps;

    void Start()
    {
        Dictionary<int, List<Vector2Int>> tileCoordinates = ReadTilemaps(tilemaps);

        // Wyœwietlenie wspó³rzêdnych dla ka¿dej tilemapy
        foreach (var entry in tileCoordinates)
        {
            Debug.Log($"Z-level {entry.Key}:");
            foreach (var coord in entry.Value)
            {
                Debug.Log(coord);
            }
        }
    }

    private Dictionary<int, List<Vector2Int>> ReadTilemaps(List<Tilemap> tilemaps)
    {
        Dictionary<int, List<Vector2Int>> tileCoordinates = new Dictionary<int, List<Vector2Int>>();

        for (int z = 0; z < tilemaps.Count; z++)
        {
            Tilemap tilemap = tilemaps[z];
            if (tilemap == null)
            {
                Debug.LogWarning($"Tilemap at index {z} is null.");
                continue;
            }

            Debug.Log($"Processing tilemap at Z-level {z}.");

            List<Vector2Int> coordinates = new List<Vector2Int>();

            BoundsInt bounds = tilemap.cellBounds;
            for (int x = bounds.xMin; x < bounds.xMax; x++)
            {
                for (int y = bounds.yMin; y < bounds.yMax; y++)
                {
                    Vector3Int pos = new Vector3Int(x, y, 0);
                    if (tilemap.HasTile(pos))
                    {
                        coordinates.Add(new Vector2Int(x, y));
                    }
                }
            }

            tileCoordinates[z] = coordinates;
        }

        return tileCoordinates;
    }
}
