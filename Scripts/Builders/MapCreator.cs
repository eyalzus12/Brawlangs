using Godot;
using System;
using System.Collections.Generic;

public class MapCreator
{
	public const string BASE_SECTION = "Base";
	
	public string path = "res://IcerMap.ini";
	public IniFile inif = new IniFile();
	
	public MapCreator() {}
	public MapCreator(string path)
	{
		this.path = path;
		inif.Load(path);
	}
	
	public bool Build(Node n)
	{
		var plats = inif[BASE_SECTION, "Platforms", null];
		if(plats is string) BuildPlatform(plats.s(), n);
		else foreach(var str in plats.ls()) BuildPlatform(str, n);
		
		var camera = new MatchCamera();
		camera.Name = "Camera";
		var cbounds = inif[BASE_SECTION, "CameraBounds", MatchCamera.DEFAULT_LIMITS].v2();
		camera.limits = cbounds;
		var center = inif[BASE_SECTION, "Center", Vector2.Zero].v2();
		camera.middle = center;
		var zoom = inif[BASE_SECTION, "BaseZoom", 1.7f].f();
		camera.baseZoom = zoom;
		var zoff = inif[BASE_SECTION, "ZoomOffset", 0.1f].f();
		camera.zoomOffset = zoff;
		var inter = inif[BASE_SECTION, "Interpolation", 0.01f].f();
		camera.interpolationWeight = inter;
		
		camera.Current = true;
		
		var blast = inif[BASE_SECTION, "BlastZones", BlastZone.DEFAULT_SIZE].v2();
		var bz = new BlastZone(center, blast);
		
		n.AddChild(camera);
		n.AddChild(bz);
		return true;
	}
	
	public void BuildPlatform(String section, Node n)
	{
		section = section.Trim();
		var sp = new StaticPlatform2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		sp.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		sp.Rotation = rot;
		var fr = inif[section, "Friction", 1f].f();
		sp.PlatformFriction = fr;
		var bn = inif[section, "Bounce", 0f].f();
		sp.PlatformBounce = bn;
		var vl = inif[section, "Velocity", Vector2.Zero].v2();
		sp.PlatformLinearVelocity = vl;
		var cl = inif[section, "Clingable", true].b();
		sp.Clingable = cl;
		var ft = inif[section, "FallThrough", false].b();
		sp.FallThroughPlatform = ft;
		
		var cp = inif[section, "Collision", section+"col"];
		if(cp is string) BuildCollision(cp.s(), sp);
		else foreach(var s in cp.ls()) BuildCollision(s, sp);
		
		var sr = inif[section, "Sprite", section+"sprite"];
		if(sr is string) BuildSprite(sr.s(), sp);
		else foreach(var ss in sr.ls()) BuildSprite(ss, sp);
		
		n.AddChild(sp);
	}
	
	public void BuildCollision(string section, Node n)
	{
		section = section.Trim();
		var cs = new CollisionShape2D();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		cs.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		cs.Rotation = rot;
		var ds = inif[section, "Disabled", false].b();
		cs.Disabled = ds;
		var ow = inif[section, "OneWay", false].b();
		cs.OneWayCollision = ow;
		var type = inif[section, "Type", ""].s();
		cs.Shape = t2s(type, section);
		
		n.AddChild(cs);
	}
	
	public void BuildSprite(string section, Node n)
	{
		section = section.Trim();
		var sp = new Sprite();
		
		var pos = inif[section, "Position", Vector2.Zero].v2();
		sp.Position = pos;
		var rot = inif[section, "Rotation", 0f].f();
		sp.Rotation = rot;
		var scl = inif[section, "Scale", new Vector2(1f,1f)].v2();
		sp.Scale = scl;
		var te = inif[section, "Texture", "res://icon.png"].s();
		sp.Texture = ResourceLoader.Load<Texture>(te);
		var zz = inif[section, "Z", 1].i();
		sp.ZIndex = zz;
		
		n.AddChild(sp);
	}
	
	private Shape2D t2s(string type, string section) => GetFunc(type)(section);
	
	private Func<string, Shape2D> GetFunc(string type)
	{
		switch(type)
		{
			case "Rectangle":
				return s2r;
			case "Circle":
				return s2c;
			case "Capsule":
				return s2p;
			case "Line":
				return s2l;
			case "ConvexPolygon":
				return s2xp;
			case "ConvcavePolygon":
				return s2vp;
			case "Segment":
				return s2s;
			default:
				return null;
		}
	}
	
	private RectangleShape2D s2r(string section)
	{
		var rs = new RectangleShape2D();
		
		var ex = inif[section, "Extents", new Vector2(16f,16f)].v2();
		rs.Extents = ex;
		
		return rs;
	}
	
	private CircleShape2D s2c(string section)
	{
		var cs = new CircleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		cs.Radius = rd;
		
		return cs;
	}
	
	private CapsuleShape2D s2p(string section)
	{
		var ps = new CapsuleShape2D();
		
		var rd = inif[section, "Radius", 16f].f();
		ps.Radius = rd;
		var hg = inif[section, "Height", 16f].f();
		ps.Height = hg;
		
		return ps;
	}
	
	private LineShape2D s2l(string section)
	{
		var ls = new LineShape2D();
		
		var nr = inif[section, "Normal", new Vector2(0, -1)].v2();
		ls.Normal = nr;
		var ds = inif[section, "Distance", 0f].f();
		ls.D = ds;
		
		return ls;
	}
	
	private ConvexPolygonShape2D s2xp(string section)
	{
		var xps = new ConvexPolygonShape2D();
		
		var vl = inif[section, "Points", null].lv2().ToArray();
		xps.Points = vl;
		
		return xps;
	}
	
	private ConcavePolygonShape2D s2vp(string section)
	{
		var vps = new ConcavePolygonShape2D();
		
		var vl = inif[section, "Segments", null].lv2().ToArray();
		vps.Segments = vl;
		
		return vps;
	}
	
	private SegmentShape2D s2s(string section)
	{
		var ss = new SegmentShape2D();
		
		var a = inif[section, "PointA", Vector2.Zero].v2();
		ss.A = a;
		var b = inif[section, "PointB", Vector2.Zero].v2();
		ss.B = b;
		
		return ss;
	}
}
