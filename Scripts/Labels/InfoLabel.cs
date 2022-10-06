using Godot;
using System;
using System.Text;

public class InfoLabel : Label
{
	public InfoLabel() {}
	
	private bool _debugMode = false;
	public bool DebugMode
	{
		get => _debugMode;
		set
		{
			_debugMode = value;
			Visible = value;
		}
	}
	
	protected StringBuilder Commit{get; set;} = new StringBuilder();
	
	public override void _Ready()
	{
		PauseMode = Node.PauseModeEnum.Process;
		DebugMode = false;
		Init();
	}
	
	public virtual void Init() {}
	
	public override void _PhysicsProcess(float delta)
	{
		if(Input.IsActionJustPressed("debug_toggle")) DebugMode = !DebugMode;
		if(Visible)
		{
			Commit.Clear();
			UpdateLabel();
			ApplyText();
		}
	}
	
	public virtual void UpdateLabel() {}
	
	public void ApplyText() => Text = Commit.ToString().Trim();
	
	public void Add(string name, object obj, bool dot=true, bool space=true) 
	{
		Commit.Append(name);
		if(dot) Dot();
		Commit.Append(obj.ToString());
		if(space) Space();
	}
	
	public void Dot() => Commit.Append(": ");
	public void Space() => Commit.Append("    ");
	public void Newline() => Commit.AppendLine();
}
