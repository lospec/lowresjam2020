tool
extends TileSet

func _is_tile_bound(drawn_id, neighbor_id):
	if drawn_id == 15 and neighbor_id == 0:
		return true
	if drawn_id == 0 and neighbor_id == 15:
		return true
	if drawn_id == 2 and neighbor_id == 16:
		return true
	if drawn_id == 16 and neighbor_id == 2:
		return true
	if drawn_id == 12 and neighbor_id == 17:
		return true
	if drawn_id == 17 and neighbor_id == 12:
		return true
	return drawn_id == neighbor_id
