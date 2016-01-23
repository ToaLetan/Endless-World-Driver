using UnityEngine;
using System.Collections;

public class SkyManager : MonoBehaviour
{
    private WorldTimeManager worldTimeManager = null;

    private GameObject celestialBodiesObj = null;
    private GameObject sunObj = null;
    private GameObject moonObj = null;

    private SpriteRenderer skyRenderer = null;
    private SpriteRenderer groundOverlayRenderer = null;
    private SpriteRenderer sunRenderer = null;
    private SpriteRenderer moonRenderer = null;

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

    private float sun_Moon_RotationSpeed = 0.0f;
    private float tweenSpeed = 0.0f;

    // Use this for initialization
    void Start ()
    {
        GROUND_SUNSET = new Color(SKY_SUNSET.r, SKY_SUNSET.g, SKY_SUNSET.b, 0.15f);
        GROUND_SUNRISE = new Color(SKY_SUNRISE.r, SKY_SUNRISE.g, SKY_SUNRISE.b, 0.15f);
        GROUND_NIGHT = new Color(SKY_NIGHT.r, SKY_NIGHT.g, SKY_NIGHT.b, 0.75f);

        worldTimeManager = WorldTimeManager.Instance;
        worldTimeManager.HourTimer.OnTimerComplete += UpdateSky;

        sun_Moon_RotationSpeed = 360 / WorldTimeManager.DAY_LENGTH_SECONDS;
        tweenSpeed = 24 / WorldTimeManager.DAY_LENGTH_SECONDS; //Fade transitions last the entire hour

        celestialBodiesObj = gameObject.transform.FindChild("Sun_Moon_Rotation").gameObject;
        sunObj = celestialBodiesObj.transform.FindChild("Sun").gameObject;
        moonObj = celestialBodiesObj.transform.FindChild("Moon").gameObject;

        sunRenderer = celestialBodiesObj.transform.FindChild("Sun").GetComponent<SpriteRenderer>();
        moonRenderer = celestialBodiesObj.transform.FindChild("Moon").GetComponent<SpriteRenderer>();

        skyRenderer = gameObject.GetComponent<SpriteRenderer>();
        groundOverlayRenderer = gameObject.transform.FindChild("Ground_Overlay").GetComponent<SpriteRenderer>();
        groundOverlayRenderer.enabled = true;

        currentSkyTint = SKY_CLEAR_DAY; //Starting at day since world time starts at 8 AM
        currentGroundTint = GROUND_CLEAR_DAY;

        SetSky();
        //UpdateSky();
        SetCelestialRotation();
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
        UpdateCelestialRotation();
    }

    private void UpdateSky()
    {
        Debug.Log(worldTimeManager.Hour);

        //Set the sky tint based on time of day. Eventually make this gradually tween.
        if (worldTimeManager.Hour == 17) //SUNSET: 5 PM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_SUNSET, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_SUNSET, tweenSpeed);
        }  
        else if (worldTimeManager.Hour == 18) //NIGHT: 6 PM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_NIGHT, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_NIGHT, tweenSpeed);
        }
        else if (worldTimeManager.Hour == 6) //SUNRISE: 5 AM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_SUNRISE, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_SUNRISE, tweenSpeed);
        }
        else if(worldTimeManager.Hour == 7)//DAY: 6 AM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_CLEAR_DAY, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_CLEAR_DAY, tweenSpeed);
        }

        skyRenderer.color = currentSkyTint;
    }

    private void UpdateCelestialRotation()
    {
        float angle = sun_Moon_RotationSpeed * Time.deltaTime;

        celestialBodiesObj.transform.Rotate(new Vector3(0, 0, angle));
        sunObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        moonObj.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void SetSky()
    {
        if(worldTimeManager.Hour == 17) //Sunset
        {
            currentSkyTint = SKY_SUNSET;
            currentGroundTint = GROUND_SUNSET;
        }
        else if(worldTimeManager.Hour >= 18 || worldTimeManager.Hour <= 4) //Night
        {
            currentSkyTint = SKY_NIGHT;
            currentGroundTint = GROUND_NIGHT;
        }
        else if(worldTimeManager.Hour == 5) //Sunrise
        {
            currentSkyTint = SKY_SUNRISE;
            currentGroundTint = GROUND_SUNRISE;
        }
        else //Day
        {
            currentSkyTint = SKY_CLEAR_DAY;
            currentGroundTint = GROUND_CLEAR_DAY;
        }

        skyRenderer.color = currentSkyTint;
        groundOverlayRenderer.color = currentGroundTint;
    }

    private void SetCelestialRotation()
    {
        float angle = 360 / 24 * worldTimeManager.Hour;

        celestialBodiesObj.transform.localRotation = Quaternion.Euler(0, 0, angle);
        sunObj.transform.rotation = Quaternion.Euler(0, 0, 0);
        moonObj.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
