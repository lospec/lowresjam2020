extends KinematicBody2D
class_name BaseEntity

# Signals
signal health_changed(old_health, new_health)

# Constants
const SPEED = 20
const MIN_SPEED_FOR_RUN_ANIM = 0
enum Animations {
	IDLE_DOWN,
	IDLE_UP,
	IDLE_SIDE,
	RUN_DOWN,
	RUN_SIDE,
	RUN_UP,
}
const DIR_TO_ANIM = {
	Vector2.DOWN: Animations.RUN_DOWN,
	Vector2.UP: Animations.RUN_UP,
	Vector2.RIGHT: Animations.RUN_SIDE,
	Vector2.LEFT: Animations.RUN_SIDE,
}
const RUN_ANIM_TO_IDLE_ANIM = {
	Animations.RUN_DOWN: Animations.IDLE_DOWN,
	Animations.RUN_UP: Animations.IDLE_UP,
	Animations.RUN_SIDE: Animations.IDLE_SIDE,
}
const IDLE_ANIMS = [
	Animations.IDLE_DOWN,
	Animations.IDLE_UP,
	Animations.IDLE_SIDE,
]

# Exported Variables
export (int) var max_health = 10

# Public Variables
var health: int setget set_health
var velocity = Vector2()
var current_anim = Animations.IDLE_DOWN
var anim_frame = 0

# Onready Variables
onready var sprite = $Sprite
onready var animation_player = $AnimationPlayer


func _ready():
	if max_health <= 0:
		return
	health = max_health


func _physics_process(_delta):
	movement()
	animate()


func movement():
	velocity = move_and_slide(velocity)


func calculate_wait_time(fps):
	return 1.0/fps


func animate():
	if velocity.length() > 0:
		sprite.flip_h = velocity.x < 0
	
	# Get current animation
	var old_anim = current_anim
	
	if velocity.length() > MIN_SPEED_FOR_RUN_ANIM: # Running
		var anim_dir = velocity.normalized()
		# If running diagonally, choose vertical run anim
		if abs(anim_dir.x) > MIN_SPEED_FOR_RUN_ANIM and abs(anim_dir.y) > MIN_SPEED_FOR_RUN_ANIM:
			anim_dir = Vector2(0, anim_dir.y).normalized()
		current_anim = DIR_TO_ANIM[anim_dir]
	elif not current_anim in IDLE_ANIMS:
		current_anim = RUN_ANIM_TO_IDLE_ANIM[current_anim]
	
	# If a change in animation has taken place
	if current_anim != old_anim:
		animation_player.play(Animations.keys()[current_anim].to_lower())


func set_health(value):
	var old_health = health
	health = value
	emit_signal("health_changed", old_health, health)
