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
# velocity is just input velocity (i.e. the velocity that the entity wants to use)
# and real_velocity is what it is actually using
# (E.g. if the entity collides then it will be 0, 0)
var velocity = Vector2()
var real_velocity = Vector2()
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
	
	real_velocity = move_and_slide(velocity)
	
	# Fixes some collision issues
	var slide_count = get_slide_count()
	if slide_count >= 2:
		var normal = Vector2.ZERO
		for i in slide_count:
			var collision = get_slide_collision(i)
			normal += collision.normal
		
		if abs(normal.x) > 0.3 and abs(normal.y) > 0.3:
			if sign(velocity.x) == sign(-normal.x):
				position.x = round(position.x)
			if sign(velocity.y) == sign(-normal.y):
				position.y = round(position.y)
	
	# Prevent diagonal jittering
	# Credit to:
	# https://www.reddit.com/r/godot/comments/cvn6qn/ive_figured_out_a_way_to_smooth_out_jittery/
	# Disables diagonal jitter fix if colliding -
	# This is so hacky it's painful
	if abs(real_velocity.x) > 0 and abs(real_velocity.y) > 0 and \
		not (get("collision_detector") and \
		get("collision_detector").get_overlapping_bodies()):
		if abs(_old_x - position.x) > abs(_old_y - position.y):
			_x = round(position.x)
			_y = round(position.y + (_x - position.x) * real_velocity.y / real_velocity.x)
			position.y = _y
		elif abs(_old_x - position.x) <= abs(_old_y - position.y):
			_y = round(position.y)
			_x = round(position.x + (_y - position.y) * real_velocity.x / real_velocity.y)
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


# Animation : play footstep sound
func anim_play_footstep (sfx_offset: int):
	if is_in_group("PlayerGroup"):
		var _a = AudioSystem.play_sfx(AudioSystem.SFX.FOOTSTEP_1 + sfx_offset, null, -35)
