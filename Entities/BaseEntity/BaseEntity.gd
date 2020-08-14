extends KinematicBody2D
class_name BaseEntity

# Signals
signal health_changed(old_health, new_health)

# Constants
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
export (float) var move_speed = 10

# Public Variables
var health: int setget set_health
var velocity = Vector2()
var current_anim = Animations.IDLE_DOWN
var anim_frame = 0
var status_effects = {}

# Private Variables
var _x = position.x
var _y = position.y
var _old_x = position.x
var _old_y = position.y

# Onready Variables
onready var sprite = $Sprite
onready var animation_player = $AnimationPlayer

func add_status_effect(script_name: String):
	var se = StatEffectUtility.get_status_effect(script_name)
	
	# HARDCODED INTERACTION: Should make a system to make this more flexible
	if script_name == "OnFire" and status_effects.has("Frozen"):
		status_effects.erase("Frozen")
		return
		
	if script_name == "Frozen" and status_effects.has("OnFire"):
		status_effects.erase("Frozen")
		return
	
	# Overwrite effect if exist, Should probably make stacking system if needed
	if status_effects.has(se.name):
		status_effects.erase(se.name)
	
	status_effects[se.name] = se

func _ready():
	if max_health <= 0:
		return
	health = max_health


func _physics_process(_delta):
	movement()
	animate()


func movement():
	_old_x = position.x
	_old_y = position.y
	
	velocity = move_and_slide(velocity)
	
	# Prevent diagonal jittering
	# Credit to:
	# https://www.reddit.com/r/godot/comments/cvn6qn/ive_figured_out_a_way_to_smooth_out_jittery/
	if abs(_old_x - position.x) > abs(_old_y - position.y) and velocity.x: 
		_x = round(position.x)
		_y = round(position.y + (_x - position.x) * velocity.y / velocity.x)
		position.y = _y
	elif abs(_old_x - position.x) <= abs(_old_y - position.y) and velocity.y:
		_y = round(position.y)
		_x = round(position.x + (_y - position.y) * velocity.x / velocity.y)
		position.x = _x


func calculate_wait_time(fps):
	return 1.0 / fps


func animate():
	if velocity.length() > 0:
		sprite.flip_h = velocity.x < 0

	# Get current animation
	var old_anim = current_anim

	if velocity.length() > MIN_SPEED_FOR_RUN_ANIM:  # Running
		var anim_dir = velocity.normalized()
		
		if abs(anim_dir.x) > MIN_SPEED_FOR_RUN_ANIM and abs(anim_dir.y) > MIN_SPEED_FOR_RUN_ANIM:
			anim_dir = (Vector2(0, anim_dir.y) if abs(anim_dir.y) >= abs(anim_dir.x) else Vector2(anim_dir.x, 0)).normalized()
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
