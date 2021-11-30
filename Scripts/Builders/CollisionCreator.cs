using Godot;
using System;
using System.Collections.Generic;

public class CollisionCreator
{
	public const string BrawlangsServer_JoinOrBad = "https://discord.gg/ZaGfdm3bad";
	
	public IniFile inif = new IniFile();
	public string path;
	
	public CollisionCreator() {}
	
	public CollisionCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public void Build(Character ch)
	{
		var cs = new CharacterCollision();
		ch.AddChild(cs);
		cs.Shape = new RectangleShape2D();
		cs.Name = "Collision";
		
		var HurtboxScript = inif["", "HurtboxScript", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var hr = TypeUtils.LoadScript<Hurtbox>(HurtboxScript, new Hurtbox(), "Hurtbox", baseFolder);
		ch.AddChild(hr);
		hr.Name = "Hurtbox";
		hr.CreateCollision();
		
		ch.collision = cs;
		ch.hurtbox = hr;
		
		var oBase = inif["", "Bases", new List<string>()];
		if(oBase is string) BuildBase(ch, oBase.s());
		else
		{
			var Bases = oBase.ls();
			foreach(var s in Bases) BuildBase(ch, s);
		}
		
		var platDrop = new Area2D();
		platDrop.Name = "PlatformDrop";
		ch.AddChild(platDrop);
		platDrop.Connect("body_exited", ch, "OnSemiSolidLeave");
		platDrop.CollisionLayer = 0;
		platDrop.CollisionMask = 0b10;
		var droppos = inif["", "PlatDropPosition", new Vector2(0, 19)].v2();
		platDrop.Position = droppos;
		var dropc = new CollisionShape2D();
		platDrop.AddChild(dropc);
		dropc.Shape = new RectangleShape2D();
		dropc.Name = "DropCollision";
		var dropext = inif["", "PlatDropExtents", new Vector2(32, 11)].v2();
		(dropc.Shape as RectangleShape2D).Extents = dropext;
		
		platDrop.Visible = false;
	}
	
	public void BuildBase(Character ch, string section)
	{
		var pos = inif[section, "CollisionPosition", Vector2.Zero].v2();
		var xt = inif[section, "CollisionExtents", new Vector2(16f, 16f)].v2();
		var rd = inif[section, "HurtboxRadius", 30].f();
		var he = inif[section, "HurtboxHeight", 16].f();
		var hpos = inif[section, "HurtboxPosition", Vector2.Zero].v2();
		var rot = inif[section, "HurtboxRotation", 0f].f();
		rot = (float)(rot*Math.PI/180f);//to rads
		
		/*
		byte mask = 0;
		
		for(int i = 8; i >= 1; --i)
		{
			//GD.Print($"Fetching bit {i}");
			var bit = inif[section, i.ToString(), false].b();
			//GD.Print($"Current bit: {bit}");
			var b = Convert.ToByte(!bit);
			//flip and convert to byte
			//why you ask?
			//refer to this: https://discord.com/channels/789165471311462471/800276377387532328/884433912758149182
			//if you cant access it, you're not in the brawlangs discord server
			//so join it
			//if the server doesnt exist anymore... L
			//oh right the invite is https://discord.gg/ZaGfdm3bad
			//GD.Print($"As byte: {b}");
			mask |= b;
			mask <<= 1;
			//GD.Print($"Current layer: {mask}");
		}
		
		mask >>= 1;//undo last one
		UInt32 actualMask = (UInt32)mask;
		//GD.Print($"Full Layer: {actualMask}");
		actualMask <<= 3;//shift to match
		//GD.Print($"Stored Layer: {actualMask}");
		*/
		ch.settings.Add(section, new CollisionSettings(section, xt, pos, rd, he, hpos, rot/*, actualMask*/));
	}
}
