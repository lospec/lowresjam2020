extends KinematicBody2D

# Constants
const SPEED = 20

# Exported Variables
export (int) var max_health
export (Resource) var entity_resource

# Public Variables
var current_health: int
var velocity = Vector2()
var velocity_leftover = Vector2()

onready var animated_sprite = $AnimatedSprite
onready var animation_tree = $AnimationTree
onready var playback = $AnimationTree.get("parameters/playback")
onready var run_blend_tree = $AnimationTree.get("parameters/Run/blend_position")



func _init():
	self.animated_sprite = animated_sprite


func _ready():
	entity_resource.apply(self)
	if max_health <= 0:
		return
	current_health = max_health
	playback.start("Run")


func _process(_delta):
	pass
	if velocity == Vector2.ZERO:
		playback.travel("Idle")
	else:
		playback.travel("Run")
		run_blend_tree = velocity.normalized()


func _physics_process(_delta):
	velocity = move_and_slide(velocity)
