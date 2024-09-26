using GameEngine.src.physics.component;
using GameEngine.src.physics.collision.shape;
using System.Numerics;

namespace GameEngine.src.physics.body;

// A special type of rigid body that doesn't have rotational motion
public class CharacterBody2D : RigidBody2D
{

    public CharacterBody2D(Vector2 position, float rotation, Vector2 scale, float width, float height) :
        base(position, rotation, Vector2.One, 0.985f, 0f, ShapeType.Box, width: width *= scale.X, height: height *= scale.Y)
    {
        // Gravity is optional for the player
        components = new List<Component>() { new Motion() };
    }

}
