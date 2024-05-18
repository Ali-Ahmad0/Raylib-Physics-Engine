using GameEngine.src.physics.body;
using GameEngine.src.tilemap;
using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;

namespace Game.res.scenes;

public class TilemapTest : Global
{
    private TileMapProps tileMapProps;
    private List<PhysicsBody2D> bodies;
    private List<Color> colors;

    private Camera2D camera;

    internal TilemapTest()
    {
        bodies = new List<PhysicsBody2D>();

        colors = new List<Color>() {
            Color.White,
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Gold
        };


        tileMapProps = new TileMapProps()
        {
            tileMap = new int[,]
                {
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    { 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
                    { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0 },
                    { 0, 1, 1, 1, 0, 0, 1, 1, 0, 0, 0, 1, 1, 1, 0, 0, 1, 1, 1, 0 }

                },
            size = 4
            
        };

        TileMap.GenerateTileMap(ref tileMapProps, bodies);
        Raylib.ShowCursor();

        // Create a camera centered at the middle of the screen
        camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);
    }

    public override void Update(double delta)
    {
        Raylib.BeginMode2D(camera);

        Raylib.DrawText("Tilemap Test", 20, 20, 32, Color.Green);
        Draw();

        Raylib.EndMode2D();
    }

    private void Draw()
    {
        for (int i = 0; i < bodies.Count; i++)
        {
            DrawCollisionShapes(bodies[i], colors[i % 5]);
        }
    }

}