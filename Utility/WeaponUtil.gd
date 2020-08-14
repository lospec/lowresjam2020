
enum Damage_Type {
	NONE,
	PIERCE,
	BLUNT,
	FIRE,
	WATER,
	ELECTRICITY,
}

static func get_damage_type_name(type: int) -> String:
	return Damage_Type.keys()[type]
