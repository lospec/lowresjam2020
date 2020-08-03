extends Node

func _ready():
	randomize()

static func randomRange(minim, maxim):
	return randi() % (int(maxim - minim)) + int(minim)

static func rand_element(arr):
	return arr[randi() % arr.size()]

static func rand_bool() -> bool:
	return bool(randi() % 2)
