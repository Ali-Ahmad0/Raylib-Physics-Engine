
using Game.res.scenes;
using GameEngine.src.main;
using GameEngine.src.physics.body;
using GameEngine.src.physics.component;
using GameEngine.src.tilemap;
using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;

namespace GameEngine.example.scenes.game;

public class Level3 : World
{
    private TileMapProps tileMapProps;
    private List<PhysicsBody2D> bodies;

    private Camera2D camera;

    internal Level3()
    {
        bodies = new List<PhysicsBody2D>();

        // Create player
        //CreatePlayerBody(new Vector2(256, 512), 0, 1f, 64f, 128f, out PlayerBody2D player);
        Player player = new Player(new Vector2(256, 512), 0, 64f, 128f);
        bodies.Add(player);

        int[,] tilemap = TileMap.GetTilemapFromJSON(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/tilemap/") + "level_3.json");
        int[] collisionTiles = { 16, 17, 18, 31, 33, 46, 47, 48, 136, 137, 138, 142, 145, 151, 152, 153, 160 };

        tileMapProps = new TileMapProps()
        {
            textureMap = tilemap,
            collisionMap = TileMap.GetCollisionFromTilemap(tilemap, collisionTiles),
            size = 4,

            tileSet = new TileSet(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/tilemap/") + "tileset.png", new Rectangle(0, 0, 16, 16), 15, 18)
        };

        TileMap.GenerateTileMap(ref tileMapProps, bodies);
        Raylib.ShowCursor();

        // Create a camera centered at the middle of the screen
        camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1f);
    }

    public override void Update(double delta)
    {
        Player player = (Player)bodies[0];

        // Center the camera on the player's position

        camera.Target = new Vector2(player.Transform.Translation.X + player.CollisionShape.Dimensions.Width / 2, player.Transform.Translation.Y + player.CollisionShape.Dimensions.Height / 2);
        camera.Offset = new Vector2(Raylib.GetScreenWidth() / 2f, Raylib.GetScreenHeight() / 2f);

        // Limit camera movement within the tilemap bounds

        camera.Target.X = Math.Clamp(camera.Target.X, 768, (tileMapProps.textureMap.GetLength(1) * 64) - Raylib.GetScreenWidth() / 2);
        camera.Target.Y = Math.Clamp(camera.Target.Y, 512, (tileMapProps.textureMap.GetLength(0) * 64) - Raylib.GetScreenHeight() / 2);

        if (player.Transform.Translation.Y > 1216)
        {
            SceneTree.Scene = 4;
        }

        // Begin 2D mode with the camera
        Raylib.BeginMode2D(camera);
        Draw();

        player.Update(delta);

        if (player.Transform.Translation.X > 6230)
        {
            SceneTree.Scene = 4;
        }

        Raylib.EndMode2D();

        HandlePhysics(bodies, delta, camera);
        Raylib.DrawText("Level - 3", 64, 32, 40, Color.Red);
    }

    private void Draw()
    {
        TileMap.DrawBackground(tileMapProps);
    }
}