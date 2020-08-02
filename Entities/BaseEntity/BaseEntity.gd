extends KinematicBody2D

# Constants
const SPEED = 20

# Exported Variables
export(int) var max_health

# Public Variables
var current_health: int
var velocity = Vector2()
var velocity_leftover = Vector2()

func _init():
	self.velocity = velocity

func _ready():
	if max_health <= 0:
		return
	current_health = max_health

func _physics_process(_delta):
	velocity = move_and_slide(velocity)
