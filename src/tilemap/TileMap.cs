using GameEngine.src.physics.body;
using System.Numerics;
using Raylib_cs;
using GameEngine.src.world;
using System.Text.Json;
using GameEngine.src.physics.collision.shape;

namespace GameEngine.src.tilemap;

#pragma warning disable CS8603 // Possible null reference return.

public struct TileMapProps
{
    public int[,] collisionMap; // 2D array representing the collision map
    public int[,] textureMap; // 2D array representing the texture map
    
    public Texture2D texture; // The texture of the tilemap

    public TileSet tileSet; // TileSet object containing information about the tileset
    public int size; // Size of the tiles
}

public class TileSet
{
    public Texture2D texture; // Texture of the tileset
    public Rectangle rect; // Rectangle representing the area of a single tile in the tileset
    public int columns; // Number of columns in the tileset
    public int rows; // Number of rows in the tileset

    public TileSet(Texture2D texture, Rectangle rect, int columns, int rows)
    {
        // Make a new tileset with the given parameters
        this.texture = texture;
        this.rect = rect;
        this.columns = columns;
        this.rows = rows;
    }

    public TileSet(string path, Rectangle rect, int columns, int rows)
    {
        // Make a new tileset with a given path
        texture = Raylib.LoadTexture(path);
        this.rect = rect;
        this.columns = columns;
        this.rows = rows;
    }
}


public class TileMap : World
{
    public static void GenerateTileMapTerrain(int[,] grid, int size, List<PhysicsBody2D> bodies)
    {
        // Find all the boxes in the grid
        List<Rectangle> boxes = FindBoxes(grid);

        // Create the bodies
        AddBodies(boxes, bodies, size);
    }

    // Find all the boxes in the grid
    public static List<Rectangle> FindBoxes(int[,] grid)
    {
        // Get the dimensions of the grid
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // Create a list to store the boxes
        List<Rectangle> boxes = new List<Rectangle>();

        for (int i = 0; i < rows; i++)
        {
            // Iterate through the rows and find the horizontal boxes
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
        // Merge vertically adjacent boxes
        MergeVerticalBoxes(boxes);
        return boxes;
    }

    // Merge vertically adjacent boxes
    private static void MergeVerticalBoxes(List<Rectangle> boxes)
    {
        for (int i = 0; i < boxes.Count; i++)
        {
            // Loop through the boxes and merge the vertically adjacent ones
            for (int j = i + 1; j < boxes.Count; j++)
            {
                if (boxes[i].X == boxes[j].X && boxes[i].Width == boxes[j].Width)
                {
                    if (boxes[i].Y + boxes[i].Height == boxes[j].Y)
                    {
                        // Merge the boxes and remove the second one
                        boxes[i] = new Rectangle(boxes[i].X, boxes[i].Y, boxes[i].Width, boxes[i].Height + boxes[j].Height);
                        boxes.RemoveAt(j);
                    }
                }
            }
        }
    }

    // Create physics bodies for the boxes
    public static void AddBodies(List<Rectangle> boxes, List<PhysicsBody2D> bodies, int size)
    {
        // Iterate through the edges and create the bodies
        foreach (Rectangle box in boxes)
        {
            Vector2 position = size * (box.Position + new Vector2(box.Width / 2.0f, box.Height / 2.0f));

            float width = box.Width * size;
            float height = box.Height * size;
            //WorldEditor.CreateStaticBody(position, 0f, Vector2.One, 0.5f, ShapeType.Box, out StaticBody2D staticBody, width, height);
            
            StaticBody2D tile = new StaticBody2D(position, 0f, Vector2.One, 0.5f, ShapeType.Box, width:width, height:height);
            bodies.Add(tile);
        }
    }

    //// Draw the background using the tileset and texture map
    //public static void DrawBackground(TileSet tileSet, int[,] textureMap, int size)
    //{
    //    for (int i = 0; i < textureMap.GetLength(0); i++)
    //    {
    //        for (int j = 0; j < textureMap.GetLength(1); j++)
    //        {
    //            if (textureMap[i, j] >= 0)
    //            {
    //                // Iterate through the texture map and draw the tiles
    //                Rectangle source = new Rectangle((textureMap[i, j] % tileSet.columns * tileSet.rect.Width) + tileSet.rect.X, (textureMap[i, j] / tileSet.columns % tileSet.rows * tileSet.rect.Height) + tileSet.rect.Y, tileSet.rect.Width, tileSet.rect.Height);
    //                Rectangle dest = new Rectangle(j * size, i * size, size, size);
    //                Raylib.DrawTexturePro(tileSet.texture, source, dest, new Vector2(0, 0), 0, Color.White);
    //            }
    //        }
    //    }
    //}

    //// Draw the background using the tilemap properties
    //public static void DrawBackground(TileMapProps tileMapProps)
    //{
    //    DrawBackground(tileMapProps.tileSet, tileMapProps.textureMap, tileMapProps.size);
    //}

    public static void CreateTilemapTexture(ref TileMapProps tileMapProps)
    {
        // Create a RenderTexture2D with the appropriate width and height based on the tilemap size
        int width = tileMapProps.textureMap.GetLength(1) * tileMapProps.size;
        int height = tileMapProps.textureMap.GetLength(0) * tileMapProps.size;
        RenderTexture2D renderTexture = Raylib.LoadRenderTexture(width, height);

        // Start drawing to the render texture
        Raylib.BeginTextureMode(renderTexture);
        Raylib.ClearBackground(Color.Blank); // Optional: Clear with transparent background

        for (int i = 0; i < tileMapProps.textureMap.GetLength(0); i++)
        {
            for (int j = 0; j < tileMapProps.textureMap.GetLength(1); j++)
            {
                if (tileMapProps.textureMap[i, j] >= 0)
                {
                    // Calculate source and destination rectangles for drawing the tile
                    Rectangle source = new Rectangle(
                        (tileMapProps.textureMap[i, j] % tileMapProps.tileSet.columns * tileMapProps.tileSet.rect.Width) + tileMapProps.tileSet.rect.X,
                        (tileMapProps.textureMap[i, j] / tileMapProps.tileSet.columns % tileMapProps.tileSet.rows * tileMapProps.tileSet.rect.Height) + tileMapProps.tileSet.rect.Y,
                        tileMapProps.tileSet.rect.Width, -tileMapProps.tileSet.rect.Height
                    );

                    // Invert the Y coordinate when calculating the destination rectangle
                    Rectangle dest = new Rectangle(j * tileMapProps.size, height - (i + 1) * tileMapProps.size, tileMapProps.size, tileMapProps.size);

                    // Draw the tile onto the render texture
                    Raylib.DrawTexturePro(tileMapProps.tileSet.texture, source, dest, new Vector2(0, 0), 0, Color.White);
                }
            }
        }

        Raylib.EndTextureMode(); // Stop drawing to the render texture

        // Return the rendered texture as a Texture2D
        tileMapProps.texture = renderTexture.Texture;
    }

    public static void DrawTilemapTexture(ref TileMapProps tileMapProps)
    {
        // Draw the pre-rendered tilemap texture at the specified position
        Raylib.DrawTexture(tileMapProps.texture, 0, 0, Color.White);
    }

    // Generate the tilemap terrain
    public static void GenerateTileMap(ref TileMapProps tileMapProps, List<PhysicsBody2D> bodies)
    {
        tileMapProps.size = (int)Math.Pow(2, tileMapProps.size + 2);
        GenerateTileMapTerrain(tileMapProps.collisionMap, tileMapProps.size, bodies);
    }

    // Convert JSON tilemap into 2D int array
    public static int[,] GetTilemapFromJSON(string path)
    {
        // Read JSON File
        string json = File.ReadAllText(path);
        var obj = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

        if (obj != null && obj.TryGetValue("layers", out var layersObj))
        {
            var layers = (JsonElement)layersObj;
            foreach (JsonElement layer in layers.EnumerateArray())
            {
                // Check if all valid properties are present
                if (layer.TryGetProperty("data", out var data) &&
                    layer.TryGetProperty("width", out var widthElement) &&
                    layer.TryGetProperty("height", out var heightElement))
                {
                    int width = widthElement.GetInt32();
                    int height = heightElement.GetInt32();
                    int[,] tilemap = new int[height, width];

                    // Convert tilemap data into 2d integer array
                    for (int i = 0; i < data.GetArrayLength(); i++)
                    {
                        int row = i / width;
                        int col = i % width;

                        tilemap[row, col] = data[i].GetInt32() - 1;
                    }
                    return tilemap;
                }
            }
        }

        return null;
    }

    // Get the collision map from the tilemap
    public static int[,] GetCollisionFromTilemap(int[,] tilemap, int[] collisionTiles)
    {
        // Get the dimensions of the tilemap
        int rows = tilemap.GetLength(0);
        int cols = tilemap.GetLength(1);

        // Create a new 2D array to store the collision map
        int[,] collisionMap = new int[rows, cols];

        // Create a HashSet from collisionTiles for efficient lookup
        HashSet<int> collisionSet = new HashSet<int>(collisionTiles);

        // Iterate through the tilemap
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                // Check if the current tile is in the collisionSet
                if (collisionSet.Contains(tilemap[i, j]))
                {
                    // If it is, set the corresponding position in collisionMap to 1
                    collisionMap[i, j] = 1;
                }
                else
                {
                    // Otherwise, set it to 0
                    collisionMap[i, j] = 0;
                }
            }
        }

        // Return the collision map
        return collisionMap;
    }
}
