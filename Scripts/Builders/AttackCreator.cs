using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Conn = System.Collections.Generic.Dictionary<string, (string, Attack)>;

public class AttackCreator
{
	public string path;
	public IniFile inif = new IniFile();
	
	public AttackCreator() {}
	
	public AttackCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public void Build(Node2D n)
	{
		var attacks = inif["", "AttackSections", Enumerable.Empty<string>()].ls();
		foreach(var s in attacks) BuildAttack(n, s);
	}
	
	public void BuildAttack(Node n, string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate attack {section} as it is not a real section"); return;}
		var AttackScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var a = TypeUtils.LoadScript<Attack>(AttackScript, new Attack(), baseFolder);
		var ch = n as Character;
		a.OwnerObject = ch;
		a.Name = section;
		
		var fric = inif[section, "Friction", 1f].f();
		a.AttackFriction = fric;
		
		var sharesCooldownWith = inif[section, "SharesCooldownWith", Enumerable.Empty<string>()].ls();
		a.SharesCooldownWith.AddRange(sharesCooldownWith);
		
		var startPartSection = inif[section, "StartPart", ""].s();
		var partSections = inif[section, "Parts", Enumerable.Empty<string>()].ls();
		foreach(var s in partSections) BuildPart(a, s, startPartSection);
		
		a.LoadProperties();
		LoadExtraProperties(a, section);
		
		n.AddChild(a);
		a.Owner = n;//for scene packing
		ch.Attacks.Add(a.Name, a);
	}
	
	public AttackPart BuildPart(Attack a, string section, string start)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate attack part {section} as it is not a real section"); return null;}
		var PartScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var ap = TypeUtils.LoadScript<AttackPart>(PartScript, new AttackPart(), baseFolder);
		ap.att = a;
		ap.OwnerObject = a.OwnerObject;
		ap.Name = section;
		a.Parts.Add(section, ap);
		if(section == start) a.StartPart = ap;
		
		var su = inif[section, "Startup", 0].i();
		ap.Startup = su;
		var ln = inif[section, "Length", 0].i();
		ap.Length = ln;
		var cd = inif[section, "Cooldown", 0].i();
		ap.Cooldown = cd;
		var mv = inif[section, "Movement", Vector2.Zero].v2();
		ap.Movement = mv;
		var ox = inif[section, "OverwriteXMovement", false].b();
		ap.OverwriteXMovement = ox;
		var oy = inif[section, "OverwriteYMovement", false].b();
		ap.OverwriteYMovement = oy;
		var dfa = inif[section, "DriftForwardAcceleration", 0f].f();
		ap.DriftForwardAcceleration = dfa;
		var dfs = inif[section, "DriftForwardSpeed", 0f].f();
		ap.DriftForwardSpeed = dfs;
		var dba = inif[section, "DriftBackwardsAcceleration", 0f].f();
		ap.DriftBackwardsAcceleration = dba;
		var dbs = inif[section, "DriftBackwardsSpeed", 0f].f();
		ap.DriftBackwardsSpeed = dbs;
		var ws = inif[section, "SlowOnWalls", true].b();
		ap.SlowOnWalls = ws;
		var ffl = inif[section, "FastFallLocked", false].b();
		ap.FastFallLocked = ffl;
		var mc = inif[section, "MissCooldown", 0].i();
		ap.MissCooldown = mc;
		var gm = inif[section, "GravityMultiplier", 1f].f();
		ap.GravityMultiplier = gm;
		var dm = inif[section, "DamageDoneMult", 1f].f();
		ap.DamageDoneMult = dm;
		var km = inif[section, "KnockbackDoneMult", 1f].f();
		ap.KnockbackDoneMult = km;
		var sm = inif[section, "StunDoneMult", 1f].f();
		ap.StunDoneMult = sm;
		var aa = inif[section, "Animation", ""].s();
		ap.AttackAnimation = aa;
		var ps = inif[section, "Sound", ""].s();
		ap.AttackSound = ps;
		
		var ep = inif[section, "EmittedProjectiles", Enumerable.Empty<string>()].ls();
		ap.EmittedProjectiles = ep;
		
		var hitboxSections = inif[section, "Hitboxes", Enumerable.Empty<string>()].ls();
		foreach(var s in hitboxSections) BuildHitbox(ap, s);
		
		var transitionSections = inif[section, "Transitions", Enumerable.Empty<string>()].ls();
		foreach(var s in transitionSections) BuildTransition(ap, s);
		
		var hitTransitionSection = inif[section, "HitTransition", ""].s();
		if(hitTransitionSection != "") ap.TransitionManager.Add(new AttackPartTransition(Enumerable.Empty<Vector2>(), new AttackPartTransitionTagExpression(new object[]{("Hit",StateTag.Active),("Hit",StateTag.Starting),'|'}), hitTransitionSection));
		
		var missTransitionSection = inif[section, "MissTransition", ""].s();
		if(missTransitionSection != "") ap.TransitionManager.Add(new AttackPartTransition(Enumerable.Empty<Vector2>(), new AttackPartTransitionTagExpression(new object[]{("Hit",StateTag.NotActive)}), missTransitionSection));
		
		var nextTransitionSection = inif[section, "NextTransition", ""].s();
		if(nextTransitionSection != "") ap.TransitionManager.Add(new AttackPartTransition(Enumerable.Empty<Vector2>(), new AttackPartTransitionTagExpression(), nextTransitionSection));
		
		ap.LoadProperties();
		LoadExtraProperties(ap, section);
		
		a.AddChild(ap);
		ap.Owner = a;//for scene packing
		
		return ap;
	}
	
	public void BuildHitbox(AttackPart ap, string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate hitbox {section} as it is not a real section"); return;}
		var HitboxScript = inif[section, "Script", ""].s();
		var baseFolder = path.SplitByLast('/')[0];
		var h = TypeUtils.LoadScript<Hitbox>(HitboxScript, new Hitbox(), baseFolder);
		h.Name = section;
		h.OwnerObject = ap;
		
		var sk = inif[section, "SetKnockback", Vector2.Zero].v2();
		h.SetKnockback = sk;
		var vk = inif[section, "VarKnockback", Vector2.Zero].v2();
		h.VarKnockback = vk;
		var sst = inif[section, "Stun", 0f].f();
		h.SetStun = sst;
		var vst = inif[section, "VarStun", 0f].f();
		h.VarStun = vst;
		var hl = inif[section, "HitLag", 0].f();
		h.HitLag = hl;
		var hp = inif[section, "ExtraOpponentHitLag", 0].f();
		h.SetHitPause = hl+hp;
		var ehp = inif[section, "ExtraOpponentVarHitLag", 0].f();
		h.VarHitPause = ehp;
		var dm = inif[section, "Damage", 0f].f();
		h.Damage = dm;
		var pr = inif[section, "Priority", 0].i();
		h.HitPriority = pr;
		var cm = inif[section, "MomentumCarry", Vector2.Zero].v2();
		h.MomentumCarry = cm;
		var hs = inif[section, "HitSound", "DefaultHit"].s();
		if(ap.OwnerObject.Audio.ContainsSound(hs)) h.HitSound = ap.OwnerObject.Audio[hs];
		else GD.PushError($"Hit sound {hs} for hitbox {section} in file at path {inif.filePath} could not be found.");
		
		var af = inif[section, "ActiveFrames", Enumerable.Empty<Vector2>()].lv2();
		h.ActiveFrames = af;
		
		var hafs = inif[section, "HorizontalAngleFlipper", "Directional"].s();
		Hitbox.AngleFlipper haf;
		Enum.TryParse<Hitbox.AngleFlipper>(hafs, out haf);
		h.HorizontalAngleFlipper = haf;
		var vafs = inif[section, "VerticalAngleFlipper", "None"].s();
		Hitbox.AngleFlipper vaf;
		Enum.TryParse<Hitbox.AngleFlipper>(vafs, out vaf);
		h.VerticalAngleFlipper = vaf;
		
		var shcs = inif[section, "SelfHitCondition", "DecideByFriendlyFire"].s();
		HitCondition shc;
		Enum.TryParse<HitCondition>(shcs, out shc);
		h.SelfHitCondition = shc;
		var thcs = inif[section, "TeamHitCondition", "DecideByFriendlyFire"].s();
		HitCondition thc;
		Enum.TryParse<HitCondition>(thcs, out thc);
		h.TeamHitCondition = thc;
		
		var tkm = inif[section, "TeamKnockbackMultiplier", 1f].f();
		h.TeamKnockbackMult = tkm;
		var tdm = inif[section, "TeamDamageMultiplier", 1f].f();
		h.TeamDamageMult = tdm;
		var tsm = inif[section, "TeamStunMultiplier", 1f].f();
		h.TeamStunMult = tsm;
		
		var whitelistedStates = inif[section, "WhitelistedStates", Enumerable.Empty<string>()].ls();
		h.WhitelistedStates = new HashSet<string>(whitelistedStates);
		
		var blacklistedStates = inif[section, "BlacklistedStates", Enumerable.Empty<string>()].ls();
		h.BlacklistedStates = new HashSet<string>(blacklistedStates);
		
		h.LoadProperties();
		LoadExtraProperties(h, section);
		
		//Build collision. no need for seperate function
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		h.OriginalPosition = pos;
		var rot = inif[section, "Rotation", 0f].f();
		var rotrad = (float)(rot*Math.PI/180f);
		cs.Rotation = rotrad;//turn to rads
		h.OriginalRotation = rotrad;
		
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 0f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 0f].f();
		ps.Height = hg;
		
		cs.Shape = ps;
		cs.Disabled = true;
		cs.Name = section + "Collision";
		h.HitboxShape = cs;
		h.AddChild(cs);
		cs.Owner = h;//for scene packing
		ap.AddChild(h);
		h.Owner = ap;//for scene packing
	}
	
	public void BuildTransition(AttackPart ap, string section)
	{
		if(!inif.HasSection(section)) {GD.PushError($"Can't generate transition {section} as it is not a real section"); return;}
		
		var tags = inif[section, "Tag", ""].s();
		
		try
		{
			var parsedTagList = ParseTagList(tags);
			var tagExpression = new AttackPartTransitionTagExpression();
			var frames = inif[section, "Frames", Enumerable.Empty<Vector2>()].lv2();
			var nextPart = inif[section, "Next", ""].s();
			var addedTransition = new AttackPartTransition(frames, tagExpression, nextPart);
			ap.TransitionManager.Add(addedTransition);
			
		}
		catch(FormatException fe)
		{
			GD.PushError(fe.Message);
			return;
		}
	}
	
	private static readonly char[] OPERATORS = new char[]{'!', '|', '&', '(', ')'};
	public IEnumerable<object> ParseTagList(string s)
	{
		if(s == "") yield break;
		
		var operators = new Stack<char>();
		
		var stateName = new StringBuilder();
		var tagValue = new StringBuilder();
		bool doingTagValue = false;
		
		foreach(var c in s)
		{
			if(OPERATORS.Contains(c))
			{
				if(!doingTagValue) throw new FormatException($"Too few periods in attack part transition expression {s}");
				
				var _stateName = stateName.ToString();
				var _tagValue = tagValue.ToString();
				StateTag tag;
				if(!Enum.TryParse<StateTag>(_tagValue, out tag)) throw new FormatException($"Unknown tag value {_tagValue} in attack part transition expression {s}");
				yield return (_stateName, tag);
				stateName.Clear(); tagValue.Clear(); doingTagValue = false;
				
				if(c == '(') operators.Push(c);
				else if(c == ')')
				{
					while(operators.Count > 0 && operators.Peek() != '(') yield return operators.Pop();
					if(operators.Count == 0) throw new FormatException($"Attack part transition expression {s} has imbalanced brackets");
					operators.Pop(); //remove the (
				}
				else
				{
					int pre = OPERATORS.FindIndex(c);
					while(operators.Count > 0 && OPERATORS.FindIndex(operators.Peek()) <= pre) yield return operators.Pop();
					operators.Push(c);
				}
			}
			else if(c == '.')
			{
				if(doingTagValue) throw new FormatException($"Too many periods in attack part transition expression {s}");
				doingTagValue = true;
			}
			else
			{
				if(doingTagValue) tagValue.Append(c);
				else stateName.Append(c);
			}
		}
		
		if(doingTagValue)
		{
			var _stateName = stateName.ToString();
			var _tagValue = tagValue.ToString();
			StateTag tag;
			if(!Enum.TryParse<StateTag>(_tagValue, out tag)) throw new FormatException($"Unknown tag value {_tagValue} in attack part transition expression {s}");
			yield return (_stateName, tag);
			stateName.Clear(); tagValue.Clear(); doingTagValue = false;
		}
		else if(stateName.Length != 0) throw new FormatException($"Too few periods in attack part transition expression {s}");
		
		while(operators.Count > 0) yield return operators.Pop();
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
