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
		//open ini file
		var charinif = new CfgFile();
		charinif.Load(path);
		var name = charinif["Name", ""].s();
		//load name
		ch.Name = name;
		n.AddChild(ch);
		
		ch.CollisionLayer = 0b100;
		ch.CollisionMask = 0b011;
		
		/*var oCanHitSlots = charinif[BASE_SECTION, "CanHit", new List<int>()];
		if(oCanHitSlots is int)
		{
			ch.CollisionLayer = 0;
			ch.SetCollisionLayerBit(oCanHitSlots.i() + 2)
		}
		else
		{
			
		}*/
		
		//find stat file path
		var statPath = charinif["Stats", ""].s();
		//load properties from stat file
		var prop = new PropertyMap();
		prop.ReadFromConfigFile(statPath, "Stats");
		prop.LoadProperties(ch);
		
		//find collision file path
		var collisionPath = charinif["Collision", ""].s();
		//create collision
		var collCreator = new CollisionCreator(collisionPath);
		collCreator.Build(ch);
		
		//find attack file path
		var attackPath = charinif["Attacks", ""].s();
		//create attacks
		var attCreator = new AttackCreator(attackPath);
		attCreator.Build(ch);
		
		/*var cl = new CanvasLayer();
		cl.Name = "UI";
		ch.AddChild(cl);
		var lb = new DebugLabel();
		cl.AddChild(lb);
		var dl = new DamageLabel();
		dl.ch = ch;
		dl.MarginLeft = 1000;
		dl.MarginTop = 200;
		cl.AddChild(dl);*/
		
		var animationPath = charinif["Animations", ""].s();
		BuildAnimations(ch, animationPath);
		return ch;
	}
	
	public void BuildAnimations(Character ch, string animationPath)
	{
		var spr = new AnimationSprite();
		spr.Name = "Sprite";
		
		var anminif = new IniFile();
		anminif.Load(animationPath);
		//GD.Print(anminif.ToString());
		foreach(var section in anminif.Keys)
		{
			if(section == "") continue;
			var resourcePath = anminif[section, "Path", ""].s();
			var frames = anminif[section, "Frames", new Vector2(4,4)].v2();
			var loop = anminif[section, "Loop", true].b();
			var texture = ResourceLoader.Load<Texture>(resourcePath);
			spr.AddSheet(texture, section, frames.x.i(), frames.y.i(), loop);
		}
		
		ch.AddChild(spr);
		spr.InitFramePlayer();
		ch.sprite = spr;
		spr.Play("Default");
	}
}
