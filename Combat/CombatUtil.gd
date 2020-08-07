
const color_values = preload("res://Utility/ColorValues.gd")

# Constants
enum Combat_Action {
	INVALID = -1,
	QUICK,
	COUNTER,
	HEAVY,
	FLEE,
}

# Should probably change to something better
# maybe make it a variable per char or something
const MULTIPLIER_PER_COMBO: int = 1

static func GetActionWeakness(action: int) -> int:
	#print("WEAKNESS: %s" % action)
	match (action):
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
	if action1 == action2:
		return 0
	elif GetActionWeakness(action2) == action1:
		return 1
	elif GetActionWeakness(action1) == action2:
		return 2
	return -1

static func GetCombatActionColor(action: int) -> Color:
	match action:
		Combat_Action.QUICK:
			return color_values.att_quick
		
		Combat_Action.COUNTER:
			return color_values.att_counter
		
		Combat_Action.HEAVY:
			return color_values.att_heavy
		
		_:
			print("ERROR CombatUtil.gd: Invalid Action whlie getting color")
			return color_values.invalid
