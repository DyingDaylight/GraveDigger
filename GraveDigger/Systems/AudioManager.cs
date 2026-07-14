using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;

namespace GraveDigger.Systems;

public sealed class AudioManager
{
    private static readonly Lazy<AudioManager> instance = new(() => new AudioManager());
    public static AudioManager Instance => instance.Value;

    private ContentManager content;
    private readonly Dictionary<string, SoundEffectInstance> sfxInstances = new();

    private AudioManager() { }

    public void Initialize(ContentManager contentManager)
    {
        content = contentManager ?? throw new ArgumentNullException(nameof(contentManager));
    }
    
    // SFX
    public void PlaySFX(string name, bool loop = false, float volume = 1.0f)
    {
        if (!sfxInstances.TryGetValue(name, out SoundEffectInstance instance))
        {
            try
            {
                SoundEffect effect = content.Load<SoundEffect>($"Sound/Effects/{name}");
                instance = effect.CreateInstance();
                instance.IsLooped = loop;
                sfxInstances[name] = instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AudioManager] Error loading SFX '{name}': {ex.Message}");
                return;
            }
        }
    
        instance.Volume = MathHelper.Clamp(volume, 0f, 1f);
        instance.Play();
    }

    public void PauseSFX(string name)
    {
        if (sfxInstances.TryGetValue(name, out SoundEffectInstance instance) && instance.State == SoundState.Playing)
            instance.Pause();
    }

    public void StopSFX(string name)
    {
        if (sfxInstances.TryGetValue(name, out SoundEffectInstance instance))
            instance.Stop();
    }

    // MUSIC
    public void PlayMusic(string name, bool loop = true, float volume = 0.2f)
    {
        try
        {
            Song song = content.Load<Song>($"Sound/Music/{name}");
            MediaPlayer.IsRepeating = loop;
        
            MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
        
            MediaPlayer.Play(song);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AudioManager] Error loading Music '{name}': {ex.Message}");
        }
    }

    public void StopMusic()
    {
        MediaPlayer.Stop();
    }
    
    public void SetMusicVolume(float volume)
    {
        MediaPlayer.Volume = MathHelper.Clamp(volume, 0f, 1f);
    }

// Звуковые эффекты (диапазон от 0.0f до 1.0f)
    public void SetSFXVolume(float volume)
    {
        float clampedVolume = MathHelper.Clamp(volume, 0f, 1f);
        SoundEffect.MasterVolume = clampedVolume;
    }
}