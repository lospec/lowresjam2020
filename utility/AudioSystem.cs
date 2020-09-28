using System;
using Godot;
using HeroesGuild.data;
using Array = Godot.Collections.Array;

namespace HeroesGuild.utility
{
	public class AudioSystem : Node
	{
		public enum Music
		{
			TitleScreen,
			Overworld,
			Guild,
			BattleIntro,
			BattleBeast,
			BattleDemon,
			BattleFlora,
			BattleGnome,
			BattleHuman,
			BattleRobot,
			BattleSlime,
			GameOver
		}

		public enum SFX
		{
			ButtonClick,
			ButtonClickShort,
			ButtonHover,
			Deny,
			Token,
			Footstep1,
			Footstep2,
			DoorOpen,
			ChestOpen,
			HeroesGuildNox1,
			HeroesGuildNox2,
			HeroesGuildNox3,
			HeroesGuildPureAsbestos,
			HeroesGuildUnsettled,
			HeroesGuildWildleoknight,
			VictoryJingle,
			BeastHurt,
			DemonHurt,
			FloraHurt,
			HumanHurt,
			RobotHurt,
			SlimeHurt,
			PlayerHurt,
			GnomeHurt,
			QuickAttack,
			HeavyAttack,
			CounterAttack,
			ChargedStatus,
			ConfusionStatus,
			OnFireStatus,
			FrozenStatus,
			PoisonedStatus,
			WeakStatus
		}

		private const float FADE_IN_START_VOLUME = -80f;
		private const float FADE_IN_DURATION = 0.5f;
		private const string MUSIC_PLAYERS_GROUP_NAME = "music_players";
		private const string SFX_PLAYERS_GROUP_NAME = "sfx_players";

		public static AudioSystem instance;

		private float _musicVolume;
		private float _sfxVolume;
		private Tween _tween;

		public static float MusicVolume
		{
			get => instance._musicVolume;
			set
			{
				value = Mathf.Max(-80, value);
				foreach (AudioStreamPlayer sfxPlayer in GetMusicPlayers())
					sfxPlayer.VolumeDb += value - instance._sfxVolume;
				instance._musicVolume = value;
			}
		}
		public static float SFXVolume
		{
			get => instance._sfxVolume;
			set
			{
				value = Mathf.Max(-80, value);
				foreach (AudioStreamPlayer sfxPlayer in GetSFXPlayers())
					sfxPlayer.VolumeDb += value - instance._sfxVolume;
				instance._sfxVolume = value;
			}
		}

		public static bool IsMusicPlaying
		{
			get
			{
				return GetMusicPlayers().Count > 0;
			}
		}

		public override void _Ready()
		{
			_tween = GetNode<Tween>("Tween");
			instance = this;
		}

		public static AudioStream GetMusicResource(Music music)
		{
			var path = music switch
			{
				Music.TitleScreen => "res://music/title_chiptune.ogg",
				Music.Overworld => "res://music/overworld.ogg",
				Music.Guild => "res://music/guild.ogg",
				Music.BattleIntro => "res://music/battle_intro.ogg",
				Music.BattleBeast => "res://music/battle_beast.ogg",
				Music.BattleDemon => "res://music/battle_demon.ogg",
				Music.BattleFlora => "res://music/battle_flora.ogg",
				Music.BattleGnome => "res://music/battle_gnome.ogg",
				Music.BattleHuman => "res://music/battle_human.ogg",
				Music.BattleRobot => "res://music/battle_robot.ogg",
				Music.BattleSlime => "res://music/battle_slime.ogg",
				Music.GameOver => "res://music/game_over.ogg",
				_ => throw new ArgumentException()
			};
			return ResourceLoader.Load<AudioStream>(path);
		}

		public static AudioStream GetSFXResource(SFX sfx)
		{
			var path = sfx switch
			{
				SFX.ButtonClick => "res://sfx/ui_confirm.wav",
				SFX.ButtonClickShort => "res://sfx/click_short.wav",
				SFX.ButtonHover => "res://sfx/ui_hover.wav",
				SFX.Deny => "res://sfx/ui_deny.wav",
				SFX.Token => "res://sfx/token_3.wav",
				SFX.Footstep1 => "res://sfx/footstep_1.wav",
				SFX.Footstep2 => "res://sfx/footstep_2.wav",
				SFX.DoorOpen => "res://sfx/door_open.wav",
				SFX.ChestOpen => "res://sfx/chest_open.wav",
				SFX.HeroesGuildNox1 => "res://sfx/heroes_guild_nox.wav",
				SFX.HeroesGuildNox2 => "res://sfx/heroes_guild_nox2.wav",
				SFX.HeroesGuildNox3 => "res://sfx/heroes_guild_nox3.wav",
				SFX.HeroesGuildPureAsbestos =>
					"res://sfx/heroes_guild_pureasbestos.wav",
				SFX.HeroesGuildUnsettled =>
					"res://sfx/heroes_guild_unsettled.wav",
				SFX.HeroesGuildWildleoknight =>
					"res://sfx/heroes_guild_wildleoknight.wav",
				SFX.VictoryJingle => "res://sfx/victory_jingle.wav",
				SFX.BeastHurt => "res://sfx/beast_hit.wav",
				SFX.DemonHurt => "res://sfx/demon_hit.wav",
				SFX.FloraHurt => "res://sfx/flora_hit.wav",
				SFX.HumanHurt => "res://sfx/human_hit.wav",
				SFX.RobotHurt => "res://sfx/robot_hit.wav",
				SFX.SlimeHurt => "res://sfx/slime_hit.wav",
				SFX.PlayerHurt => "res://sfx/player_hit.wav",
				SFX.QuickAttack => "res://sfx/quick.wav",
				SFX.HeavyAttack => "res://sfx/heavy.wav",
				SFX.CounterAttack => "res://sfx/counter.wav",
				SFX.ChargedStatus => "res://sfx/charged.wav",
				SFX.ConfusionStatus => "res://sfx/confusion.wav",
				SFX.OnFireStatus => "res://sfx/fire3.wav",
				SFX.FrozenStatus => "res://sfx/ice.wav",
				SFX.PoisonedStatus => "res://sfx/poison.wav",
				SFX.WeakStatus => "res://sfx/weak.wav",
				_ => throw new ArgumentException()
			};
			return ResourceLoader.Load<AudioStream>(path);
		}

		public static MusicRecord? GetMusicRecord(string musicName)
		{
			var data = Autoload.Get<Data>();
			if (!data.musicData.TryGetValue(musicName, out MusicRecord musicRecord))
			{
				GD.PushError($"Music record not found with key \"{musicName}\"");
				return null;
			}

			return musicRecord;
		}

		public static SFXRecord? GetSFXRecord(string sfxName)
		{
			var data = Autoload.Get<Data>();
			if (!data.sfxData.TryGetValue(sfxName, out SFXRecord sfxRecord))
			{
				GD.PushError($"SFX record not found with key \"{sfxName}\"");
				return null;
				
			}

			return sfxRecord;
		}

		public static AudioStreamPlayer PlayMusic(string musicName)
		{
			return PlayMusic(GetMusicRecord(musicName).Value);
		}

		public static AudioStreamPlayer PlayMusic(MusicRecord musicRecord)
		{
			AudioStream audioStream = GetMusicResource(musicRecord.musicClip);
			AudioStreamPlayer musicPlayer = instance.PlayAudio(audioStream, musicRecord.volumeDb, 1f, musicRecord.fadeIn, musicRecord.playbackPosition);
			musicPlayer.AddToGroup(MUSIC_PLAYERS_GROUP_NAME);
			return musicPlayer;
		}

		public static AudioStreamPlayer PlaySFX(string sfxName)
		{
			return PlaySFX(GetSFXRecord(sfxName).Value);
		}

		public static AudioStreamPlayer2D PlaySFX(string sfxName, Vector2 audioPosition)
		{
			return PlaySFX(GetSFXRecord(sfxName).Value, audioPosition);
		}

		public static AudioStreamPlayer PlaySFX(SFXRecord sfxRecord)
		{
			AudioStream audioStream = GetSFXResource(sfxRecord.sfxClip);

			float volumeDb = sfxRecord.minVolumeDb == sfxRecord.maxVolumeDb ?
				sfxRecord.minVolumeDb :
				(float)GD.RandRange(sfxRecord.minVolumeDb, sfxRecord.maxVolumeDb);

			float pitchScale = sfxRecord.minPitch == sfxRecord.maxPitch ?
				sfxRecord.minPitch :
				(float)GD.RandRange(sfxRecord.minPitch, sfxRecord.maxPitch);

			AudioStreamPlayer sfxPlayer = instance.PlayAudio(audioStream, volumeDb, pitchScale, false, 0f);
			sfxPlayer.AddToGroup(SFX_PLAYERS_GROUP_NAME);
			return sfxPlayer;
		}

		public static AudioStreamPlayer2D PlaySFX(SFXRecord sfxRecord, Vector2 audioPosition)
		{
			AudioStream audioStream = GetSFXResource(sfxRecord.sfxClip);

			float volumeDb = sfxRecord.minVolumeDb == sfxRecord.maxVolumeDb ?
				sfxRecord.minVolumeDb :
				(float)GD.RandRange(sfxRecord.minVolumeDb, sfxRecord.maxVolumeDb);

			float pitchScale = sfxRecord.minPitch == sfxRecord.maxPitch ?
				sfxRecord.minPitch :
				(float)GD.RandRange(sfxRecord.minPitch, sfxRecord.maxPitch);

			AudioStreamPlayer2D sfxPlayer = instance.PlayAudio(audioStream, audioPosition, volumeDb, pitchScale, false, 0f);
			sfxPlayer.AddToGroup(SFX_PLAYERS_GROUP_NAME);
			return sfxPlayer;
		}

		public AudioStreamPlayer PlayAudio(AudioStream audioStream, float volumeDb, float pitchScale, bool fadeIn, float fromPosition)
		{
			var audioPlayer = new AudioStreamPlayer()
			{
				Stream = audioStream,
				VolumeDb = volumeDb,
				PitchScale = pitchScale
			};

			AddChild(audioPlayer);
			audioPlayer.Connect("finished", audioPlayer, "queue_free");

			if (fadeIn)
			{
				_tween.InterpolateProperty(audioPlayer, "volume_db",
					FADE_IN_START_VOLUME, volumeDb, FADE_IN_DURATION);
				_tween.Start();
			}

			audioPlayer.Play(fromPosition);

			return audioPlayer;
		}

		public AudioStreamPlayer2D PlayAudio(AudioStream audioStream, Vector2 audioPosition, float volumeDb, float pitchScale, bool fadeIn, float fromPosition)
		{
			var audioPlayer = new AudioStreamPlayer2D()
			{
				Position = audioPosition,
				Stream = audioStream,
				VolumeDb = volumeDb,
				PitchScale = pitchScale
			};

			AddChild(audioPlayer);
			audioPlayer.Connect("finished", audioPlayer, "queue_free");

			if (fadeIn)
			{
				_tween.InterpolateProperty(audioPlayer, "volume_db",
					FADE_IN_START_VOLUME, volumeDb, FADE_IN_DURATION);
				_tween.Start();
			}

			audioPlayer.Play(fromPosition);

			return audioPlayer;
		}

		public static Array GetMusicPlayers()
		{
			return instance.GetTree().GetNodesInGroup(MUSIC_PLAYERS_GROUP_NAME);
		}

		public static Array GetSFXPlayers()
		{
			return instance.GetTree().GetNodesInGroup(SFX_PLAYERS_GROUP_NAME);
		}

		public static void StopAllMusic()
		{
			foreach (Node musicPlayer in GetMusicPlayers())
			{
				(musicPlayer as AudioStreamPlayer)?.Stop();
				(musicPlayer as AudioStreamPlayer2D)?.Stop();
				musicPlayer.QueueFree();
			}
		}

		public static void StopAllSFX()
		{
			foreach (Node sfxPlayer in GetSFXPlayers())
			{
				(sfxPlayer as AudioStreamPlayer)?.Stop();
				(sfxPlayer as AudioStreamPlayer2D)?.Stop();
				sfxPlayer.QueueFree();
			}
		}

		public static void StopAllAudio()
		{
			StopAllMusic();
			StopAllSFX();
		}
	}
}