using Godot;
using System;
using System.Collections.Generic;

public interface IProjectileEmitter
{
	Dictionary<string, HashSet<Projectile>> ActiveProjectiles{get; set;}
	ProjectilePool ProjPool{get; set;}
	
	void DisableAllProjectiles();
	void EmitProjectile(string proj);
	void HandleProjectileDestruction(Projectile who);
}
