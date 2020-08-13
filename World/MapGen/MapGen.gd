extends Reference

const UPSCALE = 4
const CHUNK_SIZE = 64

const MAP_CHUNK_SIZE_X = 6
const MAP_CHUNK_SIZE_Y = 4

const MAP_SIZE_X = MAP_CHUNK_SIZE_X * CHUNK_SIZE
const MAP_SIZE_Y = MAP_CHUNK_SIZE_Y * CHUNK_SIZE

const JITTER_PROBABILITY = 0.1
const JITTER_STRENGTH = 0.01

const CHUNK_SIZE_MIN = 512
const CHUNK_SIZE_MAX = 3980
const LAND_PERCENTAGE = 0.4

const WATER_LEVEL = 2
const ELEVATION_MIN = -3
const ELEVATION_MAX = 7

const HIGH_RISE_STRENGTH = 1
const HIGH_RISE_PROBABILITY = 0.1
const SINK_PROBABILITY = 0.05
const EROSIAN_PERCENTAGE = 0.8
const EROSIAN_CYCLE = 8

const MAP_BORDER_X = 96
const MAP_BORDER_Y = 48
const REGION_BORDER = 20

const CELLULAR_AUTOMATA_CYCLE = 3
const CELLULAR_AUTOMATA_LIVE = 5
const CELLULAR_AUTOMATA_DEAD = 4
const CELLULAR_AUTOMATA_SMOOTH_ELEVATION_CYCLES = 2

const REGION_COUNT = 1

# climate
var climate_parameters

enum Direction { N, NE, E, SE, S, SW, W, NW }
const WIND_DIRECTION = Direction.NE

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


class ClimateData:
	var clouds: float = 0
	var moisture: float = 0

	func _init(starting_moisture = null):
		if starting_moisture:
			moisture = starting_moisture

	func duplicate():
		var data = ClimateData.new()
		data.clouds = clouds
		data.moisture = moisture
		return data


var climate = []
var next_climate = []

var height_map: Image
var cloud_map: Image
var moisture_map: Image

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
		if (
			v.x < 0
			or v.x >= MAP_SIZE_X * current.upscale_factor
			or v.y < 0
			or v.y >= MAP_SIZE_Y * current.upscale_factor
		):
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
					break
	map = next


func _evolve_climate(idx: int):
	var tile = map[idx]
	var tile_climate = climate[idx]
	if not tile.is_land:
		tile_climate.moisture = 1.0
		tile_climate.clouds += climate_parameters.EVAPORATION_FACTOR
	else:
		var evaporation = tile_climate.moisture * climate_parameters.EVAPORATION_FACTOR
		tile_climate.moisture -= evaporation
		tile_climate.clouds += evaporation

	var precipitation = tile_climate.clouds * climate_parameters.PRECIPITATION_FACTOR
	tile_climate.clouds -= precipitation
	tile_climate.moisture += precipitation

	var cloud_maximum = 1 - (tile.over_water_elevation / (ELEVATION_MAX + 1))
	if tile_climate.clouds > cloud_maximum:
		tile_climate.moisture += tile_climate.clouds - cloud_maximum
		tile_climate.clouds = cloud_maximum

	var cloud_dispersal = (
		tile_climate.clouds
		* (1.0 / (5.0 + climate_parameters.WIND_STRENGTH))
		* climate_parameters.DISSIPATION_STRENTH
	)
	var runoff = (
		tile_climate.moisture
		* climate_parameters.RUNOFF_FACTOR
		* (1.0 / 8.0)
		* climate_parameters.DISSIPATION_STRENTH
	)
	var seepage = (
		tile_climate.moisture
		* climate_parameters.SEEPAGE_FACTOR
		* (1.0 / 8.0)
		* climate_parameters.DISSIPATION_STRENTH
	)

	for direction in Directions:
		var neighbor = _get_neighbor(tile, direction)
		if not neighbor:
			continue
		var neighbor_climate = next_climate[neighbor.idx]
		if direction == WIND_DIRECTION:
			neighbor_climate.clouds += cloud_dispersal * climate_parameters.WIND_STRENGTH
		else:
			neighbor_climate.clouds += cloud_dispersal

		var elevation_delta = neighbor.over_water_elevation - tile.over_water_elevation
		if elevation_delta < 0:
			tile_climate.moisture -= runoff
			neighbor_climate.moisture += runoff
		elif elevation_delta == 0:
			tile_climate.moisture -= seepage
			neighbor_climate.moisture += seepage
		next_climate[neighbor.idx] = neighbor_climate

	var next_tile_climate = next_climate[idx]
	next_tile_climate.moisture += tile_climate.moisture
	if next_tile_climate.moisture > 1:
		next_tile_climate.moisture = 1
	next_climate[idx] = next_tile_climate
	climate[idx] = ClimateData.new()


func _print_climate_prop():
	print("----- Climate Properties ------")
	print("DISSIPATION_STRENTH = %s" % climate_parameters.DISSIPATION_STRENTH)
	print("EVAPORATION_FACTOR = %s" % climate_parameters.EVAPORATION_FACTOR)
	print("PRECIPITATION_FACTOR = %s" % climate_parameters.PRECIPITATION_FACTOR)
	print("RUNOFF_FACTOR = %s" % climate_parameters.RUNOFF_FACTOR)
	print("SEEPAGE_FACTOR = %s" % climate_parameters.SEEPAGE_FACTOR)
	print("WIND_STRENGTH = %s" % climate_parameters.WIND_STRENGTH)
	print("STARTING_MOISTURE = %s" % climate_parameters.STARTING_MOISTURE)
	print("CLIMATE_CYCLE = %s" % climate_parameters.CLIMATE_CYCLE)
	print("---------------------------------")


func _create_climate():
	climate.clear()
	next_climate.clear()
	climate.resize(len(map))
	next_climate.resize(len(map))
	for i in range(len(map)):
		climate[i] = ClimateData.new(climate_parameters.STARTING_MOISTURE)
		next_climate[i] = ClimateData.new()

	for _cycle in range(0, climate_parameters.CLIMATE_CYCLE):
		for i in range(len(map)):
			_evolve_climate(i)
		var swap = climate
		climate = next_climate
		next_climate = swap


func _save_cloud_map():
	var max_clouds = -1
	for i in range(len(climate)):
		if map[i].is_land and climate[i].clouds > max_clouds:
			max_clouds = climate[i].clouds
	cloud_map = Image.new()
	cloud_map.create(MAP_SIZE_X, MAP_SIZE_Y, false, Image.FORMAT_RGB8)
	cloud_map.lock()
	var idx = 0
	for y in MAP_SIZE_Y:
		for x in MAP_SIZE_X:
			var color = Color.aqua
			if map[idx].is_land:
				var data = climate[idx]
				var v = range_lerp(data.clouds, 0, max_clouds, 0, 1)
				color = Color(v, v, v)
			cloud_map.set_pixel(x, y, color)
			idx += 1

	cloud_map.unlock()
	var _result = cloud_map.save_png(DIR + "cloud_map.png")
	print("cloud_map saved")


func _save_moisture_map():
	var max_moisture = -1
	for i in range(len(climate)):
		if map[i].is_land and climate[i].moisture > max_moisture:
			max_moisture = climate[i].moisture
	moisture_map = Image.new()
	moisture_map.create(MAP_SIZE_X, MAP_SIZE_Y, false, Image.FORMAT_RGB8)
	moisture_map.lock()
	var idx = 0
	for y in MAP_SIZE_Y:
		for x in MAP_SIZE_X:
			var color = Color.aqua
			if map[idx].is_land:
				var data = climate[idx]
				var v = range_lerp(data.moisture, 0, max_moisture, 0, 1)
				color = Color(v, v, v)
			moisture_map.set_pixel(x, y, color)
			idx += 1

	moisture_map.unlock()
	var _result = moisture_map.save_png(DIR + "moisture_map.png")
	print("moisture_map saved")


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
			var is_valid = true
			for neighbor in _get_neighbors(tile):
				if neighbor.elevation > tile.elevation:
					is_valid = false
					break
				if neighbor.is_cliff:
					is_valid = false
					break
			if not _get_neighbor(tile, Direction.E).is_land:
				is_valid = false
			if not is_valid:
				continue

			if (
				tree_noise.get_noise_2dv(tile.coordinate) > 0.2
				and granular_noise.get_noise_2dv(tile.coordinate) > 0.25
				and not tile.is_near_water
			):
				_add_feature(world, tile, world.Tile.Tree)
				continue
			if (
				tile.over_water_elevation > 2
				and world.feature_noise.get_noise_2dv(tile.coordinate) > 0.35
				and granular_noise.get_noise_2dv(tile.coordinate) > 0.3
				and feature_rng < 0.4
			):
				_add_feature(world, tile, world.Tile.Rock)
				continue
			if (
				bush_noise.get_noise_2dv(tile.coordinate) > 0.5
				and granular_noise.get_noise_2dv(tile.coordinate) > 0.5
			):
				_add_feature(world, tile, world.Tile.Bush)
				continue


func _add_feature(world, tile, type):
	for neighbor in _get_neighbors(tile):
		if neighbor.feature_type != -1:
			return
	tile.feature_type = type
	world.add_feature_tile(tile.coordinate, type)


func _set_terrain_tiles(world):
	for item in map:
		var tile: Tile = item
		if not tile.is_land:
			world.add_water_tile(tile.coordinate)
			for direction in Directions:
				var neighbor = _get_neighbor(tile, direction)
				if not neighbor or not neighbor.is_land:
					continue
				world.add_water_tile(neighbor.coordinate)
				neighbor.is_near_water = true
				for i in range(int(direction), int(direction) + 1):
					var next_neighbor = _get_neighbor(neighbor, direction)
					if not next_neighbor or not next_neighbor.is_land:
						continue
					next_neighbor.is_near_water = true
		else:
			world.add_land_tile(tile.coordinate, tile.elevation)
			for neighbor in _get_neighbors(tile):
				if neighbor.is_land and neighbor.elevation < tile.elevation:
					world.add_land_tile(tile.coordinate, neighbor.elevation)

			var elevation_above_water = tile.elevation - WATER_LEVEL
			if elevation_above_water > 0:
				var neighbor = _get_neighbor(tile, Direction.S)
				if neighbor and neighbor.elevation < tile.elevation:
					neighbor.is_cliff = true
					world.add_cliff_tile(neighbor.coordinate, tile.elevation)


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
