using Godot;
using System;

public partial class StageSelect : Control
{
	protected const string LOCAL_PATH = "res://maps/";
	protected const string REMOTE_PATH = "user://maps/";
	protected const string CONT_NAME = "GridContainer";
	protected const string MAP_EXT = ".tscn";
	protected GridContainer cont;
	
	public override void _Ready()
	{
		cont = GetNode<GridContainer>(CONT_NAME);
		var path_object = this.GetData("ini_path") as string;
		
		if(path_object is not null)
		{
			var te = GetNode<TextEdit>("TextEdit");
			te.Text = path_object ?? "";
		}
		
		ImportFromPath(LOCAL_PATH);
		ImportFromPath(REMOTE_PATH);
		
		//GD.Print(this.GetPublicData().ToString());
	}
	
	public void ImportFromPath(string path)
	{
		var dir = new Directory();
		
		if(dir.Open(path) == Error.Ok)
		{
			foreach(var file in dir.GetFiles()) if(StringUtils.GetExtension(file) == MAP_EXT)
			{
				AddMap(file, path);
			}
		}
	}
	
	public void AddMap(string map, string path)
	{
		var sel = new MapSelectorButton(map, path);
		sel.Text = StringUtils.PascalCaseToSentence(StringUtils.RemoveExtension(map));
		cont?.AddChild(sel);
	}
	
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("exit_game")) GetTree().Quit();
	}
}
