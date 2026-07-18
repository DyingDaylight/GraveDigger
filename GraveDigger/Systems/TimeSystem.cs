using System;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Systems;

public class TimeSystem : IUpdatable
{
    public float DayDuration { get; }
    public float NightDuration { get; }

    public int CurrentDay { get; private set; } = 1;
    public DayTime CurrentDayTime { get; private set; } = DayTime.Day;

    public event Action<int> DayStarted;
    public event Action<DayTime> DayTimeChanged;
    
    private float elapsedTime;
    
    public TimeSystem(float dayDuration, float nightDuration)
    {
        DayDuration = dayDuration;
        NightDuration = nightDuration;
    }
    
    public void Start()
    {
        elapsedTime = 0;
        CurrentDay = 1;
        CurrentDayTime = DayTime.Day;
    }

    public void Update(GameTime gameTime)
    {
        elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

        float duration = CurrentDayTime == DayTime.Day
            ? DayDuration
            : NightDuration;

        if (elapsedTime >= duration)
        {
            elapsedTime -= duration;

            CurrentDayTime = CurrentDayTime == DayTime.Day
                ? DayTime.Night
                : DayTime.Day;
            
            if (CurrentDayTime == DayTime.Day)
            {
                CurrentDay++;
                DayStarted?.Invoke(CurrentDay);
            }
            
            DayTimeChanged?.Invoke(CurrentDayTime);
        }
    }

}