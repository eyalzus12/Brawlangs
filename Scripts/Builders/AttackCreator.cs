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
		a.ch = n as Character;
		a.Name = section;
		
		var fric = inif[section, "Friction", 1f].f();
		a.attackFriction = fric;
		
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
		
		a.LoadProperties();
		LoadExtraProperties(a, section);
		
		n.AddChild(a);
		a.Owner = n;//for scene packing
	}
	
	public AttackPart BuildPart(Attack a, string section, string start)
	{
		var PartScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var ap = TypeUtils.LoadScript<AttackPart>(PartScript, new AttackPart(), baseFolder);
		ap.att = a;
		ap.ch = a.ch;
		ap.Name = section;
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
		else if(oHitboxSections is object)//not null
		{
			var HitboxSections = oHitboxSections.ls();
			foreach(var s in HitboxSections) BuildHitbox(ap, s);
		}
		
		var ConnectionSection = inif[section, "Connections", ""].s();
		//get connection section
		if(ConnectionSection != "") cn.Add(section, (ConnectionSection, a));
		//request connection for later
		
		ap.LoadProperties();
		LoadExtraProperties(ap, section);
		
		a.AddChild(ap);
		ap.Owner = a;//for scene packing
		
		return ap;
	}
	
	public void BuildHitbox(Node2D ap, string section)
	{
		var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<CharacterHitbox>(HitboxScript, new CharacterHitbox(), baseFolder);
		h.Name = section;
		var ch = (Character)ap.Get("ch");
		h.owner = ch;
		
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
		try
		{
			var ahs = ch.audioManager.sounds[hs];
			h.hitSound = ahs;
		}
		catch(KeyNotFoundException)
		{
			GD.Print($"Hit sound {hs} for hitbox {section} in file at path {inif.filePath} could not be found.");
		}
		
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
		
		h.LoadProperties();
		LoadExtraProperties(h, section);
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		h.originalPosition = pos;
		var rot = inif[section, "Rotation", 0f].f();
		var rotrad = (float)(rot*Math.PI/180f);
		cs.Rotation = rotrad;//turn to rads
		h.originalRotation = rotrad;
		
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 16f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.shape = cs;
		h.AddChild(cs);
		cs.Owner = h;//for scene packing
		ap.AddChild(h);
		h.Owner = ap;//for scene packing
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
