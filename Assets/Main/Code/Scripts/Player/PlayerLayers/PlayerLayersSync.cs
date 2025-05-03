using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLayersSync : MonoBehaviour
{

    public SpriteRenderer bodyRenderer;
    public Animator bodyAnimator;
    public SpriteRenderer layerRenderer;

    [Header("Sprite Sheets")]
    public Texture2D bodySheet;
    public Texture2D layerSheet;

    [Header("Sprite Sheet Settings")]
    public int frameWidth = 128;
    public int frameHeight = 128;
    public float pixelsPerUnit = 16f;

    [Header("Ciało (do synchronizacji)")]
    public DirectionalSprites BodyIdle = new DirectionalSprites();
    public DirectionalSprites BodyRun = new DirectionalSprites();
    public DirectionalSprites BodyAttack1 = new DirectionalSprites();
    public DirectionalSprites BodyAttack2 = new DirectionalSprites();
    public DirectionalSprites BodyAttack3 = new DirectionalSprites();

    [Header("Warstwa")]
    public DirectionalSprites LayerIdle = new DirectionalSprites();
    public DirectionalSprites LayerRun = new DirectionalSprites();
    public DirectionalSprites LayerAttack1 = new DirectionalSprites();
    public DirectionalSprites LayerAttack2 = new DirectionalSprites();
    public DirectionalSprites LayerAttack3 = new DirectionalSprites();

    private Dictionary<string, DirectionalSprites> bodyDict;
    private Dictionary<string, DirectionalSprites> layerDict;

    void Start()
    {
        LoadAllSprites();

        bodyDict = new Dictionary<string, DirectionalSprites>
        {
            { "Idle", BodyIdle },
            { "Run", BodyRun },
            { "Attack1", BodyAttack1 },
            { "Attack2", BodyAttack2 },
            { "Attack3", BodyAttack3 },
        };

        layerDict = new Dictionary<string, DirectionalSprites>
        {
            { "Idle", LayerIdle },
            { "Run", LayerRun },
            { "Attack1", LayerAttack1 },
            { "Attack2", LayerAttack2 },
            { "Attack3", LayerAttack3 },
        };
        SetInitialState();
        StartCoroutine(SyncLayerWithBody());
    }

    void LoadAllSprites()
    {
        // Idle (wiersze 0-3)
        BodyIdle.Right = SliceRow(bodySheet, 0, 10);
        BodyIdle.Left = SliceRow(bodySheet, 1, 10);
        BodyIdle.Up = SliceRow(bodySheet, 2, 10);
        BodyIdle.Down = SliceRow(bodySheet, 3, 10);

        LayerIdle.Right = SliceRow(layerSheet, 0, 10);
        LayerIdle.Left = SliceRow(layerSheet, 1, 10);
        LayerIdle.Up = SliceRow(layerSheet, 2, 10);
        LayerIdle.Down = SliceRow(layerSheet, 3, 10);

        // Run (wiersze 4-7)
        BodyRun.Right = SliceRow(bodySheet, 4, 8);
        BodyRun.Left = SliceRow(bodySheet, 5, 8);
        BodyRun.Up = SliceRow(bodySheet, 6, 8);
        BodyRun.Down = SliceRow(bodySheet, 7, 8);

        LayerRun.Right = SliceRow(layerSheet, 4, 8);
        LayerRun.Left = SliceRow(layerSheet, 5, 8);
        LayerRun.Up = SliceRow(layerSheet, 6, 8);
        LayerRun.Down = SliceRow(layerSheet, 7, 8);

        // Attack (wiersze 8-11)
        BodyAttack1.Right = SliceRow(bodySheet, 8, 3, 0);
        BodyAttack2.Right = SliceRow(bodySheet, 8, 3, 2);
        BodyAttack3.Right = SliceRow(bodySheet, 8, 5, 4);

        BodyAttack1.Left = SliceRow(bodySheet, 9, 3, 0);
        BodyAttack2.Left = SliceRow(bodySheet, 9, 3, 2);
        BodyAttack3.Left = SliceRow(bodySheet, 9, 5, 4);

        BodyAttack1.Up = SliceRow(bodySheet, 10, 3, 0);
        BodyAttack2.Up = SliceRow(bodySheet, 10, 3, 2);
        BodyAttack3.Up = SliceRow(bodySheet, 10, 5, 4);

        BodyAttack1.Down = SliceRow(bodySheet, 11, 3, 0);
        BodyAttack2.Down = SliceRow(bodySheet, 11, 3, 2);
        BodyAttack3.Down = SliceRow(bodySheet, 11, 5, 4);

        LayerAttack1.Right = SliceRow(layerSheet, 8, 3, 0);
        LayerAttack2.Right = SliceRow(layerSheet, 8, 3, 2);
        LayerAttack3.Right = SliceRow(layerSheet, 8, 5, 4);

        LayerAttack1.Left = SliceRow(layerSheet, 9, 3, 0);
        LayerAttack2.Left = SliceRow(layerSheet, 9, 3, 2);
        LayerAttack3.Left = SliceRow(layerSheet, 9, 5, 4);

        LayerAttack1.Up = SliceRow(layerSheet, 10, 3, 0);
        LayerAttack2.Up = SliceRow(layerSheet, 10, 3, 2);
        LayerAttack3.Up = SliceRow(layerSheet, 10, 5, 4);

        LayerAttack1.Down = SliceRow(layerSheet, 11, 3, 0);
        LayerAttack2.Down = SliceRow(layerSheet, 11, 3, 2);
        LayerAttack3.Down = SliceRow(layerSheet, 11, 5, 4);
    }

    Sprite[] SliceRow(Texture2D sheet, int rowIndex, int count, int startCol = 0)
    {
        Sprite[] sprites = new Sprite[count];
        for (int i = 0; i < count; i++)
        {
            int x = (startCol + i) * frameWidth;
            int y = sheet.height - (rowIndex + 1) * frameHeight;

            Rect rect = new Rect(x, y, frameWidth, frameHeight);
            Vector2 pivot = new Vector2(0.5f, 0.5f);

            sprites[i] = Sprite.Create(sheet, rect, pivot, pixelsPerUnit);
            sprites[i].name = $"Sprite_{rowIndex}_{startCol + i}";
        }
        return sprites;
    }

    IEnumerator SyncLayerWithBody()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            Sprite bodySprite = bodyRenderer.sprite;
            if (bodySprite == null) continue;

            float x = bodyAnimator.GetFloat("X");
            float y = bodyAnimator.GetFloat("Y");

            string dir = Mathf.Abs(x) > Mathf.Abs(y) ? (x > 0 ? "Right" : "Left") : (y > 0 ? "Up" : "Down");

            if(x == 0 && y == 0)
            {
                dir = "Right";
            }

            DirectionalSprites matchedBodySprites = null;
            DirectionalSprites matchedLayerSprites = null;
            int bodyFrameIndex = -1;

            foreach (var pair in bodyDict)
            {
                Sprite[] bodyFrames = GetDirectionalSprites(pair.Value, dir);
                for (int i = 0; i < bodyFrames.Length; i++)
                {
                    if (bodyFrames[i].texture == bodySprite.texture && bodyFrames[i].rect == bodySprite.rect)
                    {
                        matchedBodySprites = pair.Value;
                        matchedLayerSprites = layerDict[pair.Key];
                        bodyFrameIndex = i;
                        break;
                    }
                }

                if (matchedBodySprites != null)
                    break;
            }

            if (matchedBodySprites == null || matchedLayerSprites == null || bodyFrameIndex == -1)
            {
                Debug.LogWarning("[SYNC] Nie znaleziono odpowiadających klatek.");
                continue;
            }

            Sprite[] layerFrames = GetDirectionalSprites(matchedLayerSprites, dir);
            if (bodyFrameIndex >= 0 && bodyFrameIndex < layerFrames.Length)
            {
                layerRenderer.sprite = layerFrames[bodyFrameIndex];
            }
            else
            {
                Debug.LogWarning("[SYNC] Indeks warstwy poza zakresem!");
            }
        }
    }
    void SetInitialState()
    {
        bodyAnimator.SetFloat("X", 1f); 
        bodyAnimator.SetFloat("Y", 0f);
        bodyAnimator.Play("Idle", 0, 0f);
    }

    string GetAnimationName(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("Idle")) return "Idle";
        if (stateInfo.IsName("Run")) return "Run";
        if (stateInfo.IsName("Attack1")) return "Attack1";
        if (stateInfo.IsName("Attack2")) return "Attack2";
        if (stateInfo.IsName("Attack3")) return "Attack3";
        return null;
    }

    Sprite[] GetDirectionalSprites(DirectionalSprites sprites, string direction)
    {
        return direction switch
        {
            "Up" => sprites.Up,
            "Down" => sprites.Down,
            "Left" => sprites.Left,
            "Right" => sprites.Right,
            _ => sprites.Down,
        };
    }
}
