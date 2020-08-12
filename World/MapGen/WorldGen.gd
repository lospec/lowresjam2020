tool
extends Node

export (TileSet) var tileset
export (bool) var run_generator = false setget ,_run
export (bool) var print_properties = false setget , _print_prop
export (int) var map_seed = 1234567890
export (bool) var use_seed = false
export (bool) var run_climate_simulation = true

class TilemapManager:
	var _parent
	var _water_level
	var height_tilemaps: Dictionary = {}

	func _make_scene_for_elevation(elevation):
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

	func get_tilemap_for_elevation(elevation: int) -> TileMap:
		if not height_tilemaps.has(elevation):
			_make_scene_for_elevation(elevation)
		return height_tilemaps[elevation]

	func update_bitmask():
		for item in height_tilemaps.values():
			var tilemap: TileMap = item
			tilemap.update_bitmask_region()

	func _init(parent):
		_parent = parent
		_water_level = _parent.map_generator.WATER_LEVEL


enum TileType { Grass, Water, Cliff }
const Tiles = {TileType.Cliff: 2, TileType.Grass: 0, TileType.Water: 1}


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
	map_generator.generate_map(self, run_climate_simulation)
	tilemap_manager.update_bitmask()
	
	elapsed = OS.get_ticks_msec() - elapsed
	print("# Map Generation Process Complete")
	print("Time (ms) elapsed: %s" % str(elapsed))
	
	randomize()

func add_grass_tile(position: Vector2, elevation: int):
	var tilemap: TileMap = tilemap_manager.get_tilemap_for_elevation(elevation)
	tilemap.set_cellv(position, Tiles[TileType.Grass])


func add_water_tile(position: Vector2):
	var elevation = map_generator.WATER_LEVEL
	var tilemap: TileMap = tilemap_manager.get_tilemap_for_elevation(elevation)
	tilemap.set_cellv(position, Tiles[TileType.Water])


func add_cliff_tile(position: Vector2, elevation: int):
	var tilemap: TileMap = tilemap_manager.get_tilemap_for_elevation(elevation)
	tilemap.set_cellv(position, Tiles[TileType.Cliff])
