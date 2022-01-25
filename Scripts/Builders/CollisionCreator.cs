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
		cs.Shape = new RectangleShape2D();
		cs.Name = "Collision";
		
		var HurtboxScript = inif["", "HurtboxScript", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var hr = TypeUtils.LoadScript<Hurtbox>(HurtboxScript, new Hurtbox(), baseFolder);
		hr.Name = "Hurtbox";
		hr.owner = ch;
		
		//////////////////////////////////////////////////////////////////////////////////
		
		var oBase = inif["", "Bases", new List<string>()];
		if(oBase is string) BuildBase(ch, oBase.s());
		else
		{
			var Bases = oBase.ls();
			foreach(var s in Bases) BuildBase(ch, s);
		}
		
		//////////////////////////////////////////////////////////////////////////////////
		
		var platDrop = new Area2D();
		platDrop.Name = "PlatformDrop";
		
		platDrop.Connect("body_exited", ch, "OnSemiSolidLeave");
		platDrop.CollisionLayer = 0;
		platDrop.CollisionMask = 0b10;
		
		var droppos = inif["", "PlatDropPosition", new Vector2(0, 19)].v2();
		platDrop.Position = droppos;
		
		var dropc = new CollisionShape2D();
		dropc.Shape = new RectangleShape2D();
		dropc.Name = "DropCollision";
		var dropext = inif["", "PlatDropExtents", new Vector2(32, 11)].v2();
		(dropc.Shape as RectangleShape2D).Extents = dropext;
		
		platDrop.Visible = false;
		
		//////////////////////////////////////////////////////////////////////////////////
		
		platDrop.AddChild(dropc);
		ch.AddChild(platDrop);
		ch.hurtbox = hr;
		ch.AddChild(hr);
		ch.collision = cs;
		ch.AddChild(cs);
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
		ch.settings.Add(section, new CollisionSettings(section, xt, pos, rd, he, hpos, rot));
	}
}
