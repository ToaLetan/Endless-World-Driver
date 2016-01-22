using UnityEngine;
using System.Collections;

public class Tween
{
    protected float speed = 0.0f;

    public Tween(float tweenSpeed)
    {
        speed = tweenSpeed;
    }

    public virtual void Update()
    {
        UpdateTween();
    }

    public virtual void UpdateTween()
    {

    }
}

public class ColourTween : Tween
{
    private Color toColour;
    private Color currentColour;

    public Color CurrentColour
    {
        get { return currentColour; }
    }

    public ColourTween(Color colourFrom, Color colourTo, float tweenSpeed) : base(tweenSpeed)
    {
        currentColour = colourFrom;
        toColour = colourTo;
        speed = tweenSpeed;
    }

    public override void Update()
    {
        UpdateTween();
    }

    public override void UpdateTween()
    {
        if(currentColour != toColour)
        {
            float red = (toColour.r - currentColour.r) * speed * Time.deltaTime;
            float green = (toColour.g - currentColour.g) * speed * Time.deltaTime;
            float blue = (toColour.b - currentColour.b) * speed * Time.deltaTime;
            float alpha = (toColour.a - currentColour.a) * speed * Time.deltaTime;

            currentColour = new Color(currentColour.r + red, currentColour.g + green, currentColour.b + blue, currentColour.a + alpha);
        }
    }
}

public static class TweenFunctions
{

}
