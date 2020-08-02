extends StateAction
class_name WanderAction

export(String) var place

func perform(stateMachine):
	print("IS WANDERING to %s" % place)

