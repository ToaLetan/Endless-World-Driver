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
        worldTimeManager.OnLunarPhaseChange += UpdateMoonSprite;
        worldTimeManager.OnSunChange += UpdateSunSprite;

        sun_Moon_RotationSpeed = 360 / WorldTimeManager.DAY_LENGTH_SECONDS;
        tweenSpeed = 24 / WorldTimeManager.DAY_LENGTH_SECONDS; //Fade transitions last the entire hour

        celestialBodiesObj = gameObject.transform.FindChild("Sun Moon Rotation").gameObject;
        sunObj = celestialBodiesObj.transform.FindChild("Sun").gameObject;
        moonObj = celestialBodiesObj.transform.FindChild("Moon").gameObject;

        sunRenderer = celestialBodiesObj.transform.FindChild("Sun").GetComponent<SpriteRenderer>();
        moonRenderer = celestialBodiesObj.transform.FindChild("Moon").GetComponent<SpriteRenderer>();

        skyRenderer = gameObject.GetComponent<SpriteRenderer>();
        groundOverlayRenderer = gameObject.transform.FindChild("Ground Overlay").GetComponent<SpriteRenderer>();
        groundOverlayRenderer.enabled = true;

        currentSkyTint = SKY_CLEAR_DAY; //Starting at day since world time starts at 8 AM
        currentGroundTint = GROUND_CLEAR_DAY;

        RandomizeWorldTimeInfo();
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
        //Set the sky tint based on time of day. Eventually make this gradually tween.
        if (worldTimeManager.Hour == WorldTimeManager.SUNSET_TIME) //SUNSET: 5 PM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_SUNSET, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_SUNSET, tweenSpeed);
        }  
        else if (worldTimeManager.Hour == WorldTimeManager.NIGHT_START_TIME) //NIGHT: 6 PM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_NIGHT, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_NIGHT, tweenSpeed);
        }
        else if (worldTimeManager.Hour == WorldTimeManager.SUNRISE_TIME) //SUNRISE: 6 AM
        {
            skyColourShift = new ColourTween(currentSkyTint, SKY_SUNRISE, tweenSpeed);
            groundColourShift = new ColourTween(currentGroundTint, GROUND_SUNRISE, tweenSpeed);
        }
        else if(worldTimeManager.Hour == WorldTimeManager.DAY_START_TIME)//DAY: 7 AM
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
        if(worldTimeManager.Hour == WorldTimeManager.SUNSET_TIME) //Sunset
        {
            currentSkyTint = SKY_SUNSET;
            currentGroundTint = GROUND_SUNSET;
        }
        else if(worldTimeManager.Hour >= WorldTimeManager.NIGHT_START_TIME || worldTimeManager.Hour <= WorldTimeManager.SUNRISE_TIME - 1) //Night
        {
            currentSkyTint = SKY_NIGHT;
            currentGroundTint = GROUND_NIGHT;
        }
        else if(worldTimeManager.Hour == WorldTimeManager.SUNRISE_TIME) //Sunrise
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

    private void RandomizeWorldTimeInfo()
    {
        worldTimeManager.Hour = Random.Range(1, 24);
        worldTimeManager.CurrentLunarPhase = (WorldTimeManager.LunarPhase)Random.Range((int)WorldTimeManager.LunarPhase.NEW, (int)WorldTimeManager.LunarPhase.WANING_CRESCENT + 1);

        Debug.Log(worldTimeManager.CurrentLunarPhase);
        UpdateMoonSprite();
    }

    private void UpdateMoonSprite()
    {
        SetCelestialSprite(moonRenderer);
    }

    private void UpdateSunSprite()
    {
        SetCelestialSprite(sunRenderer);
    }

    private void SetCelestialSprite(SpriteRenderer rendererToSet)
    {
        Sprite newSprite = null;
        string spriteName = "";

        if(rendererToSet == moonRenderer)
        {
            switch(worldTimeManager.CurrentLunarPhase)
            {
                case WorldTimeManager.LunarPhase.NEW:
                    spriteName = "New";
                    break;
                case WorldTimeManager.LunarPhase.WAXING_CRESCENT:
                    spriteName = "WaxingCrescent";
                    break;
                case WorldTimeManager.LunarPhase.FIRST_QUARTER:
                    spriteName = "FirstQuarter";
                    break;
                case WorldTimeManager.LunarPhase.WAXING_GIBBOUS:
                    spriteName = "WaxingGibbous";
                    break;
                case WorldTimeManager.LunarPhase.FULL:
                    spriteName = "Full";
                    break;
                case WorldTimeManager.LunarPhase.WANING_GIBBOUS:
                    spriteName = "WaningGibbous";
                    break;
                case WorldTimeManager.LunarPhase.LAST_QUARTER:
                    spriteName = "LastQuarter";
                    break;
                case WorldTimeManager.LunarPhase.WANING_CRESCENT:
                    spriteName = "WaningCrescent";
                    break;
                default:
                    break;
            }
            newSprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/Moon_Sun_Sheet", "Moon_" + spriteName);
        }
        else if (rendererToSet == sunRenderer)
        {
            if (worldTimeManager.Hour == WorldTimeManager.SUNSET_TIME)
            {
                if(rendererToSet.sprite.name.Contains("Cool"))
                    spriteName = "CoolSetting";
                else
                    spriteName = "Setting";
            }
            else
            {
                if (Random.value < 0.05f) //5% chance for the sun to be extra cool.
                    spriteName = "Cool";
                else
                    spriteName = "Day"; //Use the standard sprite
            }

            newSprite = SpriteSheetLoader.LoadSpriteFromSheet("Sprites/Moon_Sun_Sheet", "Sun_" + spriteName);
        }
        rendererToSet.sprite = newSprite;
    }
}
