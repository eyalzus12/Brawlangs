extends Node
class_name AttackComponent

const Hitbox = preload("res://Scripts/AttackScripts/Hitbox/Hitbox.cs")
const Hurtbox = preload("res://Scripts/AttackScripts/Hurtbox/Hurtbox.cs")
const AttackPart = preload("res://Scripts/AttackScripts/AttackPart/AttackPart.cs")
const Attack = preload("res://Scripts/AttackScripts/Attack/Attack.cs")

var attack: Attack

func init() -> void:
	pass

func on_start() -> void:
	pass

func on_end() -> void:
	pass

func loop() -> void:
	pass

func can_start() -> bool:
	return true

func on_hit(hitbox: Hitbox, hurtbox: Hurtbox) -> void:
	pass

func on_part_change(part: AttackPart) -> void:
	pass

func get_extra_properties_list() -> Array:
	return []
