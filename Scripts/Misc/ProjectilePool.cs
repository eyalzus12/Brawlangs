using Godot;
using System;
using System.Collections.Generic;

public class ProjectilePool : Node
{
	public const int LoadAmount = 3;
	public Dictionary<string, Queue<Projectile>> ProjectileDict;
	public Dictionary<string, string> LoadDict;
	public Queue<Tuple<string, Projectile>> ReturnQueue;
	public HashSet<Projectile> ReturnQueueSet;
	public ProjectileCreator ProjCreat;
	
	public ProjectilePool()
	{
		ProjectileDict = new Dictionary<string, Queue<Projectile>>();
		LoadDict = new Dictionary<string, string>();
		ProjCreat = new ProjectileCreator();
	}
	
	public override void _PhysicsProcess(float delta)
	{
		CleanReturnQueue();
	}
	
	public Projectile GetProjectile(string identifier, string loader = "")
	{
		Queue<Projectile> poolQueue;
		if(!ProjectileDict.TryGetValue(identifier, out poolQueue) || poolQueue.Count <= 0)//no available objects
		{
			poolQueue = CreateNewProjectile(identifier, loader);//load new ones
		}
		
		return poolQueue?.Dequeue();//get available object if exists
	}
	
	public Queue<Projectile> CreateNewProjectile(string identifier, string source = "")
	{
		if(!ProjectileDict.ContainsKey(identifier))//no queue exists
		{
			if(source == "")//cant instance 
			{
				GD.Print($"Cannot pool object with identifier {identifier} because given section is null");
				return null;
			}
			
			ProjectileDict.Add(identifier, new Queue<Projectile>());//make a queue
			LoadDict.Add(identifier, source);
		}
		
		for(int i = 0; i < LoadAmount; ++i)
		{
			/*var loader = source??LoadDict[identifier];
			var obj = source.Instance<Node2D>();//instance new object*/
			var obj = ProjCreat.BuildProjectile(source);
			ProjectileDict[identifier].Enqueue(obj);//put in the pool
		}
		
		return ProjectileDict[identifier];
	}
	
	public bool InsertProjectile(Projectile p, string identifier = "")
	{
		if(identifier == "") identifier = p.Filename;
		
		if(identifier == "")
		{
			GD.Print($"Cannot pool object {p} because it does not have a given identifier (or wasnt generated from one)");
			return false;
		}
		
		if(ReturnQueueSet.Contains(p)) return false;
		ReturnQueue.Enqueue(new Tuple<string,Projectile>(identifier,p));
		ReturnQueueSet.Add(p);
		return true;
	}
	
	public void CleanReturnQueue()
	{
		while(ReturnQueue.Count > 0)
		{
			var h = ReturnQueue.Dequeue();
			var obj = h.Item2;
			var identifier = h.Item1;
			if(obj is null || !Godot.Object.IsInstanceValid(obj)) continue;
			var parent = obj.GetParent();
			parent.RemoveChild(obj);
			
			if(!ProjectileDict.ContainsKey(identifier))
				ProjectileDict.Add(identifier, new Queue<Projectile>());//make a queue
			ProjectileDict[identifier].Enqueue(obj);
			
			ReturnQueueSet.Remove(obj);
		}
	}
	
	public void ClearPool()
	{
		foreach(var entry in ProjectileDict)
		{
			var queue = entry.Value;
			while(queue.Count > 0)
			{
				var obj = queue.Dequeue();
				obj.QueueFree();
			}
		}
		
		ProjectileDict.Clear();
		LoadDict.Clear();
	}
	
	public override void _ExitTree()
	{
		ClearPool();
		ReturnQueue.Clear();
		ReturnQueueSet.Clear();
	}
}
