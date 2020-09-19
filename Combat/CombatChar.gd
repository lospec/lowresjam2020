extends Node

class_name CombatChar

# Signals
signal damage_taken(damage, damage_type)
#signal attack_char(target, damage)
#signal died()

# Constants


# Public Variables
var hit_combo: int = 0
var char_instance: BaseEntity

# warning-ignore:unused_argument
func take_damage(damage: int, damage_type: String, instance: BaseEntity):
	print("TAKE DAMAGE: %s type %s" % [damage, damage_type])
	emit_signal("damage_taken", damage, damage_type)
	
	# HARDCODED INTERACTION: Should make a system to make this more flexible
	if char_instance.status_effects.has("Frozen") and damage_type == "Fire":
		char_instance.status_effects.erase("Frozen")
	
	char_instance.health -= damage
	
	if char_instance.health <= 0:
		char_instance.health = 0
	
	hit_combo = 0
	
	if instance.is_in_group("PlayerGroup"):
		var _a = AudioSystem.play_sfx(AudioSystem.SFX.PLAYER_HURT, null, -30)
	else:
		var enemy_data = Data.enemy_data[instance.enemy_name]
		match enemy_data.race:
			"Beast":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.BEAST_HURT, null, -30)
			"Demon":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.DEMON_HURT, null, -30)
			"Flora":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.FLORA_HURT, null, -30)
			"Human":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.HUMAN_HURT, null, -30)
			"Robot":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.ROBOT_HURT, null, -30)
			"Slime":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.SLIME_HURT, null, -30)


func attack(target: CombatChar, action: int, damage: int, instance: BaseEntity,
		target_instance: BaseEntity):
	if char_instance.status_effects.has("Weak"):
		damage /= 2
	
	damage = int(max(damage, 1))
	
	var damage_type = get_damage_type(action)
	target.take_damage(damage, damage_type, target_instance)
	
	# Apply Stat Effect
	var se: String = get_status_effect(action)
	var sePercent: float = get_effect_chance(action)
	
	# HARDCODED RESISTANCE
	if se == "OnFire" and "resistance" in target.char_instance:
		if target.char_instance.resistance == "Fire":
			return
	
	if randf() < sePercent:
		target.char_instance.add_status_effect(se)
		
		match se:
			"Charged":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.CHARGED_STATUS, null, -30)
			"Confusion":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.CONFUSION_STATUS, null, -30)
			"Frozen":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.FROZEN_STATUS, null, -30)
			"Poisoned":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.POISONED_STATUS, null, -30)
			"OnFire":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.ONFIRE_STATUS, null, -30)
			"Weak":
				var _a = AudioSystem.play_sfx(AudioSystem.SFX.WEAK_STATUS, null, -30)
		
		if "enemy_name" in target.char_instance:
			print("Applied %s to %s" % [se, target.char_instance.enemy_name])

# warning-ignore:unused_argument
func get_base_damage(action: int) -> int:
	return 0

# warning-ignore:unused_argument
func get_damage_type(action: int) -> String:
	return "None"

# warning-ignore:unused_argument
func get_status_effect(action: int) -> String:
	return "None"

# warning-ignore:unused_argument
func get_effect_chance(action: int) -> float:
	return 0.0

func get_action() -> int:
	return -1
