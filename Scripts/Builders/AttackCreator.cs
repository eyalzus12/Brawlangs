using Godot;
using System;
using System.Collections.Generic;
using Conn = System.Collections.Generic.Dictionary<string, (string, Attack)>;

public class AttackCreator
{
	public string path;
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
		var AttackScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var a = TypeUtils.LoadScript<Attack>(AttackScript, new Attack(), baseFolder);
		n.AddChild(a);
		
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
		
		LoadExtraProperties(a, section);
	}
	
	public AttackPart BuildPart(Attack a, string section, string start)
	{
		var PartScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var ap = TypeUtils.LoadScript<AttackPart>(PartScript, new AttackPart(), baseFolder);
		
		ap.Name = section;
		a.AddChild(ap);
		if(section == start) a.start = ap;
		
		var su = inif[section, "Startup", 0].i();
		ap.startup = su;
		var ln = inif[section, "Length", 0].i();
		ap.length = ln;
		var el = inif[section, "Endlag", 0].i();
		ap.endlag = el;
		var cd = inif[section, "Cooldown", 0].i();
		ap.cooldown = cd;
		var mv = inif[section, "Movement", Vector2.Zero].v2();
		ap.movement = mv;
		var me = inif[section, "MissEndlag", 0].i();
		ap.missEndlag = me;
		var mc = inif[section, "MissCooldown", 0].i();
		ap.missCooldown = mc;
		var dm = inif[section, "DamageMult", 1f].f();
		ap.damageMult = dm;
		var km = inif[section, "KnockbackMult", 1f].f();
		ap.knockbackMult = km;
		var sm = inif[section, "StunMult", 1].i();
		ap.stunMult = sm;
		var sa = inif[section, "StartupAnimation", "Default"].s();
		ap.startupAnimation = sa;
		var aa = inif[section, "AttackAnimation", "Default"].s();
		ap.attackAnimation = aa;
		var ea = inif[section, "EndlagAnimation", "Default"].s();
		ap.endlagAnimation = ea;
		var ps = inif[section, "Sound", ""].s();
		ap.attackSound = ps;
		
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
		
		LoadExtraProperties(ap, section);
		
		return ap;
	}
	
	public void BuildHitbox(AttackPart ap, string section)
	{
		var HitboxSctipt = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxSctipt, new Hitbox(), baseFolder);
		
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
		var hs = inif[section, "HitSound", "DefaultHit"].s();
		h.hitSound = hs;
		h.Name = section;
		
		var af = inif[section, "ActiveFrames", new List<Vector2>()];
		if(af is Vector2) h.activeFrames = new List<Vector2> {af.v2()};
		else h.activeFrames = af.lv2();
		
		var hafs = inif[section, "HorizontalAngleFlipper", "Directional"].s();
		Hitbox.AngleFlipper haf;
		Enum.TryParse<Hitbox.AngleFlipper>(hafs, out haf);
		h.horizontalAngleFlipper = haf;
		var vafs = inif[section, "VerticalAngleFlipper", "None"].s();
		Hitbox.AngleFlipper vaf;
		Enum.TryParse<Hitbox.AngleFlipper>(vafs, out vaf);
		h.verticalAngleFlipper = vaf;
		
		var tkm = inif[section, "TeamKnockbackMultiplier", 1f].f();
		h.teamKnockbackMult = tkm;
		var tdm = inif[section, "TeamDamageMultiplier", 1f].f();
		h.teamDamageMult = tdm;
		var tsm = inif[section, "TeamStunMultiplier", 1].i();
		h.teamStunMult = tsm;
		
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
				var mult = entry.Value.i();
				h.stateStunMult.Add(stateName, (float)mult);
			}
		}
		
		LoadExtraProperties(h, section);
		
		ap.AddChild(h);
		ap.hitboxes.Add(h);
		h.ch = ap.ch;
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		cs.Rotation = (float)(rot*Math.PI/180f);//turn to rads
		
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
				//TODO: take from the attack's attack part dictionary, for safer connecting.
				AttackPart toConnect = (AttackPart)a.GetNode(connection.Value.s());
				//get requested to connect to
				ap.Connect(connection.Key, toConnect);
				//connect
			}
		}
	}
	
	/*
	the following is a quick fix for being unable to get the load request dictionary
	when using the Get method for godot objects, it refuses to be casted into the dictionary
	oh well. this works ok.
	*/
	
	public void LoadExtraProperties(Attack loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	public void LoadExtraProperties(AttackPart loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	public void LoadExtraProperties(Hitbox loadTo, string section) => LoadExtraProperties(loadTo, loadTo.LoadExtraProperties, section);
	
	public void LoadExtraProperties(Godot.Object loadTo, Dictionary<string, ParamRequest> load, string section)
	{
		foreach(var entry in load)
		{
			var ininame = entry.Key;
			var request = entry.Value;
			var type = request.ParamType;
			var objname = request.ParamName;
			var @default = request.ParamDefault;
			var prop = inif[section, ininame, @default].cast(type, $"loading extra properties for {loadTo.GetType().Name} {section}");
			loadTo.Set(objname, prop);
		}
	}
}
