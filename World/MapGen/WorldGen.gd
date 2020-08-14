tool
extends Node

export (TileSet) var tileset
export (bool) var run_generator = false setget ,_run
export (bool) var print_properties = false setget , _print_prop
export (bool) var use_existing_map = false setget _set_use_existing_map
export (int) var map_seed = 1234567890
export (bool) var use_seed = false

export (OpenSimplexNoise) var feature_noise = OpenSimplexNoise.new()
export (OpenSimplexNoise) var grass_noise = OpenSimplexNoise.new()

enum Direction { N, NE, E, SE, S, SW, W, NW }

class TilemapManager:
	var _parent: Node
	var _water_level
	var height_tilemaps: Dictionary = {}
	var cliff_tilemaps: Dictionary =  {}
	var feature_tilemap
	var grass_tilemap

	func _make_tilemap_for_elevation(elevation):
		for i in range(_water_level, elevation + 1):
			if height_tilemaps.has(i):
				continue
			var tilemap: TileMap = TileMap.new()
			tilemap.cell_size = Vector2(4, 4)
			tilemap.tile_set = _parent.tileset
			
			if i > _water_level:
				var cliff_tilemap = tilemap.duplicate()
				cliff_tilemap.name = "cliff_%s" % str(i - _water_level)
				_parent.map_node.add_child(cliff_tilemap)
				cliff_tilemap.owner = _parent
				cliff_tilemaps[i] = cliff_tilemap
			
			var land_tilemap = tilemap.duplicate()
			land_tilemap.name = "height_%s" % str(i - _water_level)
			_parent.map_node.add_child(land_tilemap)
			land_tilemap.owner = _parent
			height_tilemaps[i] = land_tilemap
	
	func get_grass_tilemap():
		if not grass_tilemap:
			var tilemap: TileMap = TileMap.new()
			tilemap.cell_size = Vector2(4, 4)
			tilemap.tile_set = _parent.tileset
			tilemap.name = "grass"
			_parent.map_node.add_child(tilemap)
			tilemap.owner = _parent
			_parent.map_node.move_child(tilemap, _parent.map_node.get_child_count()-1)
			grass_tilemap = tilemap
		return grass_tilemap
	
	func get_feature_tilemap():
		if not grass_tilemap:
			get_grass_tilemap()
		if not feature_tilemap:
			var tilemap: TileMap = TileMap.new()
			tilemap.cell_size = Vector2(4, 4)
			tilemap.tile_set = _parent.tileset
			tilemap.name = "features"
			_parent.map_node.add_child(tilemap)
			tilemap.owner = _parent
			_parent.map_node.move_child(tilemap, _parent.map_node.get_child_count()-1)
			feature_tilemap = tilemap
		return feature_tilemap

	func get_elevation_tilemap(elevation: int) -> TileMap:
		if not height_tilemaps.has(elevation):
			_make_tilemap_for_elevation(elevation)
		return height_tilemaps[elevation]
		
	func get_cliff_tilemap(elevation: int) -> TileMap:
		if not cliff_tilemaps.has(elevation):
			_make_tilemap_for_elevation(elevation)
		return cliff_tilemaps[elevation]

	func update_bitmask():
		for tilemap in height_tilemaps.values():
			tilemap.update_bitmask_region()
		for tilemap in cliff_tilemaps.values():
			tilemap.update_bitmask_region()

	func _init(parent):
		_parent = parent
		_water_level = _parent.map_generator.WATER_LEVEL


enum Tile { Land, Water, Cliff, Grass, Tree, Rock, Other, Bush, WaterEdge, WaterCorner, SolidWater}

const TILES = {
	Tile.Land: 0,
	Tile.Water: 1,
	Tile.Cliff: 2,
	Tile.Other: 4,
	Tile.Rock: 5,
	Tile.Bush: 6,
	Tile.Tree: 7,
	Tile.Grass: [8,9,10,11],
	Tile.WaterEdge: 12,
	Tile.WaterCorner: [
		Vector2(9,3),
		Vector2(10,3),
		Vector2(9,4),
		Vector2(10,4),
		Vector2(9,5),
		Vector2(10,5)
	],
	Tile.SolidWater: Vector2(6, 2)
}

var map_generator = preload("res://World/MapGen/MapGen.gd").new()
var tilemap_manager = TilemapManager.new(self)
var map_node: Node

func _set_use_existing_map(value):
	use_existing_map = value

func _print_prop():
	if not Engine.editor_hint or not print_properties:
		return
	print_properties = false
	if not use_existing_map:
		map_generator = preload("res://World/MapGen/MapGen.gd").new()
	map_generator._print_prop()
		

func _run():
	if not Engine.editor_hint or not run_generator:
		return
	run_generator = false
	
	print("# Starting Map Generation")
	var elapsed = OS.get_ticks_msec()
	
	map_node = find_node("CoreMap")
	if not use_existing_map:
		randomize()
		if not use_seed:
			map_seed = randi()	
		seed(map_seed)
		feature_noise.seed = map_seed
		map_generator = preload("res://World/MapGen/MapGen.gd").new()
		tilemap_manager = TilemapManager.new(self)
		if map_node:
			remove_child(map_node)
		map_node = Node2D.new()
		map_node.name = "CoreMap"
		add_child(map_node)
		map_node.owner = self
	
	if tilemap_manager:
		var tilemaps = [map_node.find_node("grass"), map_node.find_node("features")]
		for tilemap in tilemaps:
			if tilemap:
				map_node.remove_child(tilemap)
	
	map_generator.generate_map(self, use_existing_map)
	tilemap_manager.update_bitmask()
	map_generator.set_special_rule_tiles(self)
	
	elapsed = OS.get_ticks_msec() - elapsed
	print("# Map Generation Process Complete")
	print("Total Time (ms) elapsed: %s" % str(elapsed))

	randomize()
	
func add_grass_tile(position: Vector2):
	var tilemap = tilemap_manager.get_grass_tilemap()
	var tile = TILES[Tile.Grass][randi() % len(TILES[Tile.Grass])]
	tilemap.set_cellv(position, tile)
	
func add_feature_tile(position: Vector2, type):
	var tilemap = tilemap_manager.get_feature_tilemap()
	tilemap.set_cellv(position, TILES[type])

func add_land_tile(position: Vector2, elevation: int):
	var tilemap: TileMap = tilemap_manager.get_elevation_tilemap(elevation)
	tilemap.set_cellv(position, TILES[Tile.Land])


func add_water_tile(position: Vector2, edge = false, solid = false):
	var elevation = map_generator.WATER_LEVEL
	var tilemap: TileMap = tilemap_manager.get_elevation_tilemap(elevation)
	if solid:
		tilemap.set_cell(position.x, position.y, Tile.Water, false ,false, false, TILES[Tile.SolidWater])
	else:
		tilemap.set_cellv(position, TILES[Tile.WaterEdge if edge else Tile.Water])
	
func add_water_corner(position: Vector2, direction, upper = false):
	var elevation = map_generator.WATER_LEVEL
	var tilemap: TileMap = tilemap_manager.get_elevation_tilemap(elevation)
	var type
	if direction == Direction.SW:
		type = TILES[Tile.WaterCorner][0] 
	elif direction == Direction.SE:
		type = TILES[Tile.WaterCorner][1]
	elif direction == Direction.NE:
		type = TILES[Tile.WaterCorner][2 if not upper else 5]
	elif direction == Direction.NW:
		type = TILES[Tile.WaterCorner][3 if not upper else 4]
	else:
		return
	tilemap.set_cell(position.x, position.y, Tile.Water, false ,false, false, type)

func add_cliff_tile(position: Vector2, elevation: int):
	var tilemap: TileMap = tilemap_manager.get_cliff_tilemap(elevation)
	tilemap.set_cellv(position, TILES[Tile.Cliff])
