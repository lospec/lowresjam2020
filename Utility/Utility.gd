static func getPixellizedPos(pos):
	pos.x = round(pos.x)
	pos.y = round(pos.y)
	
	return pos

static func randomRange(minim, maxim):
	randomize()
	return randi() % (int(maxim - minim)) + int(minim)
