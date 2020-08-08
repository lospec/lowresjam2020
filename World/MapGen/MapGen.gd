tool
extends EditorScript

const CHUNK_SIZE = 4
const MAP_CHUNK_SIZE_X = 24
const MAP_CHUNK_SIZE_Y = 16

const MAP_SIZE_X = MAP_CHUNK_SIZE_X * CHUNK_SIZE 
const MAP_SIZE_Y = MAP_CHUNK_SIZE_Y * CHUNK_SIZE

const DIR = "res://World/MapGen/"

const JITTER_PROBABILITY = 0.25
const CHUNK_SIZE_MIN = 30
const CHUNK_SIZE_MAX = 200
const LAND_PERCENTAGE = 0.4
const WATER_LEVEL = 3
const HIGH_RISE_PROBABILITY = 0.25
const SINK_PROBABILITY = 0.2
const ELEVATION_MIN = -2
const ELEVATION_MAX = 8

const MAP_BORDER_X = 15
const MAP_BORDER_Y = 5

var xMin
var xMax
var yMin
var yMax

class Tile:
	var idx
	var value
	var search_phase = 0
	var distance
	var search_heuristic
	var search_priority setget ,_get_priority
	var coordinate: Vector2 setget ,_get_coordinate
	var next_with_same_priority = null
	var elevation = 0
	var water_level = WATER_LEVEL
	
	func _to_string():
		return str(distance)
	
	func _get_coordinate():
		return Vector2(idx % MAP_SIZE_X, floor(idx / MAP_SIZE_X))
	func _get_priority():
		return distance + search_heuristic
		
	
	func _init(idx: int, value = null):
		self.idx = idx
		self.value = value


var image: Image

var map = []
var search_frontier = PriorityQueue.new()
var search_frontier_phase = 0

func _init_map():
	print("image initializing")
	image = Image.new()
	image.create(MAP_SIZE_X, MAP_SIZE_Y, false , Image.FORMAT_RGB8)
	map.resize(MAP_SIZE_X*MAP_SIZE_Y)
	
	for i in range(0, map.size()):
		map[i] = Tile.new(i, 0)
	
func _save():
	image.lock()
	var idx = 0
	for y in MAP_SIZE_Y:
		for x in MAP_SIZE_X:
			var elevation = map[idx].elevation
			var color: Color
			if elevation < WATER_LEVEL:
				color = Color.aqua
			else:
				var v = elevation * 0.1
				color = Color(v,v,v)
			image.set_pixel(x,y, color)
			idx += 1
	image.unlock()
	image.save_png(DIR + "test.png")
	print("image saved")
	
func _get_random_tile():
	var x = int(rand_range(xMin, xMax))
	var y = int(rand_range(yMin, yMax))
	return map[x + (y * MAP_SIZE_X)]
	
func raise_terrain(chunk_size: int, budget: int):
	search_frontier_phase += 1
	var first_tile: Tile = _get_random_tile()
	first_tile.search_phase = search_frontier_phase
	first_tile.distance = 0
	first_tile.search_heuristic = 0
	search_frontier.enqueue(first_tile, first_tile.search_priority)
	var center = first_tile.coordinate
	var rise = 2 if randf() < HIGH_RISE_PROBABILITY else 1
	var size = 0
	while size < chunk_size and search_frontier.count > 0:
		var current: Tile = search_frontier.dequeue()
		var original_elevation = current.elevation
		var new_elevation = original_elevation + rise
		if new_elevation > ELEVATION_MAX:
			continue
		current.elevation = new_elevation
		if original_elevation < WATER_LEVEL and new_elevation >= WATER_LEVEL:
			budget-=1
			if budget == 0:
				break
		size += 1
		var neighbors = _get_neighbors(current)
		for i in range(0, len(neighbors)):
			var neighbor: Tile = neighbors[i]
			if neighbor and neighbor.search_phase < search_frontier_phase:
				neighbor.search_phase = search_frontier_phase
				var v = neighbor.coordinate
				neighbor.distance = v.distance_to(center)
				neighbor.search_heuristic = 1 if randf() < JITTER_PROBABILITY else 0
				search_frontier.enqueue(neighbor, neighbor.search_priority)
				
	search_frontier.clear()
	return budget
	
func sink_terrain(chunk_size: int, budget: int):
	search_frontier_phase += 1
	var first_tile: Tile = _get_random_tile()
	first_tile.search_phase = search_frontier_phase
	first_tile.distance = 0
	first_tile.search_heuristic = 0
	search_frontier.enqueue(first_tile, first_tile.search_priority)
	var center = first_tile.coordinate
	var sink = 2 if randf() < SINK_PROBABILITY else 1
	var size = 0
	while size < chunk_size and search_frontier.count > 0:
		var current: Tile = search_frontier.dequeue()
		var original_elevation = current.elevation
		var new_elevation = current.elevation - sink
		if new_elevation < ELEVATION_MIN:
			continue
		current.elevation = new_elevation
		if original_elevation >= WATER_LEVEL and new_elevation < WATER_LEVEL:
			budget+=1
		size += 1
		var neighbors = _get_neighbors(current)
		for i in range(0, len(neighbors)):
			var neighbor: Tile = neighbors[i]
			if neighbor and neighbor.search_phase < search_frontier_phase:
				neighbor.search_phase = search_frontier_phase
				var v = neighbor.coordinate
				neighbor.distance = v.distance_to(center)
				neighbor.search_heuristic = 1 if randf() < JITTER_PROBABILITY else 0
				search_frontier.enqueue(neighbor, neighbor.search_priority)
				
	search_frontier.clear()
	return budget

func _get_neighbors(current: Tile):
	var neighbors = []
	var coord = current.coordinate
	var N = Vector2(coord.x, coord.y-1)
	var S = Vector2(coord.x, coord.y+1)
	var W = Vector2(coord.x-1, coord.y)
	var E = Vector2(coord.x+1, coord.y)
	var NE = Vector2(coord.x+1, coord.y-1)
	var SE = Vector2(coord.x+1, coord.y+1)
	var NW = Vector2(coord.x-1, coord.y-1)
	var SW = Vector2(coord.x-1, coord.y+1)
	for v in [N,S,W,E,NW,NE,SW,SE]:
		if v.x < 0 or v.x >= MAP_SIZE_X or v.y < 0 or v.y >= MAP_SIZE_Y:
			continue
		neighbors.append(map[v.x + v.y * MAP_SIZE_X]) 
	return neighbors
				
func _create_land():
	var land_budget = round(len(map) * LAND_PERCENTAGE)
	while land_budget > 0:
		var chunk_size = CHUNK_SIZE_MIN + (randi() % CHUNK_SIZE_MAX)
		if randf() < SINK_PROBABILITY:
			land_budget = sink_terrain(chunk_size, land_budget)
		else:
			land_budget = raise_terrain(chunk_size, land_budget)

func _run():
	_init_map()
	xMin = MAP_BORDER_X
	xMax = MAP_SIZE_X - MAP_BORDER_X
	yMin = MAP_BORDER_Y
	yMax = MAP_SIZE_Y - MAP_BORDER_Y
	_create_land()
	_save()
