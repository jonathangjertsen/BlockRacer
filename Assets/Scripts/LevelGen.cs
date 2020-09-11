using UnityEngine;

public class LevelGen : MonoBehaviour
{
    public ColorToPrefab[] colorMap;
    public Level[] levels;
    public int playerR;
    public int playerG;
    public int playerB;
    public float scale = 2;

    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        // Select level
        // NOTE: not correct, just to prevent crashing for now
        Level level = levels[GameControl.level % levels.Length];

        // Populate target score table
        ScoreTarget.SetTarget("gold", level.goldScore);
        ScoreTarget.SetTarget("silver", level.silverScore);
        ScoreTarget.SetTarget("bronze", level.bronzeScore);

        // Populate map
        Texture2D map = level.groundMap;
        for (int x = 0; x < map.width; x++)
        {
            for (int y = 0; y < map.height; y++)
            {
                GenerateTile(map, x, y);
            }
        }
    }

    void GenerateTile(Texture2D map, int x, int y)
    {
        Color color = map.GetPixel(x, y);
        if (color.a == 0)
        {
            return;
        }

        int r = (int)(color.r * 255);
        int g = (int)(color.g * 255);
        int b = (int)(color.b * 255);

        if ((r, g, b) == (playerR, playerG, playerB))
        {
            FindObjectOfType<Player>().Move(x * scale);
        }

        foreach (ColorToPrefab c in colorMap)
        {
            if ((r, g, b) == (c.r, c.g, c.b))
            {
                Vector3 position = new Vector3(x * scale, 0, y * scale);
                Instantiate(
                    c.prefab,
                    position,
                    Quaternion.identity,
                    transform
                );
                break;
            }
        }
    }
}
