using UnityEngine;
using System.Collections;

public class SkyManager : MonoBehaviour
{
    private WorldTimeManager worldTimeManager = null;

    private SpriteRenderer skyRenderer = null;

    private Color currentSkyTint = Color.white;

	// Use this for initialization
	void Start ()
    {
        worldTimeManager = WorldTimeManager.Instance;
        worldTimeManager.HourTimer.OnTimerComplete += UpdateSky;

        skyRenderer = gameObject.GetComponent<SpriteRenderer>();

        UpdateSky();

    }
	
	// Update is called once per frame
	void Update ()
    {
        if (worldTimeManager != null)
            worldTimeManager.Update();
	}

    private void UpdateSky()
    {
        //Debug.Log(worldTimeManager.Hour);
        if (worldTimeManager.Hour >= 18 && worldTimeManager.Hour <= 20)
            currentSkyTint = Color.yellow;
        else
            currentSkyTint = Color.blue;

        skyRenderer.color = currentSkyTint;
    }
}
