tool
extends Node

export (TileSet) var tileset
export (bool) var run_generator = false setget ,_run
export (bool) var print_properties = false setget , _print_prop
export (int) var map_seed = 1234567890
export (bool) var use_seed = false
export (bool) var run_climate_simulation = true
export (bool) var create_climate_texture_maps = true

export (OpenSimplexNoise) var feature_noise = OpenSimplexNoise.new()

class TilemapManager:
	var _parent: Node
	var _water_level
	var height_tilemaps: Dictionary = {}
	var feature_tilemap
	var grass_tilemap

	func _make_tilemap_for_elevation(elevation):
		for i in range(_water_level, elevation + 1):
			if height_tilemaps.has(i):
				continue

			var tilemap: TileMap = TileMap.new()
			tilemap.cell_size = Vector2(4, 4)
			tilemap.tile_set = _parent.tileset
			tilemap.name = "height_%s" % str(i - _water_level)
			_parent.map_node.add_child(tilemap)
			tilemap.owner = _parent
			height_tilemaps[i] = tilemap
	
	func get_grass_tilemap():
		if not grass_tilemap:
			var tilemap: TileMap = TileMap.new()
			tilemap.cell_size = Vector2(4, 4)
			tilemap.tile_set = _parent.tileset
			tilemap.name = "grass"
			_parent.map_node.add_child(tilemap)
			_parent.move_child(tilemap, _parent.get_child_count()-1)
			tilemap.owner = _parent
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
			_parent.move_child(tilemap, _parent.get_child_count()-1)
			tilemap.owner = _parent
			feature_tilemap = tilemap
		return feature_tilemap

	func get_tilemap_for_elevation(elevation: int) -> TileMap:
		if not height_tilemaps.has(elevation):
			_make_tilemap_for_elevation(elevation)
		return height_tilemaps[elevation]

	func update_bitmask():
		for item in height_tilemaps.values():
			var tilemap: TileMap = item
			tilemap.update_bitmask_region()

	func _init(parent):
		_parent = parent
		_water_level = _parent.map_generator.WATER_LEVEL


enum Tile { Land, Water, Cliff, Grass, Tree, Rock, Other, Bush }

const TILES = {
	Tile.Land: 0,
	Tile.Water: 1,
	Tile.Cliff: 2,
	Tile.Other: 4,
	Tile.Rock: 5,
	Tile.Bush: 6,
	Tile.Tree: 7,
	Tile.Grass: [8,9,10,11],
}


var map_generator = preload("res://World/MapGen/MapGen.gd").new()
var tilemap_manager = TilemapManager.new(self)
var map_node: Node


func _print_prop():
	if not Engine.editor_hint or not print_properties:
		return
	print_properties = false
	map_generator = preload("res://World/MapGen/MapGen.gd").new()
	map_generator._print_prop()
		

func _run():
	if not Engine.editor_hint or not run_generator:
		return
	
	randomize()
	
	if not use_seed:
		map_seed = randi()
		
	seed(map_seed)
	feature_noise.seed = map_seed
	run_generator = false
	map_generator = preload("res://World/MapGen/MapGen.gd").new()
	print("# Starting Map Generation")
	var elapsed = OS.get_ticks_msec()
	tilemap_manager = TilemapManager.new(self)
	map_node = find_node("Map")
	if map_node:
		remove_child(map_node)

	map_node = Node2D.new()
	map_node.name = "Map"
	add_child(map_node)
	map_node.owner = self
	map_generator.generate_map(self, run_climate_simulation, create_climate_texture_maps)
	tilemap_manager.update_bitmask()
	
	elapsed = OS.get_ticks_msec() - elapsed
	print("# Map Generation Process Complete")
	print("Time (ms) elapsed: %s" % str(elapsed))
	
	randomize()
	
func add_grass_tile(position: Vector2):
	var tilemap = tilemap_manager.get_grass_tilemap()
	var tile = TILES[Tile.Grass][randi() % len(TILES[Tile.Grass])]
	tilemap.set_cellv(position, tile)
	
func add_feature_tile(position: Vector2, type):
	var tilemap = tilemap_manager.get_feature_tilemap()
	tilemap.set_cellv(position, TILES[type])

func add_land_tile(position: Vector2, elevation: int):
	var tilemap: TileMap = tilemap_manager.get_tilemap_for_elevation(elevation)
	tilemap.set_cellv(position, TILES[Tile.Land])


func add_water_tile(position: Vector2):
	var elevation = map_generator.WATER_LEVEL
	var tilemap: TileMap = tilemap_manager.get_tilemap_for_elevation(elevation)
	tilemap.set_cellv(position, TILES[Tile.Water])


func add_cliff_tile(position: Vector2, elevation: int):
	var tilemap: TileMap = tilemap_manager.get_tilemap_for_elevation(elevation)
	tilemap.set_cellv(position, TILES[Tile.Cliff])
