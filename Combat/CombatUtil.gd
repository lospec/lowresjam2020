
# Constants
enum Combat_Action {
	INVALID = -1,
	QUICK,
	COUNTER,
	HEAVY,
}

static func GetActionWeakness(action):
	print("WEAKNESS: %s" % action)
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

static func ActionCompare(action1, action2):
	if action1 == action2:
		return 0
	elif GetActionWeakness(action2) == action1:
		return 1
	elif GetActionWeakness(action1) == action2:
		return 2
	return -1
