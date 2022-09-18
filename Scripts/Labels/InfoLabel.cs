using Godot;
using System;
using System.Text;

public partial class InfoLabel : Label
{
	public InfoLabel() {}
	
	public bool debugMode;
	protected StringBuilder commit = new StringBuilder();
	
	public override void _Ready()
	{
		ProcessMode = Node.ProcessModeEnum.Always;
		debugMode = false;
		Connect();
	}
	
	public virtual void Connect() {}
	
	public override void _PhysicsProcess(double delta)
	{
		if(Input.IsActionJustPressed("debug_toggle"))
			debugMode = !debugMode;
		
		Visible = debugMode && EnsureCorrectAppearence();
		
		commit.Clear();
		
		if(Visible)
		{
			UpdateLabel();
			Commit();
		}
	}
	
	public virtual void UpdateLabel() {}
	
	public void Commit() => Text = commit.ToString().Trim();
	
	public void Add(string name, object obj, bool dot=true) 
	{
		commit.Append($"{name}");
		if(dot) commit.Append(": ");
		commit.Append($"{obj.ToString()}    ");
	}
	
	public void Newline() => commit.AppendLine();
	
	protected virtual bool EnsureCorrectAppearence() => true;
}
