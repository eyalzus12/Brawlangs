using Godot;
using System;
using System.Collections.Generic;

public class LegacyAttackCreatorPart2
{/*
	public string path = "C:\\Users\\eyalz\\Desktop\\godot\\Brawlangs\\NewTestAtt.jini";
	public JiniFile jf = new JiniFile();
	
	public AttackCreator()
	{
		jf.Load(path);
	}
	
	public AttackCreator(string path)
	{
		this.path = path;
		jf.Load(path);
	}
	
	public bool Build(Node2D n)
	{
		GD.Print(jf.ToString());
		
		BuildAttack(n);
		return true;
	}
	
	public bool BuildAttack(Node2D n)
	{
		Attack a = new Attack();
		n.AddChild(a);
		JiniSection sc;
		for(int i = 0; !((sc = jf.GetSection("Part"+i)) is null); ++i)
		{
			//GD.Print("h");
			BuildPart(a, sc);
		}
			
		for(int i = 0; !((sc = jf.GetSection("Part"+i)) is null); ++i)
		{
			JiniSection scc = sc.GetSection("Connections");
			if(!(scc is null))
			{
				var h = scc.PropertyNames;
				foreach(var s in h)
				{
					AttackPart ap = ((AttackPart)a.GetNode(scc[s, ""].s()));
					((AttackPart)a.GetNode("Part"+i)).Connect(s, ap);
					//GD.Print(ap);
				}
			}
		}
		
		a.start = (AttackPart)a.GetNode("Part0");
		a.currentPart = a.start;
		
		return true;
	}
	
	public bool BuildPart(Node2D n, JiniSection section)
	{
		//GD.Print("h");
		var ap = new AttackPart();
		var su = section.GetProperty("Startup", 0).i();
		ap.startup = su;
		var ln = section.GetProperty("Length", 0).i();
		ap.length = ln;
		var el = section.GetProperty("EndLag", 0).i();
		ap.endlag = el;
		ap.Name = section.name;
		n.AddChild(ap);
		
		JiniSection sc;
		for(int i = 0; !((sc = section.GetSection(section.name+"Hitbox"+i)) is null); ++i)
			BuildHitbox(ap, sc);
		ap.Init();
		ap.BuildHitboxAnimator();
			
		return true;
	}
	
	/*public bool BuildFrame(Node2D n, string part, string section)
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
	}*/
	
	/*public bool BuildHitbox(Node2D n, JiniSection section)
	{
		var h = new Hitbox();
		//GD.Print(section.ToString());
		var sk = section.GetProperty("SetKnockback", Vector2.Zero).v();
		h.setKnockback = sk;
		var vk = section.GetProperty("VarKnockback", Vector2.Zero).v();
		h.varKnockback = vk;
		var st = section.GetProperty("Stun", 0).i();
		h.stun = st;
		var hp = section.GetProperty("HitPause", 0).i();
		h.hitpause = hp;
		var hl = section.GetProperty("HitLag", 0).i();
		h.hitlag = hl;
		var dm = section.GetProperty("Damage", 0f).f();
		h.damage = dm;
		h.Name = section.name;
		
		var af = section.GetProperty("ActiveFrames", new List<Vector2>());
		//GD.Print(af.GetType().Name, af);
		if(af is Vector2)
		{
			var afl = new List<Vector2> {af.v()};
			h.activeFrames = afl;
		}
		else h.activeFrames = af.lv();
		n.AddChild(h);
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = section.GetProperty("Position", Vector2.Zero).v();
		cs.Position = pos;
		var rot = section.GetProperty("Rotation", 0f).f();
		cs.Rotation = rot;
		
		var ps = new CapsuleShape2D();
		
		var rd = section.GetProperty("Radius", 16f).f();
		ps.Radius = rd;
		var hg = section.GetProperty("Height", 16f).f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section.name + "Collision";
		h.AddChild(cs);
		h.shape = cs;
		return true;
	}*/
}
