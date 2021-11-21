using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class CharacterCreator
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
		n.AddChild(ch);
		
		ch.CollisionLayer = 0b100;
		ch.CollisionMask = 0b011;
		
		var directoryPath = path.SplitByLast('/')[0];
		//find stat file name
		var statFile = charinif["Stats", ""].s();
		//load properties from stat file
		var prop = new PropertyMap();
		prop.ReadFromConfigFile($"{directoryPath}/{statFile}.cfg", "Stats");
		prop.LoadProperties(ch);
		
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
		
		//find animation folder name
		var animationsFolder = charinif["Animations", ""].s();
		//create animations
		BuildAnimations(ch, $"{directoryPath}/{animationsFolder}");
		
		ch.teamNumber = teamNum;
		return ch;
	}
	
	public void BuildAnimations(Character ch, string animationsFolder)
	{
		var spr = new AnimationSprite();
		spr.Name = "Sprite";
		
		var files = ListPostImportDirectoryFiles(animationsFolder).Distinct().ToList();
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
				GD.Print($"failed to load animation {animationName} from path {resourcePath}");
				continue;
			}
			spr.AddSheet(texture, animationName, frames, loop);
		}
		
		ch.AddChild(spr);
		spr.InitFramePlayer();
		ch.sprite = spr;
		spr.Play("Default");
	}
	
	public Texture GenerateTextureFromPath(string path)
	{
		if(path.StartsWith("res://"))//local
		{
			return ResourceLoader.Load<Texture>(path);
		}
		else//remote
		{
			var image = new Image();
			var err = image.Load(path);
			if(err != Error.Ok)
			{
				GD.Print($"failed to load animation sheet {path}. error is {err}");
				return null;
			}
		
			var texture = new ImageTexture();
			texture.CreateFromImage(image, 0b11);
			return texture;
		}
	}
	
	public static List<string> ListPostImportDirectoryFiles(string path)
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
			else if(file.EndsWith(".png"))
			{
				fileArray.Add(file);
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
			GD.Print($"Error opening folder {path}. error is {er}");
		}
		
		return files;
	}
}
