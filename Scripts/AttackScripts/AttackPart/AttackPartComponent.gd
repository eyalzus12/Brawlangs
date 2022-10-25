extends Node
class_name AttackPartComponent

const IHittable = preload("res://Scripts/AttackScripts/Interfaces/IHittable.cs")
const Hitbox = preload("res://Scripts/AttackScripts/Hitbox/Hitbox.cs")
const Hurtbox = preload("res://Scripts/AttackScripts/Hurtbox/Hurtbox.cs")
const AttackPart = preload("res://Scripts/AttackScripts/AttackPart/AttackPart.cs")

var attack_part: AttackPart

func init() -> void:
	pass

func on_start() -> void:
	pass

func on_end() -> void:
	pass

func loop() -> void:
	pass

func pause() -> void:
	pass

func resume() -> void:
	pass

func startup_finish() -> void:
	pass

func initial_hit_event(hitbox: Hitbox, hurtbox: Hurtbox) -> void:
	pass

func hit_event(hitbox: Hitbox, hurtbox: Hurtbox) -> void:
	pass

func can_hit(h: IHittable) -> bool:
	return true

func get_extra_properties_list() -> Array:
	return []
