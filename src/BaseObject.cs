using GameEngine.src.physics;
using System.Numerics;

namespace GameEngine.src;

public abstract class BaseObject
{
    public Transform2D Transform { get; protected set; }
    
    public BaseObject(Vector2 position, float rotation)
    {
        Transform = new Transform2D(position, rotation);
    }

    public virtual void Translate(Vector2 direction)
    {
        Transform.Translate(direction);
    }

    public virtual void Rotate(float angle) 
    {
        Transform.Rotate(angle);
    }

}