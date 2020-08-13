extends Node

const weapon_util = preload("res://Utility/WeaponUtil.gd")

var attack_anims = {}

func get_anim(key) -> AnimatedTexture:
	return attack_anims[key]

func get_damage_type_anim(type) -> AnimatedTexture:
	var key = type
	if key is int:
		key = weapon_util.get_damage_type_name(type)

	return attack_anims[key.to_lower()]

func _ready():
	var dmg_types = weapon_util.Damage_Type
	var file = File.new()
	
	for type in dmg_types:
		type = type.to_lower()
		var path = "res://Combat/Effects/animations/attack_animation_%s.tres" % type
		if file.file_exists(path):
			attack_anims[type] = load(path)
	
	# PLACEHOLDER
	attack_anims["counter"] = load("res://Combat/Effects/animations/placeholder_counter_anim.tres")
