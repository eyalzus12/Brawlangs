using Godot;
using System;
using System.Collections.Generic;

public class ProjectilePool : Node
{
	public const int LoadAmount = 3;
	public Dictionary<string, Queue<Projectile>> ProjectileDict;
	public Queue<(string, Projectile)> ReturnQueue;
	public HashSet<Projectile> ReturnQueueSet;
	public ProjectileCreator ProjCreate;
	public Node2D owner;
	
	public ProjectilePool()
	{
		ProjectileDict = new Dictionary<string, Queue<Projectile>>();
		ReturnQueueSet = new HashSet<Projectile>();
		ReturnQueue = new Queue<(string, Projectile)>();
		ProjCreate = new ProjectileCreator();
	}
	
	public ProjectilePool(Node2D owner)
	{
		ProjectileDict = new Dictionary<string, Queue<Projectile>>();
		ReturnQueueSet = new HashSet<Projectile>();
		ReturnQueue = new Queue<(string, Projectile)>();
		this.owner = owner;
		ProjCreate = new ProjectileCreator();
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
			poolQueue = CreateNewProjectile(identifier);//load new ones
		}
		
		return poolQueue?.Dequeue();//get available object if exists
	}
	
	public Queue<Projectile> CreateNewProjectile(string identifier)
	{
		if(!ProjectileDict.ContainsKey(identifier))//no queue exists
			ProjectileDict.Add(identifier, new Queue<Projectile>());//make a queue
		
		for(int i = 0; i < LoadAmount; ++i)
		{
			var obj = ProjCreate.BuildProjectile(owner, identifier);
			ProjectileDict[identifier].Enqueue(obj);//put in the pool
		}
		
		return ProjectileDict[identifier];
	}
	
	public bool InsertProjectile(Projectile p)
	{
		var identifier = p.identifier;
		
		if(identifier == "")
		{
			GD.Print($"Cannot pool projectile {p} because it does not have an identifier");
			return false;
		}
		
		if(ReturnQueueSet.Contains(p))
		{
			GD.Print($"Cannot pool projectile {p} because it already got pooled");
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
				GD.Print($"Invalid instance of object {obj} was found while cealning return queue. Cheese fucked up somewhere.");
				continue;
			}
			
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
	}
	
	public override void _ExitTree()
	{
		ClearPool();
		ReturnQueue.Clear();
		ReturnQueueSet.Clear();
	}
}
