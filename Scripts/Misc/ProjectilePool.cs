using Godot;
using System;
using System.Collections.Generic;

public partial class ProjectilePool : Node
{
	public const int LOAD_AMOUNT = 3;
	public Dictionary<string, Queue<Projectile>> ProjectileDict{get; set;} = new();
	public Queue<(string, Projectile)> ReturnQueue{get; set;} = new();
	public HashSet<Projectile> ReturnQueueSet{get; set;} = new();
	public ProjectileCreator ProjCreate{get; set;} = new();
	public IAttacker OwnerObject{get; set;}
	
	public ProjectilePool()
	{
		
	}
	
	public ProjectilePool(IAttacker owner)
	{
		OwnerObject = owner;
	}
	
	public override void _PhysicsProcess(double delta)
	{
		CleanReturnQueue();
	}
	
	public Projectile GetProjectile(string identifier, string loader = "")
	{
		Queue<Projectile> poolQueue;
		if(!ProjectileDict.TryGetValue(identifier, out poolQueue) || poolQueue.Count <= 0)//no available objects
		{
			poolQueue = CreateNewProjectile(identifier);//load new ones
		}
		
		return poolQueue?.Dequeue();//get available object if exists
	}
	
	public Queue<Projectile> CreateNewProjectile(string identifier)
	{
		if(!ProjectileDict.ContainsKey(identifier))//no queue exists
			ProjectileDict.Add(identifier, new());//make a queue
		
		for(int i = 0; i < LOAD_AMOUNT; ++i)
		{
			if(OwnerObject is null)
			{
				GD.PushError("projectile pool's owner is null. failed to create new pooled projectiles");
				return null;
			}
			
			var obj = ProjCreate.BuildProjectile(OwnerObject, identifier);
			
			if(obj is null)
			{
				GD.PushError("null projectile generated in projectile pool. failed to create new pooled projectiles");
				return null;
			}
			
			ProjectileDict[identifier].Enqueue(obj);//put in the pool
		}
		
		return ProjectileDict[identifier];
	}
	
	public bool InsertProjectile(Projectile p)
	{
		var identifier = p.Identifier;
		
		if(identifier == "")
		{
			GD.PushError($"Cannot pool projectile {p} because it does not have an identifier");
			return false;
		}
		
		if(ReturnQueueSet.Contains(p))
		{
			GD.PushError($"Cannot pool projectile {p} because it already got pooled");
			return false;
		}
		
		ReturnQueue.Enqueue((identifier, p));
		ReturnQueueSet.Add(p);
		p.GetParent().RemoveChild(p);
		return true;
	}
	
	public void CleanReturnQueue()
	{
		while(ReturnQueue.Count > 0)
		{
			var h = ReturnQueue.Dequeue();
			var identifier = h.Item1;
			var obj = h.Item2;
			if(obj is null || !Godot.Object.IsInstanceValid(obj))
			{
				GD.PushError($"Invalid instance of object {obj} was found while cleaning return queue. Cheese fucked up somewhere.");
				continue;
			}
			
			if(!ProjectileDict.ContainsKey(identifier))
				ProjectileDict.Add(identifier, new());//make a queue
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
	}
	
	public override void _ExitTree()
	{
		ClearPool();
		ReturnQueue.Clear();
		ReturnQueueSet.Clear();
	}
}
