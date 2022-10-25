extends Node
class_name HitboxComponent

const Hurtbox = preload("res://Scripts/AttackScripts/Hurtbox/Hurtbox.cs")
const IHittable = preload("res://Scripts/AttackScripts/Interfaces/IHittable.cs")
const Hitbox = preload("res://Scripts/AttackScripts/Hitbox/Hitbox.cs")

var hitbox: Hitbox

#ran when the hitbox is created
func init() -> void:
	pass

#ran when the hitbox is activated
func on_activate() -> void:
	pass

#ran when the hitbox is deactivated
func on_deactivate() -> void:
	pass

#ran when the hitbox hits a hurtbox
func on_hit(hurtbox: Hurtbox) -> void:
	pass

#returns whether the hitbox is allowed to hit the given hittable
func can_hit(h: IHittable) -> bool:
	return true

#ran every frame the hitbox is active
func loop() -> void:
	pass

#returns this component's knockback multiplier for the given hittable
func get_knockback_mult(h: IHittable) -> float:
	return 1.0

#returns this component's damage multiplier for the given hittable
func get_damage_mult(h: IHittable) -> float:
	return 1.0

#returns this component's stun multiplier for the given hittable
func get_stun_mult(h: IHittable) -> float:
	return 1.0

#returns an array of arrays. each element in the array is an array with two elements.
#the first is a string for the name of the property, the second is the default value for that property.
func get_extra_properties_list() -> Array:
	return []
