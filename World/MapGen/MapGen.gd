extends Reference

const UPSCALE = 4
const CHUNK_SIZE = 64

const MAP_CHUNK_SIZE_X = 6
const MAP_CHUNK_SIZE_Y = 4

const MAP_SIZE_X = MAP_CHUNK_SIZE_X * CHUNK_SIZE
const MAP_SIZE_Y = MAP_CHUNK_SIZE_Y * CHUNK_SIZE

const JITTER_PROBABILITY = 0.1
const JITTER_STRENGTH = 0.0085

const CHUNK_SIZE_MIN = 100
const CHUNK_SIZE_MAX = 9600

const LAND_PERCENTAGE = 0.35

const WATER_LEVEL = 4
const ELEVATION_MIN = -4
const ELEVATION_MAX = 8

const HIGH_RISE_PROBABILITY = 0.2
const SINK_PROBABILITY = 0.15
const EROSIAN_PERCENTAGE = 0.6
const EROSIAN_CYCLE = 2

const MAP_BORDER_X = 64
const MAP_BORDER_Y = 48
const REGION_BORDER = 20

const CELLULAR_AUTOMATA_CYCLE = 4
const CELLULAR_AUTOMATA_LIVE = 4
const CELLULAR_AUTOMATA_DEAD = 4
const CELLULAR_AUTOMATA_SMOOTH_ELEVATION_CYCLES = 2

const REGION_COUNT = 1

# climate
const EVAPORATION_FACTOR = 0.5
const CLIMATE_CYCLE = 40
const PRECIPITATION_FACTOR = 0.25
const RUNOFF_FACTOR = 0.25
const SEEPAGE_FACTOR = 0.125
const WIND_STRENGTH = 4.0
const INITIAL_MOISTURE = 0.1

enum Direction { N, NE, E, SE, S, SW, W, NW }

const DIR = "res://World/MapGen/"


class Region:
	var xMin: float
	var xMax: float
	var yMin: float
	var yMax: float


class Tile:
	var idx
	var upscale_factor = 1
	var elevation = 0
	var coordinate: Vector2 setget , _get_coordinate
	var is_land: bool setget , _is_land
	
	
	var search_phase = 0
	var distance
	var search_heuristic
	var next_with_same_priority = null
	var search_priority setget , _get_priority
	
	var clouds: float = 0
	var moisture: float = 0

	func _is_land():
		return elevation >= WATER_LEVEL

	func _get_coordinate():
		return Vector2(idx % (MAP_SIZE_X * upscale_factor), floor(idx / (MAP_SIZE_X * upscale_factor)))

	func _get_priority():
		return distance + search_heuristic

	func _init(_idx: int):
		self.idx = _idx


var height_map: Image
var cloud_map: Image
var moisture_map: Image

var map = []
var search_frontier = PriorityQueue.new()
var search_frontier_phase = 0

var regions: Array

var _max_height_value: float
var _max_cloud_value: float
var _max_moisture_value: float


func _init_map():
	map.resize(MAP_SIZE_X * MAP_SIZE_Y)
	for i in range(0, map.size()):
		map[i] = Tile.new(i)
		map[i].moisture = INITIAL_MOISTURE


func _save_height_map():
	height_map = Image.new()
	height_map.create(MAP_SIZE_X, MAP_SIZE_Y, false, Image.FORMAT_RGB8)
	height_map.lock()
	var idx = 0
	for y in MAP_SIZE_Y:
		for x in MAP_SIZE_X:
			var elevation = map[idx].elevation
			var color: Color
			if elevation < WATER_LEVEL:
				color = Color.aqua
			else:
				var v = range_lerp(elevation, WATER_LEVEL - 1, _max_height_value, 0.15, 1)
				color = Color(v, v, v)
			height_map.set_pixel(x, y, color)
			idx += 1
	height_map.unlock()
	var _result = height_map.save_png(DIR + "height_map.png")
	print("height map saved")


func _get_random_tile(region: Region):
	var x = int(rand_range(region.xMin, region.xMax))
	var y = int(rand_range(region.yMin, region.yMax))
	return map[x + (y * MAP_SIZE_X)]


func raise_terrain(chunk_size: int, budget: int, region: Region):
	search_frontier_phase += 1
	var first_tile: Tile = _get_random_tile(region)
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
		if new_elevation > _max_height_value:
			_max_height_value = new_elevation
		if original_elevation < WATER_LEVEL and new_elevation >= WATER_LEVEL:
			budget -= 1
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
				var max_jitter = max(chunk_size * JITTER_STRENGTH, 3)
				neighbor.search_heuristic = int(rand_range(1,max_jitter)) if randf() < JITTER_PROBABILITY else 0
				search_frontier.enqueue(neighbor, neighbor.search_priority)

	search_frontier.clear()
	return budget


func sink_terrain(chunk_size: int, budget: int, region: Region):
	search_frontier_phase += 1
	var first_tile: Tile = _get_random_tile(region)
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
			budget += 1
		size += 1
		var neighbors = _get_neighbors(current)
		for i in range(0, len(neighbors)):
			var neighbor: Tile = neighbors[i]
			if neighbor and neighbor.search_phase < search_frontier_phase:
				neighbor.search_phase = search_frontier_phase
				var v = neighbor.coordinate
				neighbor.distance = v.distance_to(center)
				var max_jitter = max(chunk_size * JITTER_STRENGTH, 3)
				neighbor.search_heuristic = int(rand_range(1,max_jitter)) if randf() < JITTER_PROBABILITY else 0
				search_frontier.enqueue(neighbor, neighbor.search_priority)

	search_frontier.clear()
	return budget


func _get_neighbors(current: Tile, diagonals = true, _map = null):
	if not _map:
		_map = map
	var neighbors = []
	var coord = current.coordinate
	var N = Vector2(coord.x, coord.y - 1)
	var S = Vector2(coord.x, coord.y + 1)
	var W = Vector2(coord.x - 1, coord.y)
	var E = Vector2(coord.x + 1, coord.y)
	var NE = Vector2(coord.x + 1, coord.y - 1)
	var SE = Vector2(coord.x + 1, coord.y + 1)
	var NW = Vector2(coord.x - 1, coord.y - 1)
	var SW = Vector2(coord.x - 1, coord.y + 1)
	var directions = [N, S, W, E, NE, NW, SE, SW]
	for i in range(len(directions) - (0 if diagonals else 4)):
		var v = directions[i]
		if v.x < 0 or v.x >= MAP_SIZE_X * current.upscale_factor or v.y < 0 or v.y >= MAP_SIZE_Y * current.upscale_factor:
			continue
		neighbors.append(_map[v.x + v.y * MAP_SIZE_X * current.upscale_factor])
	return neighbors


func _get_neighbor(tile: Tile, direction):
	var coord = tile.coordinate
	match direction:
		Direction.N:
			coord.y -= 1
		Direction.NE:
			coord.y -= 1
			coord.x += 1
		Direction.E:
			coord.x += 1
		Direction.SE:
			coord.x += 1
			coord.y += 1
		Direction.S:
			coord.y += 1
		Direction.SW:
			coord.y += 1
			coord.x -= 1
		Direction.W:
			coord.x -= 1
		Direction.NW:
			coord.x -= 1
			coord.y -= 1
	
	if coord.x < 0 or coord.x >= MAP_SIZE_X * tile.upscale_factor or coord.y < 0 or coord.y >= MAP_SIZE_Y * tile.upscale_factor:
		return null
			
	var idx = coord.x + (coord.y * MAP_SIZE_X * tile.upscale_factor)
	if idx < 0 or idx >= len(map):
		return null
	
	return map[idx]

var large_count = 0

func _create_land():
	var land_budget = round(len(map) * LAND_PERCENTAGE)
	var guard = 0
	while guard < 10000:
		guard += 1
		var sink = randf() < SINK_PROBABILITY
		for region in regions:
			
			var chunk_size = int(rand_range(CHUNK_SIZE_MIN, CHUNK_SIZE_MAX if large_count < 2 else CHUNK_SIZE_MAX * 0.75))
			if chunk_size > CHUNK_SIZE_MAX * 0.8:
				large_count += 1
				
			if sink:
				land_budget = sink_terrain(chunk_size, land_budget, region)
			else:
				land_budget = raise_terrain(chunk_size, land_budget, region)
				if land_budget == 0:
					return

	if land_budget > 0:
		push_warning("%s land budget not used!" % str(land_budget))


func _create_regions():
	if not regions:
		regions = []
	else:
		regions.clear()

	match int(clamp(REGION_COUNT, 1, 4)):
		2:
			if MAP_SIZE_X >= MAP_SIZE_Y:
				var region := Region.new()
				region.xMin = MAP_BORDER_X
				region.xMax = MAP_SIZE_X / 2.0 - REGION_BORDER
				region.yMin = MAP_BORDER_Y
				region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
				regions.append(region)
				region = Region.new()
				region.xMin = MAP_SIZE_X / 2.0 + REGION_BORDER
				region.xMax = MAP_SIZE_X - MAP_BORDER_X
				region.yMin = MAP_BORDER_Y
				region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
				regions.append(region)
			else:
				var region := Region.new()
				region.xMin = MAP_BORDER_X
				region.xMax = MAP_SIZE_X - MAP_BORDER_X
				region.yMin = MAP_BORDER_Y
				region.yMax = MAP_SIZE_Y / 2.0 - REGION_BORDER
				regions.append(region)
				region = Region.new()
				region.xMin = MAP_BORDER_X
				region.xMax = MAP_SIZE_X - MAP_BORDER_X
				region.yMin = MAP_SIZE_Y / 2.0 + REGION_BORDER
				region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
				regions.append(region)
		3:
			var region := Region.new()
			region.xMin = MAP_BORDER_X
			region.xMax = MAP_SIZE_X / 3.0 - REGION_BORDER
			region.yMin = MAP_BORDER_Y
			region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
			regions.append(region)
			region = Region.new()
			region.xMin = MAP_SIZE_Y / 3.0 + REGION_BORDER
			region.xMax = MAP_SIZE_X * 2.0 / 3.0 - REGION_BORDER
			region.yMin = MAP_BORDER_Y
			region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
			regions.append(region)
			region = Region.new()
			region.xMin = MAP_SIZE_X * 2.0 / 3.0 + REGION_BORDER
			region.xMax = MAP_SIZE_X - MAP_BORDER_X
			region.yMin = MAP_BORDER_Y
			region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
			regions.append(region)
		4:
			var region := Region.new()
			region.xMin = MAP_BORDER_X
			region.xMax = MAP_SIZE_X / 2.0 - REGION_BORDER
			region.yMin = MAP_BORDER_Y
			region.yMax = MAP_SIZE_Y / 2.0 - REGION_BORDER
			regions.append(region)
			region = Region.new()
			region.xMin = MAP_SIZE_X / 2.0 + REGION_BORDER
			region.xMax = MAP_SIZE_X - MAP_BORDER_X
			region.yMin = MAP_BORDER_Y
			region.yMax = MAP_SIZE_Y / 2.0 - REGION_BORDER
			regions.append(region)
			region = Region.new()
			region.xMin = MAP_SIZE_X / 2.0 + REGION_BORDER
			region.xMax = MAP_SIZE_X - MAP_BORDER_X
			region.yMin = MAP_SIZE_Y / 2.0 + REGION_BORDER
			region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
			regions.append(region)
			region = Region.new()
			region.xMin = MAP_BORDER_X
			region.xMax = MAP_SIZE_X / 2.0 - REGION_BORDER
			region.yMin = MAP_SIZE_Y / 2.0 + REGION_BORDER
			region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
			regions.append(region)
		_:
			var region := Region.new()
			region.xMin = MAP_BORDER_X
			region.xMax = MAP_SIZE_X - MAP_BORDER_X
			region.yMin = MAP_BORDER_Y
			region.yMax = MAP_SIZE_Y - MAP_BORDER_Y
			regions.append(region)


func _is_erodibe(tile: Tile):
	var erodible_elevation = tile.elevation - 2
	var neighbors = _get_neighbors(tile, false)
	for neighbor in neighbors:
		if neighbor and neighbor.elevation <= erodible_elevation:
			return true
	return false


func _get_erosion_target(tile: Tile):
	var candidates = []
	var erodible_elevation = tile.elevation - 1
	var neighbors = _get_neighbors(tile, false)
	for neighbor in neighbors:
		if neighbor and neighbor.elevation <= erodible_elevation:
			candidates.append(neighbor)

	var target = candidates[int(rand_range(0, len(candidates)))]
	return target


func _erode_land():
	for _cycle in range(EROSIAN_CYCLE):
		var erodible_tiles = []
		for tile in map:
			if _is_erodibe(tile):
				erodible_tiles.append(tile)
		var target_erodible_count = int(len(erodible_tiles) * (1.0 - EROSIAN_PERCENTAGE))
	
		while len(erodible_tiles) > target_erodible_count:
			var index = randi() % len(erodible_tiles)
			var tile = erodible_tiles[index]
			var target = _get_erosion_target(tile)
			tile.elevation -= 1
			target.elevation += 1
			if not _is_erodibe(tile):
				erodible_tiles.remove(index)
	
			for neighbor in _get_neighbors(tile, false):
				if (
					neighbor
					and neighbor.elevation == tile.elevation + 2
					and not erodible_tiles.has(neighbor)
				):
					erodible_tiles.append(neighbor)
	
			if _is_erodibe(target) and not erodible_tiles.has(target):
				erodible_tiles.append(target)
	
			for neighbor in _get_neighbors(target, false):
				if (
					neighbor
					and neighbor != tile
					and neighbor.elevation == target.elevation + 1
					and not _is_erodibe(neighbor)
				):
					erodible_tiles.erase(neighbor)


func _smooth_land():
	for _cycle in range(0, CELLULAR_AUTOMATA_CYCLE):
		var next = map.duplicate()
		for tile in next:
			var land_count = 0
			var avg_elevation = 0

			for neighbor in _get_neighbors(tile):
				if not neighbor.is_land:
					continue
				land_count += 1
				avg_elevation += neighbor.elevation

			if (
				(not tile.is_land or _cycle < CELLULAR_AUTOMATA_SMOOTH_ELEVATION_CYCLES)
				and land_count > CELLULAR_AUTOMATA_LIVE
			):
				tile.elevation = int(avg_elevation / land_count)
			if tile.is_land and land_count < CELLULAR_AUTOMATA_DEAD:
				tile.elevation = WATER_LEVEL - 1

		map = next

func _remove_lone_pillars():
	var next = map.duplicate()
	for tile in next:
		if not tile.is_land:
			continue
		var lower_elevation_count = 0
		var neighbor_elevations = {}
		for neighbor in _get_neighbors(tile):
			if neighbor.elevation < tile.elevation:
				lower_elevation_count += 1
				if not neighbor_elevations.has(neighbor.elevation):
					neighbor_elevations[neighbor.elevation] = 0
				neighbor_elevations[neighbor.elevation] += 1
		if lower_elevation_count >= 7:
			for elevation in neighbor_elevations:
				if neighbor_elevations[elevation] == neighbor_elevations.values().max():
					tile.elevation = elevation
					print("removing lone pillar at %s" % str(tile.coordinate))
					break
	map = next

func _evolve_climate(tile: Tile, _map):
	if not tile.is_land:
		tile.moisture = 1
		tile.clouds += EVAPORATION_FACTOR
	else:
		var evaporation = tile.moisture * EVAPORATION_FACTOR
		tile.moisture -= evaporation
		tile.clouds += evaporation

	var precipitation = tile.clouds * PRECIPITATION_FACTOR
	tile.clouds -= precipitation
	tile.moisture += precipitation
	var cloud_maximum = 1 - (tile.elevation - WATER_LEVEL) / (ELEVATION_MAX + 1)

	if tile.clouds > cloud_maximum:
		tile.moisture += tile.clouds - cloud_maximum
		tile.clouds = cloud_maximum

	var cloud_dispersal = tile.clouds * (1 / 8)
	var runoff = tile.moisture * RUNOFF_FACTOR * (1 / 8)
	var seepage = tile.moisture * SEEPAGE_FACTOR * (1 / 8)
	for neighbor in _get_neighbors(tile, true, _map):
		if not neighbor:
			continue
		neighbor.clouds += cloud_dispersal
		var elevation_delta = neighbor.elevation - tile.elevation - WATER_LEVEL
		if elevation_delta > 0:
			tile.moisture -= runoff
			neighbor.moisture += runoff
		elif elevation_delta == 0:
			tile.moisture -= seepage
			neighbor.moisture += seepage
	tile.clouds = 0


func _create_climate():
	for _cycle in range(CLIMATE_CYCLE):
		var _map = map.duplicate()
		for tile in _map:
			_evolve_climate(tile, _map)
		map = _map.duplicate()
		_map.clear()


func _save_cloud_map():
	for tile in map:
		if tile.clouds > _max_cloud_value:
			_max_moisture_value = tile.moisture

	cloud_map = Image.new()
	cloud_map.create(MAP_SIZE_X, MAP_SIZE_Y, false, Image.FORMAT_RGB8)
	cloud_map.lock()
	var idx = 0
	for y in MAP_SIZE_Y:
		for x in MAP_SIZE_X:
			var tile = map[idx]
			var v = range_lerp(tile.clouds, 0, _max_cloud_value, 0, 0.9)
			var color = Color(v, v, v)
			cloud_map.set_pixel(x, y, color)
			idx += 1

	cloud_map.unlock()
	var _result = cloud_map.save_png(DIR + "cloud_map.png")
	print("cloud_map saved")


func _save_moisture_map():
	for tile in map:
		if tile.moisture > _max_moisture_value:
			_max_moisture_value = tile.moisture
	moisture_map = Image.new()
	moisture_map.create(MAP_SIZE_X, MAP_SIZE_Y, false, Image.FORMAT_RGB8)
	moisture_map.lock()
	var idx = 0
	for y in MAP_SIZE_Y:
		for x in MAP_SIZE_X:
			var tile = map[idx]
			var v = range_lerp(tile.moisture, 0, _max_moisture_value, 0, 1)
			var color = Color(v, v, v)
			moisture_map.set_pixel(x, y, color)
			idx += 1

	moisture_map.unlock()
	var _result = moisture_map.save_png(DIR + "moisture_map.png")
	print("moisture_map saved")


func _print_prop():
	print("----- Generator Properties ------")
	print("map size = {}x{}".format([MAP_SIZE_X, MAP_SIZE_Y], "{}") )
	print("upscaled map size = {}x{}".format([MAP_SIZE_X * UPSCALE, MAP_SIZE_Y * UPSCALE], "{}") )
	print("chunk size = {}-{}".format([CHUNK_SIZE_MIN, CHUNK_SIZE_MAX], "{}"))
	print("elevation = ({})-({})-({})".format([ELEVATION_MIN, WATER_LEVEL, ELEVATION_MAX], "{}"))
	print("land percentage = %s" % str(LAND_PERCENTAGE))
	print("map border = x: {} , y: {}".format([MAP_BORDER_X, MAP_BORDER_Y], "{}"))
	print("---------------------------------")

func generate_map(world):
	_print_prop()
	_run()
	for item in map:
		var tile: Tile = item
		if not tile.is_land:
			world.add_water_tile(tile.coordinate)
		else:
			world.add_grass_tile(tile.coordinate, tile.elevation)

			for neighbor in _get_neighbors(tile):
				if neighbor.is_land and neighbor.elevation < tile.elevation:
					world.add_grass_tile(tile.coordinate, neighbor.elevation)
			
			var elevation_above_water = tile.elevation - WATER_LEVEL
			if elevation_above_water > 0:
				var neighbor = _get_neighbor(tile, Direction.S)
				if neighbor and neighbor.elevation < tile.elevation:
					world.add_cliff_tile(neighbor.coordinate, tile.elevation)

func _upscale_map():
	if UPSCALE == 1:
		return
		
	var upscaled = []
	upscaled.resize(len(map) * pow(UPSCALE, 2))
	for i in range(len(upscaled)):
		var x = i % (MAP_SIZE_X  * UPSCALE)
		var y = floor(i / (MAP_SIZE_X  * UPSCALE))
		var xi = floor(x/UPSCALE)
		var yi = floor(y/UPSCALE)
		var idx = xi + (yi * MAP_SIZE_X)
		var original_tile = map[idx]
		var tile = Tile.new(i)
		tile.upscale_factor = UPSCALE
		tile.elevation = original_tile.elevation
		upscaled[i] = tile
		
	map = upscaled

func _run():
	_init_map()
	_create_regions()
	_create_land()
	_erode_land()
	_smooth_land()
	_remove_lone_pillars()
	_upscale_map()
	# _save_height_map()
	# _create_climate()
	# _save_cloud_map()
	# _save_moisture_map()
