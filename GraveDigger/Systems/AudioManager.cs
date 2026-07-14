using System;
using System.Collections.Generic;
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
    public void PlaySFX(string name, bool loop = false)
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
    public void PlayMusic(string name, bool loop = true)
    {
        try
        {
            Song song = content.Load<Song>($"Sound/Music/{name}");
            MediaPlayer.IsRepeating = loop;
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
}