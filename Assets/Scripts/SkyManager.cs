using UnityEngine;
using System.Collections;

public class SkyManager : MonoBehaviour
{
    private const float TWEEN_SPEED = 5.0f;

    private WorldTimeManager worldTimeManager = null;

    private SpriteRenderer skyRenderer = null;
    private SpriteRenderer groundOverlayRenderer = null;

    private ColourTween skyColourShift = null;
    private ColourTween groundColourShift = null;

    //Sky Colour Tints
    private Color SKY_CLEAR_DAY = new Color(0.28f, 0.55f, 0.94f);
    private Color SKY_SUNSET = new Color(0.91f, 0.25f, 0.1f);
    private Color SKY_SUNRISE = new Color(0.9f, 0.84f, 0.34f);
    private Color SKY_NIGHT = new Color(0.09f, 0.09f, 0.12f);
    //Ground Colour Tints
    private Color GROUND_CLEAR_DAY = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color GROUND_SUNSET = new Color();
    private Color GROUND_SUNRISE = new Color();
    private Color GROUND_NIGHT = new Color();

    private Color currentSkyTint;
    private Color currentGroundTint;

    // Use this for initialization
    void Start ()
    {
        GROUND_SUNSET = new Color(SKY_SUNSET.r, SKY_SUNSET.g, SKY_SUNSET.b, 0.15f);
        GROUND_SUNRISE = new Color(SKY_SUNRISE.r, SKY_SUNRISE.g, SKY_SUNRISE.b, 0.15f);
        GROUND_NIGHT = new Color(SKY_NIGHT.r, SKY_NIGHT.g, SKY_NIGHT.b, 0.75f);

        worldTimeManager = WorldTimeManager.Instance;
        worldTimeManager.HourTimer.OnTimerComplete += UpdateSky;

        skyRenderer = gameObject.GetComponent<SpriteRenderer>();
        groundOverlayRenderer = gameObject.transform.FindChild("Ground_Overlay").GetComponent<SpriteRenderer>();

        currentSkyTint = SKY_CLEAR_DAY; //Starting at day since world time starts at 8 AM
        currentGroundTint = GROUND_CLEAR_DAY;

        UpdateSky();

        //colourShift = new ColourTween(currentSkyTint, SKY_SUNSET, TWEEN_SPEED);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (worldTimeManager != null)
            worldTimeManager.Update();

        if (skyColourShift != null && groundColourShift != null)
        {
            skyColourShift.Update();
            groundColourShift.Update();

            currentSkyTint = skyColourShift.CurrentColour;
            skyRenderer.color = currentSkyTint;

            currentGroundTint = groundColourShift.CurrentColour;
            groundOverlayRenderer.color = currentGroundTint;
        }
            
	}

    private void UpdateSky()
    {
        Debug.Log(worldTimeManager.Hour);

        //Set the sky tint based on time of day. Eventually make this gradually tween.
        if (worldTimeManager.Hour == 18) //SUNSET: 6 PM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_SUNSET, TWEEN_SPEED);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_SUNSET, TWEEN_SPEED);
        }  
        else if (worldTimeManager.Hour == 21) //NIGHT: 9 PM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_NIGHT, TWEEN_SPEED);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_NIGHT, TWEEN_SPEED);
        }
        else if (worldTimeManager.Hour == 6) //SUNRISE: 6 AM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_SUNRISE, TWEEN_SPEED);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_SUNRISE, TWEEN_SPEED);
        }
        else if(worldTimeManager.Hour == 8)//DAY: 8 AM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_CLEAR_DAY, TWEEN_SPEED);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_CLEAR_DAY, TWEEN_SPEED);
        }

        skyRenderer.color = currentSkyTint;
    }
}
