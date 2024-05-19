using GameEngine.src.main;
using GameEngine.src.physics.body;
using GameEngine.src.tilemap;
using GameEngine.src.world;
using Raylib_cs;
using System.Numerics;

namespace GameEngine.example.scenes.game
{
    public class Level1 : Global
    {
        private TileMapProps tileMapProps;
        private List<PhysicsBody2D> bodies;

        private Camera2D camera;

        internal Level1()
        {
            bodies = new List<PhysicsBody2D>();

            // Create player
            CreatePlayerBody(new Vector2(128, 512), 0, 1f, 64f, 128f, out PlayerBody2D player);
            bodies.Add(player);

            int[,] tilemap = TileMap.GetTilemapFromJSON(Path.Combine(Properties.ExecutableDirectory, "../../../example/assets/tilemap/") + "level_1.json");
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
            PlayerBody2D player = (PlayerBody2D)bodies[0];
            player.UseDefaultPlayer(delta);

            // Center the camera on the player's position
            camera.Target = new Vector2(player.Transform.Translation.X + player.Dimensions.Width / 2, player.Transform.Translation.Y + player.Dimensions.Height / 2);
            camera.Offset = new Vector2(Raylib.GetScreenWidth() / 2f, Raylib.GetScreenHeight() / 2f);


            // Begin 2D mode with the camera
            Raylib.BeginMode2D(camera);
            Draw();
            Raylib.EndMode2D();

            HandlePhysics(bodies, delta, camera);
        }

        private void Draw()
        {
            TileMap.DrawBackground(tileMapProps);
        }

    }
}
