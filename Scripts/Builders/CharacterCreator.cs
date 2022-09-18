using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class CharacterCreator
{
	public string path;
	
	public CharacterCreator() {}
	
	public CharacterCreator(string path)
	{
		this.path = path;
	}
	
	public Character Build(Node2D n, int teamNum)
	{
		//create actual character
		var ch = new Character();
		//open cfg file
		var charinif = new CfgFile();
		charinif.Load(path);
		var name = charinif["Name", ""].s();
		//load name
		ch.Name = name + teamNum;
		
		var projPool = new ProjectilePool(ch); 
		projPool.Name = "ProjectilePool";
		ch.AddChild(projPool);
		ch.projPool = projPool;
		
		
		ch.CollisionLayer = 0b100;
		ch.CollisionMask = 0b011;
		
		var directoryPath = path.SplitByLast('/')[0];
		//find stat file name
		var statFile = charinif["Stats", ""].s();
		//load properties from stat file
		var prop = new PropertyMap();
		prop.ReadFromConfigFile($"{directoryPath}/{statFile}.cfg", "Stats");
		prop.LoadProperties(ch);
		
		//find animation folder name
		var animationsFolder = charinif["Animations", ""].s();
		//create animations
		BuildAnimations(ch, $"{directoryPath}/{animationsFolder}");
		
		//find audio folder name
		var audioFolder = charinif["Audio", ""].s();
		//load sounds
		BuildAudio(ch, $"{directoryPath}/{audioFolder}");
		
		//find collision file name
		var collisionFile = charinif["Collision", ""].s();
		//create collision
		var collCreator = new CollisionCreator($"{directoryPath}/{collisionFile}.ini");
		collCreator.Build(ch);
		
		//find attack file name
		var attackFile = charinif["Attacks", ""].s();
		//create attacks
		var attCreator = new AttackCreator($"{directoryPath}/{attackFile}.ini");
		attCreator.Build(ch);
		
		//find projectiles file name
		var projFile = charinif["Projectiles", ""].s();
		//create attacks
		var projCreator = new ProjectileCreator($"{directoryPath}/{projFile}.ini");
		ch.projPool.ProjCreate = projCreator;
		/*var projectilesBuilt = projCreator.Build(ch);
		foreach(var sp in projectilesBuilt)
		{
			var packname = sp.Item1;
			var pack = sp.Item2;
			ch.projectiles.Add(packname, pack);
		}*/
		
		ch.TeamNumber = teamNum;
		n.AddChild(ch);
		return ch;
	}
	
	public void BuildAnimations(Character ch, string animationsFolder)
	{
		var spr = new AnimationSprite();
		spr.Name = "CharacterSprite";
		
		var files = ListPostImportDirectoryFiles(animationsFolder, ".png").Distinct().ToList();
		foreach(var file in files)
		{
			var filename = StringUtils.RemoveExtension(file);
			var resourcePath = $"{animationsFolder}/{file}";
			var parts = filename.Split('_');
			var animationName = parts[0];
			var frames = int.Parse(parts[1]);
			var loop = int.Parse(parts[2]).b();
			var texture = GenerateTextureFromPath(resourcePath);
			if(texture is null)
			{
				GD.PushError($"failed to load animation {animationName} from path {resourcePath}");
				continue;
			}
			spr.Add(texture, animationName, frames, loop);
		}
		
		ch.CharacterSprite = spr;
		ch.AddChild(spr);
	}
	
	public Texture2D GenerateTextureFromPath(string path)
	{
		if(path.StartsWith("res://"))//local
		{
			return ResourceLoader.Load<Texture2D>(path);
		}
		else//remote
		{
			var image = new Image();
			var err = image.Load(path);
			if(err != Error.Ok)
			{
				GD.PushError($"failed to load animation sheet {path}. error is {err}");
				return null;
			}
		
			return ImageTexture.CreateFromImage(image);
		}
	}
	
	const int AUDIO_PLAYERS = 4;
	public void BuildAudio(Character ch, string audioFolder)
	{
		var am = new AudioManager(AUDIO_PLAYERS);
		am.Name = "AudioManager";
		
		var files = ListPostImportDirectoryFiles(audioFolder, ".wav", ".ogg", ".mp3").Distinct().ToList();
		foreach(var file in files)
		{
			var filename = StringUtils.RemoveExtension(file);
			var resourcePath = $"{audioFolder}/{file}";
			var loop = filename.EndsWith("Loop");
			var audio = GenerateAudioFromPath(resourcePath, loop);
			var noloopname = filename.Substring(0, filename.Length - "Loop".Length);
			var normalizedFilename = (loop?noloopname:filename);
			am.AddSound(normalizedFilename, audio);
		}
		
		ch.AddChild(am);
		ch.Audio = am;
	}
	
	public AudioStream GenerateAudioFromPath(string path, bool loop = false)
	{
		var audio = ResourceLoader.Load<AudioStream>(path);
		if(audio is null)
		{
			GD.PushError($"Failed to load sound file from path {path}");
			return null;
		}
		
		if(loop)
		{
			if(audio is AudioStreamOggVorbis ogg)
			{
				ogg.Loop = true;
				return ogg;
			}
			else if(audio is AudioStreamMP3 mp3)
			{
				mp3.Loop = true;
				return mp3;
			}
			else if(audio is AudioStreamWAV wav)
			{
				GD.PushWarning("WAV does not support dynamic looping. Use ogg instead.");
				//wav.LoopMode = AudioStreamWAV.LoopModeEnum.Forward;
				return wav;
			}
			else
			{
				GD.PushError($"Cannot loop audio file {path} since it isn't MP3, OGG, or WAV.");
				return audio;
			}
		}
		else return audio;
	}
	
	public static List<string> ListPostImportDirectoryFiles(string path, params string[] desiredExtensions)
	{
		var tempArray = ListDirectoryFiles(path);
		var fileArray = new List<string>();
		foreach(var file in tempArray)
		{
			if(file.EndsWith(".import"))
			{
				var filename = file.Replace(".import", "");
				fileArray.Add(filename);
			}
			else
			{
				foreach(var ext in desiredExtensions)
				if(file.EndsWith(ext))
				{
					fileArray.Add(file);
					break;
				}
			}
		}
		
		return fileArray;
	}
	
	public static List<string> ListDirectoryFiles(string path)
	{
		var files = new List<string>();
		var dir = new Directory();
		var er = dir.Open(path);
		if(er == Error.Ok)
		{
			dir.ListDirBegin();
			string file;
			
			while((file = dir.GetNext()) != "")
			{
				if(!dir.CurrentIsDir() && file[0] != '.')
				{
					files.Add(file);
				}
			}
			
			dir.ListDirEnd();
		}
		else
		{
			GD.PushError($"Error opening folder {path}. error is {er}");
		}
		
		return files;
	}
}
