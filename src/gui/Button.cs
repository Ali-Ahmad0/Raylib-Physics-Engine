using GameEngine.src.helper;
using Raylib_cs;
using System.Numerics;

namespace GameEngine.src.gui;

public abstract class Button
{
    // Area of button
    protected Rectangle Rectangle;
    
    // Bounds 
    public Vector2 Min;
    public Vector2 Max;

    protected Button(Rectangle rectangle) 
    {
        Rectangle = rectangle;
        
        Min = new Vector2(rectangle.X, rectangle.Y);
        Max = new Vector2(rectangle.X + rectangle.Width, rectangle.Y + rectangle.Height);
    }

    public bool IsHovered()
    {
        return !(Mouse.GetPos().X <= Min.X || Mouse.GetPos().X >= Max.X
            || Mouse.GetPos().Y <= Min.Y || Mouse.GetPos().Y >= Max.Y);
    }

    public bool IsClicked()
    {
        return (IsHovered() && Mouse.IsLMBPressed());
    }

    public abstract void DrawButton();
}

public class SimpleButton : Button
{
    // Attributes
    public Color ButtonColor;
    public string Text;
    private int fontSize; // Backing field for FontSize property
    public int FontSize
    {
        get { return fontSize; }
        set { fontSize = value < 0 ? -value : value; }
    }

    public Color FontColor;

    // Constructors
    public SimpleButton(Rectangle rectangle, Color buttonColor, string text)
        : base(rectangle)
    {
        ButtonColor = buttonColor;
        Text = text;
        FontSize = 16;
        FontColor = Color.Black;
    }

    public SimpleButton(Rectangle rectangle, Color buttonColor, string text, int fontSize)
        : this(rectangle, buttonColor, text)
    {
        FontSize = fontSize;
    }

    public SimpleButton(Rectangle rectangle, Color buttonColor, string text, int fontSize, Color fontColor)
        : this(rectangle, buttonColor, text, fontSize)
    {
        FontColor = fontColor;
    }

    public override void DrawButton()
    {
        // Draw the button rectangle
        Raylib.DrawRectangleRec(Rectangle, ButtonColor);

        // Measure the text to find its width and height
        int textWidth = Raylib.MeasureText(Text, FontSize);
        int textHeight = FontSize;

        // Calculate the position to draw the text at the center of the button
        float textX = Rectangle.X + (Rectangle.Width / 2) - (textWidth / 2);
        float textY = Rectangle.Y + (Rectangle.Height / 2) - (textHeight / 2);

        // Draw the text
        Raylib.DrawText(Text, (int)textX, (int)textY, FontSize, FontColor);
    }
}


public class TextureButton : Button
{
    public Texture2D Texture;

    public TextureButton(Rectangle rectangle, Texture2D texture) : base(rectangle)
    {
        Texture = texture;
    }

    public override void DrawButton()
    {
        // Calculate the scaling factors for the texture
        float scaleX = Rectangle.Width / (float)Texture.Width;
        float scaleY = Rectangle.Height / (float)Texture.Height;

        // Draw the texture within the base rectangle with proper scaling
        Raylib.DrawTexturePro(Texture, new Rectangle(0, 0, Texture.Width, Texture.Height), Rectangle,
                               new Vector2(0, 0), 0f, Color.White);
    }
}

