using UnityEngine;
using System.Collections;

public class WorldTimeManager
{
    public const float DAY_LENGTH_SECONDS = 60.0f; //60.0f
    public const int SUNSET_TIME = 17;
    public const int NIGHT_START_TIME = 18;
    public const int SUNRISE_TIME = 6;
    public const int DAY_START_TIME = 7;

    public enum LunarPhase { NEW = 0, WAXING_CRESCENT, FIRST_QUARTER, WAXING_GIBBOUS, FULL, WANING_GIBBOUS, LAST_QUARTER, WANING_CRESCENT };

    public delegate void WorldTimeEvent();
    public event WorldTimeEvent OnLunarPhaseChange;
    public event WorldTimeEvent OnSunChange;

    private static WorldTimeManager instance = null;

    private Timer hourTimer = null;

    private LunarPhase currentLunarPhase;

    private float hourLength = 0.0f;

    private int hour = 20; //The current hour from 1-23

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

    public LunarPhase CurrentLunarPhase
    {
        get { return currentLunarPhase; }
        set { currentLunarPhase = value; }
    }

    public int Hour
    {
        get { return hour; }
        set { hour = value; }
    }

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

        if (hour == DAY_START_TIME + 1) //Set the next lunar phase when the sun can't be seen
        {
            if (currentLunarPhase < LunarPhase.WANING_CRESCENT)
                currentLunarPhase++;
            else
                currentLunarPhase = LunarPhase.NEW;

            if (OnLunarPhaseChange != null)
                OnLunarPhaseChange();
        }
        else if (hour == SUNSET_TIME || hour == SUNRISE_TIME - 1) //Set the sun sprite when it can't be seen, sunset can be set while visible.
        {
            if (OnSunChange != null)
                OnSunChange();
        }
        hourTimer.ResetTimer(true);
    }
}
