extends Node

# Constants
enum Combat_Action {
	INVALID = -1,
	QUICK,
	COUNTER,
	HEAVY,
}


func GetActionWeakness(action):
	match (action):
		Combat_Action.QUICK:
			return Combat_Action.COUNTER
		Combat_Action.COUNTER:
			return Combat_Action.HEAVY
		Combat_Action.HEAVY:
			return Combat_Action.QUICK
	return Combat_Action.INVALID
