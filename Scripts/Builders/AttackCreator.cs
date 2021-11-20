using Godot;
using System;
using System.Collections.Generic;
using Conn = System.Collections.Generic.Dictionary<string, (string, Attack)>;

public class AttackCreator
{
	public string path = "res://mario.ini";
	public IniFile inif = new IniFile();
	
	private Conn cn = new Conn();
	//part name - connection section name
	
	public AttackCreator()
	{
		cn = new Conn();
	}
	
	public AttackCreator(string path)
	{
		this.path = path;
		inif.Load(path);
		cn = new Conn();
	}
	
	public void Build(Node2D n)
	{
		cn = new Conn();
		var oAttacks = inif["", "AttackSections", new List<string>()];
		if(oAttacks is string)
			BuildAttack(n, oAttacks.s());
		else
		{
			var Attacks = oAttacks.ls();
			foreach(var s in Attacks)
				BuildAttack(n, s);
		}
		
		BuildConnections();
	}
	
	public void BuildAttack(Node n, string section)
	{
		//GD.Print(section, ", ", start);
		var AttackScript = inif[section, "Script", ""].s();
		//GD.Print($"Loading script {PartScript}");
		//var charge = inif[section, "Charge", false].b();
		Attack a;
		if(AttackScript != "")
		{
			var resource = ResourceLoader.Load(AttackScript);
			if(resource is null)
			{
				GD.Print($"Attempt to load script {AttackScript} failed because that file does not exist");
				a = new Attack();
			}
			var script = resource as CSharpScript;
			if(script is null)
			{
				GD.Print($"Attempt to load script {AttackScript} failed because the object in that path is not a C# script");
				a = new Attack();
			}
			else
			{
				a = script.New() as Attack;
				if(a is null)
				{
					GD.Print($"Attempt to attach script {AttackScript} failed because the object in that path is not an AttackPart script");
					a = new Attack();
				}
			}
		}
		else a = new Attack();
		n.AddChild(a);
		
		/*if(charge)
		{
			var chargepart = new ChargePart();
			a.AddChild(chargepart);
			chargepart.MaxCharge = inif[section, "MaxCharge", 60].i();
			var input = inif[section, "Input", "heavy"].s().ToLower();
			chargepart.input = $"player_{input}_attack";
			(a as ChargeableAttack).charge = chargepart;
		}*/
		
		var fric = inif[section, "Friction", 1f].f();
		a.attackFriction = fric;
		a.Name = section;
		var StartPartSection = inif[section, "StartPart", ""].s();
		
		object oPartSections = inif[section, "Parts", new List<string>()];
		if(oPartSections is string PartSection)
			BuildPart(a, PartSection, StartPartSection);
		else
		{
			var PartSections = oPartSections.ls();
			foreach(var s in PartSections)
				BuildPart(a, s, StartPartSection);
		}
	}
	
	public AttackPart BuildPart(Attack a, string section, string start)
	{
		//GD.Print(section, ", ", start);
		var PartScript = inif[section, "Script", ""].s();
		//GD.Print($"Loading script {PartScript}");
		AttackPart ap;
		if(PartScript != "")
		{
			var resource = ResourceLoader.Load(PartScript);
			if(resource is null)
			{
				GD.Print($"Attempt to load script {PartScript} failed because that file does not exist");
				ap = new AttackPart();
			}
			var script = resource as CSharpScript;
			if(script is null)
			{
				GD.Print($"Attempt to load script {PartScript} failed because the object in that path is not a C# script");
				ap = new AttackPart();
			}
			else
			{
				ap = script.New() as AttackPart;
				if(ap is null)
				{
					GD.Print($"Attempt to attach script {PartScript} failed because the object in that path is not an AttackPart script");
					ap = new AttackPart();
				}
			}
		}
		else ap = new AttackPart();
		
		ap.Name = section;
		a.AddChild(ap);
		if(section == start) a.start = ap;
		
		var su = inif[section, "Startup", 0].i();
		ap.startup = su;
		var ln = inif[section, "Length", 0].i();
		ap.length = ln;
		var el = inif[section, "Endlag", 0].i();
		ap.endlag = el;
		var mv = inif[section, "Movement", Vector2.Zero].v2();
		ap.movement = mv;
		var me = inif[section, "MissEndlag", 0].i();
		ap.missEndlag = me;
		
		var oHitboxSections = inif[section, "Hitboxes", null];
		if(oHitboxSections is string)
			BuildHitbox(ap, oHitboxSections.s());
		else if(oHitboxSections.NotNull())
		{
			var HitboxSections = oHitboxSections.ls();
			foreach(var s in HitboxSections) BuildHitbox(ap, s);
		}
		
		var ConnectionSection = inif[section, "Connections", ""].s();
		//get connection section
		if(ConnectionSection != "") cn.Add(section, (ConnectionSection, a));
		//request connection for later
		ap.ConnectSignals();
		ap.BuildHitboxAnimator();
		
		var load = ap.LoadExtraProperties;
		foreach(var s in load.Keys)
		{
			var prop = inif[section, s, null].cast(load[s], $"loading extra properties for part {section}");
			ap.Set/*Deferred*/(s, prop);
		}
		
		return ap;
	}
	
	public void BuildHitbox(AttackPart ap, string section)
	{
		var HitboxSctipt = inif[section, "Script", ""].s();
		Hitbox h;
		if(HitboxSctipt != "")
		{
			var resource = ResourceLoader.Load(HitboxSctipt);
			if(resource is null)
			{
				GD.Print($"Attempt to load script {HitboxSctipt} failed because that file does not exist");
				h = new Hitbox();
			}
			var script = resource as CSharpScript;
			if(script is null)
			{
				GD.Print($"Attempt to load script {HitboxSctipt} failed because the object in that path is not a C# script");
				h = new Hitbox();
			}
			else
			{
				h = script.New() as Hitbox;
				if(h is null)
				{
					GD.Print($"Attempt to attach script {HitboxSctipt} failed because the object in that path is not a Hitbox script");
					h = new Hitbox();
				}
			}
		}
		else h = new Hitbox();
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.setKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.varKnockback = vk;
		var st = inif[section, "Stun", 0].i();
		h.stun = st;
		var hp = inif[section, "HitPause", 0].i();
		h.hitpause = hp;
		var hl = inif[section, "HitLag", 0].i();
		h.hitlag = hl;
		var dm = inif[section, "Damage", 0f].f();
		h.damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.hitPriority = pr;
		var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		h.momentumCarry = cm;
		h.Name = section;
		
		var af = inif[section, "ActiveFrames", new List<Vector2>()];
		if(af is Vector2) h.activeFrames = new List<Vector2> {af.v2()};
		else h.activeFrames = af.lv2();
		
		var kt = inif[section, "KnockbackType", "Directional"].s();
		Hitbox.KnockbackSetting ks;
		Enum.TryParse<Hitbox.KnockbackSetting>(kt, out ks);
		h.knockbackSetting = ks;
		
		var kmu = inif[section, "KnockbackMultiplier", new Vector3(0,1,1)].v3();
		h.knockbackMult = kmu;
		var dmu = inif[section, "DamageMultiplier", new Vector3(0,1,1)].v3();
		h.damageMult = dmu;
		var smu = inif[section, "StunMultiplier", new Vector3(0,1,1)].v3();
		h.stunMult = smu;
		
		var kms = inif[section, "KnockbackStateMultipliers", ""].s();
		if(kms != "")
		{
			h.stateKnockbackMult = new Dictionary<string, float>();
			foreach(var entry in inif[kms])
			{
				var stateName = entry.Key;
				var mult = entry.Value.f();
				h.stateKnockbackMult.Add(stateName, mult);
			}
		}
		
		var dms = inif[section, "DamageStateMultipliers", ""].s();
		if(dms != "")
		{
			h.stateDamageMult = new Dictionary<string, float>();
			foreach(var entry in inif[dms])
			{
				var stateName = entry.Key;
				var mult = entry.Value.f();
				h.stateDamageMult.Add(stateName, mult);
			}
		}
		
		var sms = inif[section, "StunStateMultipliers", ""].s();
		if(sms != "")
		{
			h.stateStunMult = new Dictionary<string, float>();
			foreach(var entry in inif[sms])
			{
				var stateName = entry.Key;
				var mult = entry.Value.f();
				h.stateStunMult.Add(stateName, mult);
			}
		}
		
		ap.AddChild(h);
		ap.hitboxes.Add(h);
		h.ch = ap.ch;
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		cs.Rotation = (float)(rot*Math.PI/180f);
		
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 16f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.AddChild(cs);
		h.Reload();
	}
	
	public void BuildConnections()
	{
		foreach(var entry in cn)//go over requests
		{
			Attack a = entry.Value.Item2;
			AttackPart ap = (AttackPart)a.GetNode(entry.Key);//get requester
			var conndict = inif.dict[entry.Value.Item1];//get dictionary of connection
			foreach(var connection in conndict)//go over it
			{
				AttackPart toConnect = (AttackPart)a.GetNode(connection.Value.s());
				//get requested to connect to
				ap.Connect(connection.Key, toConnect);
				//connect
			}
		}
	}
}
