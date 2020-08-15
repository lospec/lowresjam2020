class_name CombatUtil

const color_values = preload("res://Utility/ColorValues.gd")

# Constants
enum Combat_Action {
	INVALID = -1,
	NONE,
	QUICK,
	COUNTER,
	HEAVY,
	FLEE,
}


class FleeRule:
	enum Outcome { SUCCESS, SUCCESS_DMG, FAIL }
	var _flee_no_dmg_chance
	var _flee_dmg_chance
	var _no_flee_chance
	var _dmg_modifier

	var _outcome_table

	func roll():
		var _roll = randf()
		var chance = 0.0
		for outcome in _outcome_table:
			chance += _outcome_table[outcome]
			if _roll < chance:
				return outcome

	func _init(combat_action):
		_flee_no_dmg_chance = FLEE_RULES[combat_action][0]
		_flee_dmg_chance = FLEE_RULES[combat_action][1]
		_no_flee_chance = FLEE_RULES[combat_action][2]
		_dmg_modifier = FLEE_RULES[combat_action][3]
		_outcome_table = {
		Outcome.SUCCESS: _flee_no_dmg_chance,
		Outcome.SUCCESS_DMG: _flee_dmg_chance,
		Outcome.FAIL: _no_flee_chance
	}


const FLEE_RULES = {
	Combat_Action.NONE: [1, 0, 0, 0],
	Combat_Action.COUNTER: [1, 0, 0, 0],
	Combat_Action.QUICK: [0.2, 0.3, 0.5, 2.0],
	Combat_Action.HEAVY: [0.2, 0.5, 0.3, 1.0]
}

# Should probably change to something better
# maybe make it a variable per char or something
const MULTIPLIER_PER_COMBO: int = 1

static func GetActionName(action: int) -> String:
	match action:
		Combat_Action.QUICK:
			return "Quick"

		Combat_Action.COUNTER:
			return "Counter"

		Combat_Action.HEAVY:
			return "Heavy"

		Combat_Action.FLEE:
			return "Flee"

		Combat_Action.NONE:
			return "None"

		_:
			return "INVALID"

static func GetActionWeakness(action: int) -> int:
	#print("WEAKNESS: %s" % action)
	match action:
		Combat_Action.QUICK:
			return Combat_Action.COUNTER
		Combat_Action.COUNTER:
			return Combat_Action.HEAVY
		Combat_Action.HEAVY:
			return Combat_Action.QUICK
		_:
			print("ERROR: Invalid action to get weakness")
	return Combat_Action.INVALID

static func ActionCompare(action1: int, action2: int) -> int:
	if action1 == Combat_Action.FLEE:
		if action2 == action1 or action2 == Combat_Action.COUNTER:
			return 1

		else:
			return 2

	if action2 == Combat_Action.FLEE:
		if action1 == action2 or action1 == Combat_Action.COUNTER:
			return 2

		else:
			return 1

	if action1 == action2:
		return 0
	elif action2 == Combat_Action.NONE:
		return 1
	elif action1 == Combat_Action.NONE:
		return 2
	elif GetActionWeakness(action2) == action1:
		return 1
	elif GetActionWeakness(action1) == action2:
		return 2
	return -1

static func GetActionColor(action: int) -> Color:
	match action:
		Combat_Action.QUICK:
			return color_values.att_quick

		Combat_Action.COUNTER:
			return color_values.att_counter

		Combat_Action.HEAVY:
			return color_values.att_heavy

		Combat_Action.FLEE:
			return color_values.flee

		Combat_Action.NONE:
			return color_values.white

		_:
			print("ERROR CombatUtil.gd: Invalid Action whlie getting color")
			return color_values.invalid
