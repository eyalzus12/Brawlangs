using Godot;
using System;
using System.Collections.Generic;

public class ProjectilePool : Node
{
	public const int LOAD_AMOUNT = 5;
	
	public Dictionary<string, Queue<Projectile>> ProjectileDict{get; set;} = new Dictionary<string, Queue<Projectile>>();
	public Queue<(string, Projectile)> ReturnQueue{get; set;} = new Queue<(string, Projectile)>();
	public HashSet<Projectile> ReturnQueueSet{get; set;} = new HashSet<Projectile>();
	public ProjectileCreator ProjCreate{get; set;} = new ProjectileCreator();
	public IAttacker OwnerObject{get; set;}
	
	public ProjectilePool() {}
	public ProjectilePool(IAttacker owner) {OwnerObject = owner;}
	
	public override void _PhysicsProcess(float delta)
	{
		CleanReturnQueue();
	}
	
	public Projectile GetProjectile(string identifier, int loadAmount = LOAD_AMOUNT, string loader = "")
	{
		Queue<Projectile> poolQueue;
		if(!ProjectileDict.TryGetValue(identifier, out poolQueue) || poolQueue.Count <= 0)//no available objects
		{
			poolQueue = CreateNewProjectiles(identifier, loadAmount);//load new ones
		}
		
		return poolQueue?.Dequeue();//get available object if exists
	}
	
	public Queue<Projectile> CreateNewProjectiles(string identifier, int amount)
	{
		if(OwnerObject is null)
		{
			GD.PushError("projectile pool's owner is null. failed to create new pooled projectiles");
			return null;
		}
		
		if(!ProjectileDict.ContainsKey(identifier))//no queue exists
			ProjectileDict[identifier] = new Queue<Projectile>();//make a queue
		
		for(int i = 0; i < amount; ++i)
		{
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
			(string identifier, Projectile obj) = ReturnQueue.Dequeue();
			if(obj is null || !Godot.Object.IsInstanceValid(obj))
			{
				GD.PushError($"Invalid instance of object {obj} was found while cleaning return queue. Cheese fucked up somewhere.");
				continue;
			}
			
			if(!ProjectileDict.ContainsKey(identifier)) ProjectileDict[identifier] = new Queue<Projectile>();//make a queue
			ProjectileDict[identifier].Enqueue(obj);
			
			ReturnQueueSet.Remove(obj);
		}
	}
	
	public void ClearPool()
	{
		foreach(var queue in ProjectileDict.Values)
		while(queue.Count > 0)
		{
			var p = queue.Dequeue();
			p.Movement.QueueFree();
			p.QueueFree();
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
