extends KinematicBody2D

# Constants
const SPEED = 20

# Exported Variables
export (int) var max_health
export (Resource) var entity_resource

# Public Variables
var in_game_name: String
var health: int
var velocity = Vector2()
var velocity_leftover = Vector2()

# Onready Variables
onready var animated_sprite = $AnimatedSprite
onready var animation_tree = $AnimationTree
onready var playback = $AnimationTree.get("parameters/playback")
onready var run_blend_tree = $AnimationTree.get("parameters/Run/blend_position")

func _ready():
	entity_resource.apply(self)
	if max_health <= 0:
		return
	health = max_health
	playback.start("Run")


func _process(_delta):
	if velocity == Vector2.ZERO:
		playback.travel("Idle")
	else:
		playback.travel("Run")
		animation_tree.set("parameters/Run/blend_position", velocity.normalized())


func _physics_process(_delta):
	velocity = move_and_slide(velocity)
