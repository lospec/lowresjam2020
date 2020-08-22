using Godot;
using System;
using Godot.Collections;
using HeroesGuild.Utility;
using Array = Godot.Collections.Array;

public class AudioSystem : Node
{
    public static AudioSystem instance;

    public enum Music
    {
        None = -1,
        TitleScreen,
        Overworld,
        Guild,
        BattleBeast,
        BattleDemon,
        BattleFlora,
        BattleGnome,
        BattleHuman,
        BattleRobot,
        BattleSlime,
        GameOver,
    }

    public static AudioStream GetMusicResource(Music music)
    {
        var path = music switch
        {
            Music.TitleScreen => "res://music/title_chiptune.ogg",
            Music.Overworld => "res://music/overworld.ogg",
            Music.Guild => "res://music/guild.ogg",
            Music.BattleBeast => "res://music/battle beast.ogg",
            Music.BattleDemon => "res://music/battle demon.ogg",
            Music.BattleFlora => "res://music/battle flora.ogg",
            Music.BattleGnome => "res://music/battle gnome.ogg",
            Music.BattleHuman => "res://music/battle human.ogg",
            Music.BattleRobot => "res://music/battle robot.ogg",
            Music.BattleSlime => "res://music/battle slime.ogg",
            Music.GameOver => "res://music/game over.ogg",
            _ => throw new ArgumentException()
        };
        return ResourceLoader.Load<AudioStream>(path);
    }

    public enum SFX
    {
        ButtonClick,
        ButtonClickShort,
        ButtonHover,
        Deny,
        Token3,
        Footstep1,
        Footstep2,
        DoorOpen,
        ChestOpen,
        HeroesGuildNox1,
        HeroesGuildNox3,
        HeroesGuildNox4,
        HeroesGuildPureasbestos1,
        HeroesGuildUnsettled1,
        HeroesGuildWildleoknight2,
        BattleIntro,
        VictoryJingle,
        BeastHurt,
        DemonHurt,
        FloraHurt,
        HumanHurt,
        RobotHurt,
        SlimeHurt,
        PlayerHurt,
        QuickAttack,
        HeavyAttack,
        CounterAttack,
        ChargedStatus,
        ConfusionStatus,
        OnfireStatus,
        FrozenStatus,
        PoisonedStatus,
        WeakStatus,
    }

    public static AudioStream GetSFXResource(SFX sfx)
    {
        var path = sfx switch
        {
            SFX.ButtonClick => "res://sfx/ui_confirm.wav",
            SFX.ButtonClickShort => "res://sfx/click_short.wav",
            SFX.ButtonHover => "res://sfx/ui_hover.wav",
            SFX.Deny => "res://sfx/ui_deny.wav",
            SFX.Token3 => "res://sfx/token_3.wav",
            SFX.Footstep1 => "res://sfx/footstep_1.wav",
            SFX.Footstep2 => "res://sfx/footstep_2.wav",
            SFX.DoorOpen => "res://sfx/door_open.wav",
            SFX.ChestOpen => "res://sfx/chest_open.wav",
            SFX.HeroesGuildNox1 => "res://sfx/Heroes_Guild_Nox.wav",
            SFX.HeroesGuildNox3 => "res://sfx/Heroes_Guild_Nox-take3.wav",
            SFX.HeroesGuildNox4 => "res://sfx/Heroes_Guild_Nox-take4.wav",
            SFX.HeroesGuildPureasbestos1 => "res://sfx/Heroes_Guild_PureAsbestos_take1.wav",
            SFX.HeroesGuildUnsettled1 => "res://sfx/Heroes_Guild_Unsettled_take1.wav",
            SFX.HeroesGuildWildleoknight2 => "res://sfx/Heroes_Guild_WildLeoKnight_take2.wav",
            SFX.BattleIntro => "res://sfx/battle intro.wav",
            SFX.VictoryJingle => "res://sfx/Victory jingle.wav",
            SFX.BeastHurt => "res://sfx/Beast_hit.wav",
            SFX.DemonHurt => "res://sfx/Demon_hit.wav",
            SFX.FloraHurt => "res://sfx/Flora_hit.wav",
            SFX.HumanHurt => "res://sfx/Human_hit.wav",
            SFX.RobotHurt => "res://sfx/Robot_hit.wav",
            SFX.SlimeHurt => "res://sfx/Slime_hit.wav",
            SFX.PlayerHurt => "res://sfx/Player_Hit.wav",
            SFX.QuickAttack => "res://sfx/Quick.wav",
            SFX.HeavyAttack => "res://sfx/Heavy.wav",
            SFX.CounterAttack => "res://sfx/Counter.wav",
            SFX.ChargedStatus => "res://sfx/Charged.wav",
            SFX.ConfusionStatus => "res://sfx/Confusion.wav",
            SFX.OnfireStatus => "res://sfx/Fire3.wav",
            SFX.FrozenStatus => "res://sfx/Ice.wav",
            SFX.PoisonedStatus => "res://sfx/Poison.wav",
            SFX.WeakStatus => "res://sfx/Weak.wav",
            _ => throw new ArgumentException()
        };
        return ResourceLoader.Load<AudioStream>(path);
    }

    private const float FADE_IN_START_VOLUME = -80f;
    private const float FADE_IN_DURATION = 0.5f;

    public Music CurrentPlayingMusic = Music.None;

    private float _musicVolume;
    private float _sfxVolume;
    private AudioStreamPlayer _musicPlayer;
    private Tween _tween;

    public static float MusicVolume
    {
        get => instance._musicVolume;
        set
        {
            value = Mathf.Max(-80, value);
            instance._musicPlayer.VolumeDb += value - instance._musicVolume;
            instance._musicVolume = value;
        }
    }
    public static float SFXVolume
    {
        get => instance._sfxVolume;
        set
        {
            value = Mathf.Max(-80, value);
            foreach (AudioStreamPlayer sfxPlayer in instance.GetTree().GetNodesInGroup("sfx_players"))
            {
                sfxPlayer.VolumeDb += value - instance._sfxVolume;
            }

            instance._sfxVolume = value;
        }
    }

    public override void _Ready()
    {
        _musicPlayer = GetNode<AudioStreamPlayer>("Music");
        _tween = GetNode<Tween>("Tween");
        instance = this;
    }

    // Godot signals doesnt work when optional parameters are not provided, hence this overload method 
    public void PlayMusic(Music music, float volumeDb = 0f)
    {
        PlayMusic(music, volumeDb, 1f);
    }
    public void PlayMusic(Music music, float volumeDb, float pitchScale, bool fadeIn = true)
    {
        CurrentPlayingMusic = music;
        _musicPlayer.Stream = GetMusicResource(music);
        volumeDb += _musicVolume;
        if (fadeIn)
        {
            _musicPlayer.VolumeDb = FADE_IN_START_VOLUME;
            _tween.InterpolateProperty(_musicPlayer, "volume_db", FADE_IN_START_VOLUME, volumeDb, FADE_IN_DURATION);
            _tween.Start();
        }
        else
        {
            _musicPlayer.VolumeDb = volumeDb;
        }

        _musicPlayer.PitchScale = pitchScale;
        _musicPlayer.Play();
    }

    public void StopMusic()
    {
        CurrentPlayingMusic = Music.None;
        _musicPlayer.Stop();
    }

    public static AudioStreamPlayer2D PlaySFX(SFX sfx, Vector2? audioPosition = null, float volumeDb = 0f,
        float pitchScale = 1f)
    {
        var sfxPlayer = audioPosition.HasValue
            ? new AudioStreamPlayer2D {Position = audioPosition.Value}
            : new AudioStreamPlayer2D();
        sfxPlayer.Stream = GetSFXResource(sfx);
        volumeDb += instance._sfxVolume;
        sfxPlayer.VolumeDb = volumeDb;
        sfxPlayer.PitchScale = pitchScale;
        instance.AddChild(sfxPlayer);
        sfxPlayer.Play();
        sfxPlayer.Connect("finished", instance, nameof(OnSFXPlayer_Finished), new Array {sfxPlayer});
        sfxPlayer.AddToGroup("sfx_players");
        return sfxPlayer;
    }
    

    private void OnSFXPlayer_Finished(Node sfxPlayer)
    {
        sfxPlayer.QueueFree();
    }
}