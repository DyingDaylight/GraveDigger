using System;
using Interfaces;
using Microsoft.Xna.Framework;

namespace GraveDigger.Systems;

public class TimeSystem : IUpdatable
{
    private float elapsedTime;
    
    public float DayDuration { get; }
    public float NightDuration { get; }

    public int CurrentDay { get; private set; } = 1;
    public DayTime CurrentDayTime { get; private set; } = DayTime.Day;

    public event Action<int> DayStarted;
    public event Action<DayTime> DayTimeChanged;
    public event Action<float> TimeUpdated;
    
    public float CurrentPhaseDuration => CurrentDayTime == DayTime.Day ? DayDuration : NightDuration;
    public float PhaseProgress => elapsedTime / CurrentPhaseDuration;
    public float CycleProgress
    {
        get
        {
            float cycleElapsedTime = CurrentDayTime == DayTime.Day
                ? elapsedTime
                : DayDuration + elapsedTime;

            return cycleElapsedTime / (DayDuration + NightDuration);
        }
    }
    
    public TimeSystem(float dayDuration, float nightDuration)
    {
        if (dayDuration <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(dayDuration),
                "Day duration must be greater than zero.");

        if (nightDuration <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(nightDuration),
                "Night duration must be greater than zero.");
        
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
        
        Advance();
    }

    public void AdvanceTime(int seconds)
    {
        elapsedTime += seconds;
        Advance();
    }
    
    private void Advance()
    {
        float duration = CurrentPhaseDuration;

        while (elapsedTime >= duration)
        {
            elapsedTime -= duration;

            CurrentDayTime =
                CurrentDayTime == DayTime.Day
                    ? DayTime.Night
                    : DayTime.Day;

            DayTimeChanged?.Invoke(CurrentDayTime);
            
            if (CurrentDayTime == DayTime.Day)
            {
                CurrentDay++;
                DayStarted?.Invoke(CurrentDay);
            }
            
            duration = CurrentPhaseDuration;
        }
        
        TimeUpdated?.Invoke(CycleProgress);
    }
}