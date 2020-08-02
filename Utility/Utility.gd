extends Node

static func randomRange(minim, maxim):
	randomize()
	return randi() % (int(maxim - minim)) + int(minim)
