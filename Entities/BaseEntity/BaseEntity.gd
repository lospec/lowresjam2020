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

onready var sprite = $Sprite

func _ready():
	entity_resource.apply(self)
	if max_health <= 0:
		return
	health = max_health

func _physics_process(_delta):
	velocity = move_and_slide(velocity)
