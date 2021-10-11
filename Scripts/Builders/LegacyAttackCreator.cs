using Godot;
using System;
using System.Collections.Generic;

public class LegacyAttackCreator
{
	/*public string path = "C:\\Users\\eyalz\\Desktop\\godot\\Brawlangs\\TestAtt.ini";
	public MapFile mf = new MapFile();
	
	public AttackCreator()
	{
		mf.Load(path);
	}
	
	public AttackCreator(string path)
	{
		this.path = path;
		mf.Load(path);
	}
	
	public bool Build(Node2D n)
	{
		GD.Print(mf.ToString());
		
		for(int i = 0; mf.HasSection("Part"+i); ++i)
			BuildPart(n, "Part"+i);
		
		return true;
	}
	
	public bool BuildPart(Node2D n, string section)
	{
		var ap = new AttackPart();
		
		var su = mf[section, "Startup", 0].i();
		ap.startup = su;
		ap.Name = section;
		n.AddChild(ap);
		
		for(int i = 0; mf.HasSection(section+"Frame"+i); ++i)
			BuildFrame(ap, section+"Frame"+i);
			
		ap.Reload();
		//GD.Print(ap.frames.ToArray());
			
		return true;
	}
	
	public bool BuildFrame(Node2D n, string section)
	{
		var fr = new AttackFrame();
		
		var ln = mf[section, "Length", 1].i();
		fr.length = ln;
		fr.Name = section;
		n.AddChild(fr);
		
		for(int i = 0; mf.HasSection(section+"Hitbox"+i); ++i)
			BuildHitbox(fr, section+"Hitbox"+i);
		
		fr.Reload();
		//GD.Print(fr.hitboxes.ToArray());
		
		return true;
	}
	
	public bool BuildHitbox(Node2D n, string section)
	{
		section = section.Trim();
		var h = new Hitbox();
		
		var sk = mf[section, "SetKnockback", Vector2.Zero].v();
		h.setKnockback = sk;
		var vk = mf[section, "VarKnockback", Vector2.Zero].v();
		h.varKnockback = vk;
		var st = mf[section, "Stun", 0].i();
		h.stun = st;
		var hp = mf[section, "HitPause", 0].i();
		h.hitpause = hp;
		var hl = mf[section, "HitLag", 0].i();
		h.hitlag = hl;
		var dm = mf[section, "Damage", 0f].f();
		h.damage = dm;
		h.Name = section;
		n.AddChild(h);
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = mf[section, "Position", Vector2.Zero].v();
		cs.Position = pos;
		var rot = mf[section, "Rotation", 0f].f();
		cs.Rotation = rot;
		
		var ps = new CapsuleShape2D();
		
		var rd = mf[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = mf[section, "Height", 16f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.AddChild(cs);
		h.Reload();
		//GD.Print(h.shapes.ToArray());
		return true;
	}*/
}
