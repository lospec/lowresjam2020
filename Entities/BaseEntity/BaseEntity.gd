extends KinematicBody2D

export(int) var maxHealth
var currentHealth : int


func _ready():
	if maxHealth <= 0:
		return
	currentHealth = maxHealth
