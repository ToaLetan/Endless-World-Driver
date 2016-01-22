using UnityEngine;
using System.Collections;

public class WorldTimeManager
{
    private const float DAY_LENGTH_SECONDS = 6.0f;

    private static WorldTimeManager instance = null;

    private Timer hourTimer = null;

    private float hourLength = 0.0f;

    private int hour = 8; //The current hour from 1-23

    public static WorldTimeManager Instance
    {
        get
        {
            if (instance == null)
                instance = new WorldTimeManager();
            return instance;
        }
    }

    public Timer HourTimer
    { get { return hourTimer; } }

    public int Hour
    { get { return hour; } }

	// Use this for initialization
	private WorldTimeManager()
    {
        hourLength = DAY_LENGTH_SECONDS / 24; //Using hours as a base time
        hourTimer = new Timer(hourLength, true);
        hourTimer.OnTimerComplete += IncrementHour;
	}
	
	// Update is called once per frame
	public void Update ()
    {
	    if(hourTimer != null)
            hourTimer.Update();
	}

    private void IncrementHour()
    {
        if (hour < 23)
            hour++;
        else
            hour = 1;

        hourTimer.ResetTimer(true);
    }
}
