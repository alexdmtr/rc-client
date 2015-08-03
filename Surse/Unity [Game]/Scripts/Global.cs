using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using Mono.Data.Sqlite;

public enum GlobalStates { Intro, Loading, Map, Menu, Playing };
public class Global
{
    #region map class
    public class Map
    {
        public static Terrain Terrain;
        public static void BuildHeightMap()
        {
            Texture2D Heightmap = LoadTexture(@"Textures/Map/Heightmap");
            Color[] pixelMap = Heightmap.GetPixels();
            
            int width = Terrain.terrainData.heightmapWidth;
            int height = Terrain.terrainData.heightmapHeight;

            float[,] heightMap = Terrain.terrainData.GetHeights(0, 0, width, height);
            for (int y = 0; y < height-1; y++)
            {
                for (int x = 0; x < width-1; x++)
                {
                    Color colorData = pixelMap[(x * 8 * Heightmap.width + 8 * y)];
                    float percent = colorData.r * 1f;
                    heightMap[x, y] = percent;
                }
            }

            Terrain.terrainData.SetHeights(0, 0, heightMap);
        }

        static bool SameColor(Color x, Color y)
        {
            float delta = 1f;
            Color32 a = x;
            Color32 b = y;
            return a.r - b.r <= delta && a.g - b.g <= delta && a.b - b.b <= delta;
            //return Mathf.Approximately(a.r, b.r) && Mathf.Approximately(a.g, b.g) && Mathf.Approximately(a.b, b.b);
        }
        public static void BuildAlphaMap()
        {
            Texture2D Terrainmap = LoadTexture(@"Textures/Map/Terrain");
            Color[] pixelMap = Terrainmap.GetPixels();

            int width = Terrain.terrainData.alphamapWidth;
            int height = Terrain.terrainData.alphamapHeight;

            float[, ,] alphaMap = Terrain.terrainData.GetAlphamaps(0, 0, width, height);

            Vector4 Water = new Vector4(69, 91, 186, 255) / 255;
            Vector4 Grass = new Vector4(86, 124, 27, 255) / 255;
            Vector4 Forest = new Vector4(0, 86, 6, 255) / 255;
            Vector4 SnowlessMountains = new Vector4(65, 42, 17, 255) /255;
            Vector4 SnowyMountains = new Vector4(155, 155, 155, 255) /255;
            Vector4 MoreSnowyMountains = new Vector4(255, 255, 255, 255)/255;
            Vector4 Desert = new Vector4(206, 19, 99, 255) /255;
            Vector4 DesertGrass = new Vector4(130, 158, 75, 255)/255;
            Vector4 Farmlands = new Vector4(138, 11, 26, 255)/255;
            Vector4 Arctic = new Vector4(13, 96, 62, 255)/255;
            Vector4 Jungle = new Vector4(40, 180, 149, 255)/255;
            Vector4 DesertMountain = new Vector4(86, 46, 0, 255)/255;
            Vector4 SandyMountain = new Vector4(112, 74, 31, 255)/255;
            Vector4 Steppe = new Vector4(255, 186, 0, 255)/255;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    alphaMap[x, y, 0] = alphaMap[x, y, 1] = alphaMap[x, y, 2] = alphaMap[x, y, 3] = 0;
                    Color c = pixelMap[(x * 8 * Terrainmap.width + 8 * y)];
                    
                    if (SameColor(c, Water) || SameColor(c,Desert))
                        alphaMap[x, y, 0] = 1;
                    else if (SameColor(c, Grass) || SameColor(c, Forest) || SameColor(c,Farmlands) || SameColor(c,Jungle))
                        alphaMap[x, y, 1] = 1;
                    else if (SameColor(c,DesertGrass) || SameColor(c,Steppe))
                        alphaMap[x, y, 2] = 1;
                    else
                        alphaMap[x, y, 3] = 1;
                }

            }

            Terrain.terrainData.SetAlphamaps(0, 0, alphaMap);
        }
    }
    #endregion
    public static BoardState Board;
    public static GameObject GamePrefab;
    public static GlobalStates State;
    public static Game CurrentGame;


    public static Vector3 InterpolatePosition(Vector3 initial, Vector3 target, float part)
    {
        return Vector3.Lerp(initial, target, part);
    }

    public static Quaternion InterpolateRotation(Quaternion initial, Quaternion target, float part)
    {
        return Quaternion.Slerp(initial, target, part);
    }

	public static Dictionary<string, Texture2D> TextureDatabase = new Dictionary<string, Texture2D>();
	public static Texture2D LoadTexture(string path)
	{

		if (!TextureDatabase.ContainsKey(path))
			TextureDatabase[path] = (Texture2D)Resources.Load(path, typeof(Texture2D));//(Texture2D) UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));

		return TextureDatabase[path];
	}

    public static void SwitchToGame()
    {
        if (CurrentGame == null)
            StartGame();

        State = GlobalStates.Playing;
        //CurrentGame.gameObject.SetActive(true);
    }

    public static void SwitchToMenu()
    {
        if (CurrentGame != null)
            CurrentGame.gameObject.SetActive(false);

        State = GlobalStates.Menu;
    }

    static void StartGame()
    {
        Application.LoadLevel("DuelScene");
        //CurrentGame = (GameObject.Instantiate(GamePrefab) as GameObject).GetComponent<Game>();
        //CurrentGame.gameObject.SetActive(false);
    }
	public static void Initialize(GameObject GamePrefab)
	{
        Global.GamePrefab = GamePrefab;
		SQL.Initialize();

        SQL.ExecuteFile(@"SQL/Tables");
        SQL.ExecuteFile(@"SQL/CardTypes");
        SQL.ExecuteFile(@"SQL/Actions");
        SQL.ExecuteFile(@"SQL/Cards");
        SQL.ExecuteFile(@"SQL/Scenarios");
        SQL.ExecuteFile(@"SQL/LoadingScreens");
        SQL.ExecuteFile(@"SQL/Quotes");
        SQL.ExecuteFile(@"SQL/Classes");

        State = GlobalStates.Intro;
        //GameObject.Instantiate(GamePrefab);
	}

	public static void Update()
	{
		//if (Game != null)
			//Game.Update();
	}
	public static void Quit()
	{
		SQL.Quit();
	}
	
	
	
}