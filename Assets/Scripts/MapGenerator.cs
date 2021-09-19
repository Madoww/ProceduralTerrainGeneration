using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }

    [SerializeField]
    private DrawMode drawMode;

    [SerializeField] 
    private int mapWidth;
    [SerializeField] 
    private int mapHeight;
    [SerializeField] 
    private float noiseScale;
    [SerializeField]
    private MapDisplay mapDisplay;

    [SerializeField]
    private int seed;
    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private float meshHeightMultiplier;

    [SerializeField]
    private int octaves;
    [SerializeField, Range(0,1)]
    private float persistance;
    [SerializeField]
    private float lacunarity;

    public bool autoUpdate;

    [SerializeField]
    TerrainType[] regions;

    private void OnValidate()
    {
        if(mapWidth < 1)
        {
            mapWidth = 1;
        }
        if(mapHeight < 1)
        {
            mapHeight = 1;
        }
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0)
        {
            octaves = 0;
        }
    }

    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoise(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapWidth * mapHeight];

        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        if(drawMode == DrawMode.NoiseMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            mapDisplay.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
        else if(drawMode == DrawMode.Mesh)
        {
            mapDisplay.DrawMesh(MeshGenerator.GenerateTerraintMesh(noiseMap, meshHeightMultiplier), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
        }
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}