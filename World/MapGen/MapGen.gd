extends Reference

const UPSCALE = 4
const CHUNK_SIZE = 64

const MAP_CHUNK_SIZE_X = 3
const MAP_CHUNK_SIZE_Y = 2

const MAP_SIZE_X = MAP_CHUNK_SIZE_X * CHUNK_SIZE
const MAP_SIZE_Y = MAP_CHUNK_SIZE_Y * CHUNK_SIZE

const JITTER_PROBABILITY = 0.15
const JITTER_STRENGTH = 0.013

const CHUNK_SIZE_MIN = 300
const CHUNK_SIZE_MAX = 2800
const LAND_PERCENTAGE = 0.6

const WATER_LEVEL = 5
const ELEVATION_MIN = -3
const ELEVATION_MAX = 10

const HIGH_RISE_STRENGTH = 1
const HIGH_RISE_PROBABILITY = 0.1
const SINK_PROBABILITY = 0.05
const EROSIAN_PERCENTAGE = 0.85
const EROSIAN_CYCLE = 8

const MAP_BORDER_X = 32
const MAP_BORDER_Y = 16
const REGION_BORDER = 28

const CELLULAR_AUTOMATA_CYCLE = 3
const CELLULAR_AUTOMATA_LIVE = 5
const CELLULAR_AUTOMATA_DEAD = 3
const CELLULAR_AUTOMATA_SMOOTH_ELEVATION_CYCLES = 2

const REGION_COUNT = 3

enum Direction { N, NE, E, SE, S, SW, W, NW }

const Directions = [
	Direction.N,
	Direction.NE,
	Direction.E,
	Direction.SE,
	Direction.S,
	Direction.SW,
	Direction.W,
	Direction.NW,
]
const MainDirections = [
	Direction.N,
	Direction.E,
	Direction.S,
	Direction.W,
]

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
	var over_water_elevation setget , _get_over_water_elevation
	var is_near_water = false
	var is_cliff = false
	var is_water_cliff = false
	var is_cliff_corner = false
	var feature_type = -1

	var search_phase = 0
	var distance
	var search_heuristic
	var next_with_same_priority = null
	var search_priority setget , _get_priority
	var climate_data

	func _get_over_water_elevation():
		return elevation if elevation >= WATER_LEVEL else WATER_LEVEL

	func _is_land():
		return elevation > WATER_LEVEL

	func _get_coordinate():
		return Vector2(
			idx % (MAP_SIZE_X * upscale_factor), floor(idx / (MAP_SIZE_X * upscale_factor))
		)

	func _get_priority():
		return distance + search_heuristic

	func _init(_idx: int):
		self.idx = _idx


var map = []
var search_frontier = PriorityQueue.new()
var search_frontier_phase = 0

var regions: Array


func _init_map():
	map.resize(MAP_SIZE_X * MAP_SIZE_Y)
	for i in range(0, map.size()):
		map[i] = Tile.new(i)


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
	var rise = (
		int(rand_range(2, 1 + (1 if HIGH_RISE_STRENGTH < 1 else HIGH_RISE_STRENGTH)))
		if randf() < HIGH_RISE_PROBABILITY
		else 1
	)
	var size = 0
	while size < chunk_size and search_frontier.count > 0:
		var current: Tile = search_frontier.dequeue()
		var original_elevation = current.elevation
		var new_elevation = original_elevation + rise
		if new_elevation > ELEVATION_MAX:
			continue
		current.elevation = new_elevation

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
				neighbor.search_heuristic = (
					int(rand_range(1, max_jitter))
					if randf() < JITTER_PROBABILITY
					else 0
				)
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
				neighbor.search_heuristic = (
					int(rand_range(1, max_jitter))
					if randf() < JITTER_PROBABILITY
					else 0
				)
				search_frontier.enqueue(neighbor, neighbor.search_priority)

	search_frontier.clear()
	return budget


func _get_neighbors(current: Tile, outer = 1, inner = 0):
	var neighbors = []
	
	for j in range(-(outer - inner), (outer - inner) + 1):
		for i in range(-outer, outer + 1):
			var v = Vector2(i, j)
			if i == 0 and j == 0:
				continue
			v += current.coordinate
			
			if (
				v.x < 0 or v.y < 0 
				or v.x >= MAP_SIZE_X * current.upscale_factor
				or v.y >= MAP_SIZE_Y * current.upscale_factor
				
			):
				continue
			neighbors.append(map[v.x + v.y * MAP_SIZE_X * current.upscale_factor])	
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

	if (
		coord.x < 0
		or coord.x >= MAP_SIZE_X * tile.upscale_factor
		or coord.y < 0
		or coord.y >= MAP_SIZE_Y * tile.upscale_factor
	):
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
			var chunk_size = int(
				rand_range(
					CHUNK_SIZE_MIN, CHUNK_SIZE_MAX if large_count < 2 else CHUNK_SIZE_MAX * 0.75
				)
			)
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
	var neighbors = _get_neighbors(tile)
	for neighbor in neighbors:
		if neighbor and neighbor.elevation <= erodible_elevation:
			return true
	return false


func _get_erosion_target(tile: Tile):
	var candidates = []
	var erodible_elevation = tile.elevation - 1
	var neighbors = _get_neighbors(tile)
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

			for neighbor in _get_neighbors(tile):
				if (
					neighbor
					and neighbor.elevation == tile.elevation + 2
					and not erodible_tiles.has(neighbor)
				):
					erodible_tiles.append(neighbor)

			if _is_erodibe(target) and not erodible_tiles.has(target):
				erodible_tiles.append(target)

			for neighbor in _get_neighbors(target):
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
					break
	map = next


func _print_prop():
	print("----- Generator Properties ------")
	print("map size = {}x{}".format([MAP_SIZE_X, MAP_SIZE_Y], "{}"))
	print("upscaled map size = {}x{}".format([MAP_SIZE_X * UPSCALE, MAP_SIZE_Y * UPSCALE], "{}"))
	print("chunk size = {}-{}".format([CHUNK_SIZE_MIN, CHUNK_SIZE_MAX], "{}"))
	print("elevation = ({})-({})-({})".format([ELEVATION_MIN, WATER_LEVEL, ELEVATION_MAX], "{}"))
	print("land percentage = %s" % str(LAND_PERCENTAGE))
	print("map border = x: {} , y: {}".format([MAP_BORDER_X, MAP_BORDER_Y], "{}"))
	print("---------------------------------")


func generate_map(world, use_existing_map):
	_print_prop()

	var elapsed_time = OS.get_ticks_msec()
	if not use_existing_map:
		_init_map()
		_create_regions()
		_create_land()
		_erode_land()
		_smooth_land()
		_remove_lone_pillars()
		elapsed_time = OS.get_ticks_msec() - elapsed_time
		print("Generate Map Time (ms): %s" % str(elapsed_time))

	if not use_existing_map:
		elapsed_time = OS.get_ticks_msec()
		_upscale_map()
		elapsed_time = OS.get_ticks_msec() - elapsed_time
		print("Map Upscaling Time (ms): %s" % str(elapsed_time))

	elapsed_time = OS.get_ticks_msec()
	_set_terrain_tiles(world)
	_set_feature_tiles(world)
	elapsed_time = OS.get_ticks_msec() - elapsed_time
	print("Tilemap Generation Time (ms): %s" % str(elapsed_time))
	print("---------------------------------")


func _set_feature_tiles(world):
	var tree_noise: OpenSimplexNoise = world.feature_noise.duplicate()
	tree_noise.seed = randi()
	var bush_noise: OpenSimplexNoise = world.feature_noise.duplicate()
	bush_noise.seed = randi()
	var grass_noise: OpenSimplexNoise = world.grass_noise
	var granular_noise: OpenSimplexNoise = world.grass_noise.duplicate()
	granular_noise.seed = randi()
	for item in map:
		var tile: Tile = item

		if tile.is_land and not tile.is_cliff:
			if grass_noise.get_noise_2dv(tile.coordinate) > 0.2:
				world.add_grass_tile(tile.coordinate)

		var feature_rng = randf()
		if tile.is_land and not tile.is_cliff and not tile.is_near_water:
			if (
				tile.elevation > WATER_LEVEL + 2
				and world.feature_noise.get_noise_2dv(tile.coordinate) > 0.35
				and granular_noise.get_noise_2dv(tile.coordinate) > 0.3
				and feature_rng < 0.4
			):
				_add_feature(world, tile, world.Tile.Rock, 3)
				continue
			if (
				bush_noise.get_noise_2dv(tile.coordinate) > 0.3
				and granular_noise.get_noise_2dv(tile.coordinate) > 0.3
			):
				_add_feature(world, tile, world.Tile.Bush, 3)
				continue
			
			
	for tile in map:
		if tile.is_land and not tile.is_cliff and not tile.is_near_water:
			var pos = tile.coordinate
			if int(pos.y) % 2 == 0 and int(pos.x) % 4 == (0 if int(pos.y) % 4 == 0 else 2):
				_add_feature(world, tile, world.Tile.Tree, 1, true)


func _add_feature(world, tile, type, separation = 2, no_same_check = false):
	var is_valid = true
	for neighbor in _get_neighbors(tile):
		if neighbor.elevation != tile.elevation:
			return
		if neighbor.is_cliff:
			return
		if (separation > 0 and neighbor.feature_type != -1 
			and (neighbor.feature_type != type if no_same_check else true)):
			return
	if separation > 0:
		for neighbor in _get_neighbors(tile, separation, 1):
			if neighbor.feature_type != -1 and (neighbor.feature_type != type if no_same_check else true):
				return
	tile.feature_type = type
	world.add_feature_tile(tile.coordinate, type)


func _set_terrain_tiles(world):
	for item in map:
		var tile: Tile = item
		if not tile.is_land:
			_set_water_tile(tile, world)
		else:
			world.add_land_tile(tile.coordinate, tile.elevation, not tile.is_cliff_corner)
			for neighbor in _get_neighbors(tile):
				if neighbor.is_land and neighbor.elevation < tile.elevation:
					world.add_land_tile(tile.coordinate, neighbor.elevation, false)

			var elevation_above_water = tile.elevation - WATER_LEVEL
			if elevation_above_water > 1:
				_set_cliffs(tile, world)


func _set_water_tile(tile, world):
	world.add_water_tile(tile.coordinate)
	for direction in Directions:
		var neighbor = _get_neighbor(tile, direction)
		if not neighbor or not neighbor.is_land:
			continue
		if direction == Direction.N:
			_set_water_edge(tile, world)
		neighbor.is_near_water = true
		for _i in range(int(direction), int(direction) + 1):
			var next_neighbor = _get_neighbor(neighbor, direction)
			if not next_neighbor or not next_neighbor.is_land:
				continue
			next_neighbor.is_near_water = true


func set_special_rule_tiles(world):
	for item in map:
		var tile: Tile = item
		if not tile.is_land and not tile.is_water_cliff:
			var water_bottom_corner_rule = {
				Direction.SE: _get_neighbor(tile, Direction.E),
				Direction.SW: _get_neighbor(tile, Direction.W),
				Direction.NW: _get_neighbor(tile, Direction.N),
				Direction.NE: _get_neighbor(tile, Direction.N),
			}
			for direction in water_bottom_corner_rule:
				var neighbor = _get_neighbor(tile, direction)
				if (
					water_bottom_corner_rule[direction]
					and water_bottom_corner_rule[direction].is_water_cliff
					and neighbor
					and not neighbor.is_land
					and not neighbor.is_water_cliff
				):
					world.add_water_corner(tile.coordinate, direction)
					break

			var water_upper_corner_rule = {
				Direction.W: Direction.NW,
				Direction.E: Direction.NE,
			}
			for direction in water_upper_corner_rule:
				var neighbor = _get_neighbor(tile, direction)
				var next_neighbor = _get_neighbor(tile, water_upper_corner_rule[direction])
				if (
					neighbor
					and neighbor.is_land
					and next_neighbor
					and not next_neighbor.is_land
					and not next_neighbor.is_water_cliff
				):
					world.add_water_corner(
						tile.coordinate, water_upper_corner_rule[direction], true
					)
					world.add_water_tile(neighbor.coordinate, false, true)
					break


func _set_water_edge(tile, world):
	world.add_water_tile(tile.coordinate, true)
	tile.is_water_cliff = true
	for next_neighbor in [_get_neighbor(tile, Direction.W), _get_neighbor(tile, Direction.E)]:
		if next_neighbor and next_neighbor.elevation == WATER_LEVEL + 1:
			world.add_water_tile(next_neighbor.coordinate, true, false, false)
			next_neighbor.is_water_cliff = true


func _set_cliffs(tile, world):
	var valid_elevation = tile.elevation - 1
	var neighbor = tile
	for i in range(2):
		neighbor = _get_neighbor(neighbor, Direction.S)
		if neighbor and neighbor.elevation == valid_elevation:
			neighbor.is_cliff = true
			world.add_cliff_tile(neighbor.coordinate, tile.elevation)
			for next_neighbor in [
				_get_neighbor(neighbor, Direction.W), _get_neighbor(neighbor, Direction.E)
			]:
				if next_neighbor and next_neighbor.elevation == tile.elevation:
					world.add_cliff_tile(next_neighbor.coordinate, tile.elevation, false)
					if i == 0:
						next_neighbor.is_cliff_corner = true

func _upscale_map():
	if UPSCALE == 1:
		return

	var upscaled = []
	upscaled.resize(len(map) * pow(UPSCALE, 2))
	for i in range(len(upscaled)):
		var x = i % (MAP_SIZE_X * UPSCALE)
		var y = floor(i / (MAP_SIZE_X * UPSCALE))
		var xi = floor(x / UPSCALE)
		var yi = floor(y / UPSCALE)
		var idx = xi + (yi * MAP_SIZE_X)
		var original_tile = map[idx]
		var tile = Tile.new(i)
		tile.upscale_factor = UPSCALE
		tile.elevation = original_tile.elevation
		upscaled[i] = tile

	map = upscaled
