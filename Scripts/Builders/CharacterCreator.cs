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
	
	public Character Build(Node2D n)
	{
		//create actual character
		var ch = new Character();
		//open cfg file
		var charinif = new CfgFile();
		charinif.Load(path);
		var name = charinif["Name", ""].s();
		//load name
		ch.Name = name;
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
		return ch;
	}
	
	public void BuildAnimations(Character ch, string animationsFolder)
	{
		var spr = new AnimationSprite();
		spr.Name = "Sprite";
		
		var dir = new Directory();
		
		if(dir.Open(animationsFolder) == Error.Ok)
		{
			dir.ListDirBegin(true);
			string file;
			
			while((file = dir.GetNext()) != "")
			{
				if(!dir.CurrentIsDir() && StringUtils.GetExtension(file) == ".png")
				{
					var filename = StringUtils.RemoveExtension(file);
					var resourcePath = $"{animationsFolder}/{filename}.png";
					var parts = filename.Split('_');
					var animationName = parts[0];
					var frames = int.Parse(parts[1]);
					var loop = int.Parse(parts[2]).b();
					var texture = GenerateTextureFromPath(resourcePath);
					if(texture is null) continue;
					spr.AddSheet(texture, animationName, frames, loop);
				}
			}
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
		else
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
}
