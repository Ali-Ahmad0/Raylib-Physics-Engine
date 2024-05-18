using GameEngine.src.physics.body;
using System.Numerics;
using Raylib_cs;
using GameEngine.src.world;
using System.Text.Json;

namespace GameEngine.src.tilemap;

public struct TileMapProps
{
    public int[,] tileMap;
    public int[,] textureMap;
    public TileSet tileSet;
    public int size;
}

public struct TileSet
{
    public Texture2D texture;
    public Rectangle rect;
    public int columns;
    public int rows;

    public TileSet(Texture2D texture, Rectangle rect, int columns, int rows)
    {
        this.texture = texture;
        this.rect = rect;
        this.columns = columns;
        this.rows = rows;
    }

    public TileSet(string path, Rectangle rect, int columns, int rows)
    {
        this.texture = Raylib.LoadTexture(path);
        this.rect = rect;
        this.columns = columns;
        this.rows = rows;
    }
}


public static class TileMap
{
    public static void GenerateTileMapTerrain(int[,] grid, int size, List<PhysicsBody2D> bodies)
    {
        // Find all the boxes in the grid
        List<Rectangle> boxes = FindBoxes(grid);

        // Create the bodies
        AddBodies(boxes, bodies, size);
    }
    public static List<Rectangle> FindBoxes(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        List<Rectangle> boxes = new List<Rectangle>();

        for (int i = 0; i < rows; i++)
        {
            int start, end;
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == 1)
                {
                    start = j;
                    while (j < cols && grid[i, j] == 1)
                    {
                        j++;
                    }
                    end = j;
                    int width = end - start;
                    boxes.Add(new Rectangle(start, i, width, 1));
                }
            }
        }
        MergeVerticalBoxes(boxes);
        return boxes;
    }


    public static void AddBodies(List<Rectangle> boxes, List<PhysicsBody2D> bodies, int size)
    {
        // Iterate through the edges and create the bodies
        foreach (Rectangle box in boxes)
        {
            Vector2 position = size * (box.Position + new Vector2(box.Width / 2.0f, box.Height / 2.0f));

            float width = box.Width * size;
            float height = box.Height * size;
            Creation.CreateStaticBody(position, 0f, Vector2.One, 0.5f, width, height, out StaticBody2D staticBody);
            bodies.Add(staticBody);
        }
    }

    private static void MergeVerticalBoxes(List<Rectangle> boxes)
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            for (int j = i + 1; j < boxes.Count; j++)
            {
                if (boxes[i].X == boxes[j].X && boxes[i].Width == boxes[j].Width)
                {
                    if (boxes[i].Y + boxes[i].Height == boxes[j].Y)
                    {
                        boxes[i] = new Rectangle(boxes[i].X, boxes[i].Y, boxes[i].Width, boxes[i].Height + boxes[j].Height);
                        boxes.RemoveAt(j);
                    }
                }
            }
        }
    }

    public static void DrawBackground(TileSet tileSet, int[,] textureMap, int size)
    {
        for (int i = 0; i < textureMap.GetLength(0); i++)
        {
            for (int j = 0; j < textureMap.GetLength(1); j++)
            {
                if (textureMap[i, j] >= 0)
                {
                    Rectangle source = new Rectangle((textureMap[i, j] % tileSet.columns * tileSet.rect.Width) + tileSet.rect.X, (textureMap[i, j] / tileSet.columns % tileSet.rows * tileSet.rect.Height) + tileSet.rect.Y, tileSet.rect.Width, tileSet.rect.Height);
                    Rectangle dest = new Rectangle(j * size, i * size, size, size);
                    Raylib.DrawTexturePro(tileSet.texture, source, dest, new Vector2(0, 0), 0, Color.White);
                }
            }
        }
    }

    public static void DrawBackground(TileMapProps tileMapProps)
    {
        DrawBackground(tileMapProps.tileSet, tileMapProps.textureMap, tileMapProps.size);
    }

    // Make a method that takes in both tilemap and background and draws the tilemap
    public static void GenerateTileMap(ref TileMapProps tileMapProps, List<PhysicsBody2D> bodies)
    {
        tileMapProps.size = (int)Math.Pow(2, tileMapProps.size + 2);
        GenerateTileMapTerrain(tileMapProps.tileMap, tileMapProps.size, bodies);
    }

    public static int[,] GetArrayFromJSON(string path)
    {
        string json = System.IO.File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        foreach (var item in obj)
        {
            if (item.Key == "layers")
            {
                // This json element is an array of objects

                var layers = (JsonElement)item.Value;
                foreach (var layer in layers.EnumerateArray())
                {
                    Console.WriteLine("Layer : " + layer);

                    // Get the array of integers in data key
                    var data = layer.GetProperty("data");
                    var width = layer.GetProperty("width");
                    var height = layer.GetProperty("height");

                    // Create a 2D array of integers
                    int[,] ints = new int[width.GetInt32(), height.GetInt32()];

                    for (int i = 0; i < width.GetInt32(); i++)
                    {
                        for (int j = 0; j < height.GetInt32(); j++)
                        {
                            ints[i, j] = data[i * height.GetInt32() + j].GetInt32();
                            if (ints[i, j] != 0)
                            {
                                ints[i, j]--;
                            }
                        }
                    }
                    return ints;
                }
            }
        }

        return null;
    }
}
