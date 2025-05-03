using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EditorStarSpawner : MonoBehaviour
{
    public GameObject[] starPrefabs;
    public int numberOfStars = 200;
    public Vector2 areaSize = new Vector2(50, 30);
    public bool clearOldStars = true;

    [ContextMenu("Generate Stars In Editor")]
    public void GenerateStars()
    {
        if (clearOldStars)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        float minDistance = 0.5f; // minimalna odleg³oœæ miêdzy gwiazdami
        List<Vector2> placedPositions = new List<Vector2>();

        int attempts = 0;
        int maxAttempts = numberOfStars * 10;

        int starsPlaced = 0;

        while (starsPlaced < numberOfStars && attempts < maxAttempts)
        {
            Vector2 position = new Vector2(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                Random.Range(-areaSize.y / 2, areaSize.y / 2)
            );

            bool tooClose = false;
            foreach (var pos in placedPositions)
            {
                if (Vector2.Distance(pos, position) < minDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
            {
                GameObject prefab = starPrefabs[Random.Range(0, starPrefabs.Length)];
                GameObject star = Instantiate(prefab);

                star.transform.SetParent(transform);
                star.transform.localPosition = position;

                placedPositions.Add(position);
                starsPlaced++;
            }

            attempts++;
        }

        if (starsPlaced < numberOfStars)
        {
            Debug.LogWarning($"Tylko {starsPlaced} z {numberOfStars} gwiazd zosta³o wygenerowanych — zwiêksz areaSize lub zmniejsz minDistance.");
        }
    }

}
