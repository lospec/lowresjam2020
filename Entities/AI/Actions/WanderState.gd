extends StateAction
class_name WanderAction

export (float) var speed = 10


func perform(stateMachine):
	var move = Vector2()
	move.x = rand_range(-1, 1)
	move.y = rand_range(-1, 1)
	move = move.normalized() * speed
	stateMachine.entity.velocity = move
