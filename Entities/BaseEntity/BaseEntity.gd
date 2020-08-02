extends KinematicBody2D

# constants
const SPEED = 20

# Public Variables
var velocity = Vector2()
var velocity_leftover = Vector2()

export (int) var maxHealth
var currentHealth: int

func _init():
	self.velocity = velocity

func _ready():
	if maxHealth <= 0:
		return
	currentHealth = maxHealth


func _physics_process(_delta):
	velocity = move_and_slide(velocity)
