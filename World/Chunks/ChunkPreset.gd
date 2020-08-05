tool
extends Node2D

# Exported Variables
export(bool) var fix_tiles_toggle = false setget fix_tiles
export(OpenSimplexNoise) var grass_noise

# Constants
enum GRASS_TILES {
	MAIN_GRASS,
	GRASS_ALT,
	GRASS_ALT_2,
	GRASS_ALT_3,
	GRASS_ALT_4
}
const GRASS_ALT_TILES = [GRASS_TILES.GRASS_ALT, GRASS_TILES.GRASS_ALT_2, GRASS_TILES.GRASS_ALT_3, GRASS_TILES.GRASS_ALT_4]
const GRASS_ALT_THRESHOLD = 0.3

# Onready Variables
onready var enemy_spawns = $EnemySpawns

func fix_tiles(_b):
	if not Engine.editor_hint:
		return
	
	var height0_tilemap = $Height0
	var Utility = load("res://Utility/Utility.gd") # Tool scripts don't support the use of autoloaded scripts
	
	var grass_cells = height0_tilemap.get_used_cells_by_id(GRASS_TILES.MAIN_GRASS) + \
			height0_tilemap.get_used_cells_by_id(GRASS_TILES.GRASS_ALT) + \
			height0_tilemap.get_used_cells_by_id(GRASS_TILES.GRASS_ALT_2) + \
			height0_tilemap.get_used_cells_by_id(GRASS_TILES.GRASS_ALT_3) + \
			height0_tilemap.get_used_cells_by_id(GRASS_TILES.GRASS_ALT_4)
	for grass_cell in grass_cells:
		var new_tile = GRASS_TILES.MAIN_GRASS
		if grass_noise.get_noise_2dv(height0_tilemap.map_to_world(grass_cell)) + 0.5 < GRASS_ALT_THRESHOLD:
			new_tile = Utility.rand_element(GRASS_ALT_TILES)
		height0_tilemap.set_cellv(grass_cell, new_tile)
