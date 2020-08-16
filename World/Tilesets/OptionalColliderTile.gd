tool
extends TileSet

func bind_tiles(drawn_id, neighbor_id, x,y):
	if drawn_id == x and neighbor_id == y:
		return true
	if drawn_id == y and neighbor_id == x:
		return true

func _is_tile_bound(drawn_id, neighbor_id):
	bind_tiles(drawn_id, neighbor_id, 0, 15)
	bind_tiles(drawn_id, neighbor_id, 2, 16)
	bind_tiles(drawn_id, neighbor_id, 12, 17)
	bind_tiles(drawn_id, neighbor_id, 41, 35)
	bind_tiles(drawn_id, neighbor_id, 0, 35)
	bind_tiles(drawn_id, neighbor_id, 0, 41)
	return drawn_id == neighbor_id
