using Godot;
using System;

public partial class SelectScreenLineEdit : LineEdit
{
	public CheckBox check;
	
	public int number = 1;
	public const string PREFIX = "LoadedCharacter";
	
	private PublicData data;
	
	public override void _Ready()
	{
		number = int.Parse(GetParent().Name.ToString().Substring("Load".Length));
		//take parent name
		//remove "Load"
		//parse
		
		data = this.GetPublicData();
		object o = null;
		if(data.TryGet($"{PREFIX}{number}", out o)) Text = o.s();
		
		//bruh wtf is this
		GetParent().GetParent().GetParent().GetNode<Button>("ExitButton").Connect("pressed",new Callable(this,nameof(OnExit)));
		
	}
	
	public void OnExit()
	{
		if(Text == "")
		{
			data.Remove($"{PREFIX}{number}");
		}
		else
		{
			data.AddOverride($"{PREFIX}{number}", Text);
		}
	}
}
