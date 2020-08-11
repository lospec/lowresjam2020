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
const ANIM_START_FRAME = {
	Animations.IDLE_DOWN: 0,
	Animations.IDLE_UP: 1,
	Animations.IDLE_SIDE: 2,
	Animations.RUN_DOWN: 6,
	Animations.RUN_SIDE: 12,
	Animations.RUN_UP: 18,
}
const ANIM_FRAME_LENGTH = {
	Animations.IDLE_DOWN: 1,
	Animations.IDLE_UP: 1,
	Animations.IDLE_SIDE: 1,
	Animations.RUN_DOWN: 6,
	Animations.RUN_SIDE: 6,
	Animations.RUN_UP: 6,
}
const ANIM_FPS_SPEED = {
	Animations.IDLE_DOWN: 10,
	Animations.IDLE_UP: 10,
	Animations.IDLE_SIDE: 10,
	Animations.RUN_DOWN: 10,
	Animations.RUN_SIDE: 8,
	Animations.RUN_UP: 10,
}

# Exported Variables
export (int) var max_health = 10
export (float) var move_speed = 10

# Public Variables
var health: int setget set_health
var velocity = Vector2()
var current_anim = Animations.IDLE_DOWN
var anim_frame = 0

# Onready Variables
onready var sprite = $Sprite
onready var next_frame_timer = $NextFrameTimer
onready var anim_wait_time = {
	Animations.IDLE_DOWN: calculate_wait_time(ANIM_FPS_SPEED[Animations.IDLE_DOWN]),
	Animations.IDLE_UP: calculate_wait_time(ANIM_FPS_SPEED[Animations.IDLE_UP]),
	Animations.IDLE_SIDE: calculate_wait_time(ANIM_FPS_SPEED[Animations.IDLE_SIDE]),
	Animations.RUN_DOWN: calculate_wait_time(ANIM_FPS_SPEED[Animations.RUN_DOWN]),
	Animations.RUN_SIDE: calculate_wait_time(ANIM_FPS_SPEED[Animations.RUN_SIDE]),
	Animations.RUN_UP: calculate_wait_time(ANIM_FPS_SPEED[Animations.RUN_UP]),
}


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
		anim_frame = 0
	
	# Set animation speed
	next_frame_timer.wait_time = anim_wait_time[current_anim]
	
	# Set frames
	var anim_start_frame = ANIM_START_FRAME[current_anim]
	sprite.frame = anim_start_frame + anim_frame


func _on_NextFrameTimer_timeout():
	var anim_length = ANIM_FRAME_LENGTH[current_anim]
	anim_frame = (sprite.frame + 1) % anim_length


func set_health(value):
	var old_health = health
	health = value
	emit_signal("health_changed", old_health, health)
