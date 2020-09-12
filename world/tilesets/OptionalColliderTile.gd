tool
extends TileSet

const RULES = [
	[0, 15],
	[2, 16],
	[12, 17],
	[41, 35],
	[0, 35],
	[0, 41],
]
func bind_tiles(drawn_id, neighbor_id, x, y):
	if drawn_id == x and neighbor_id == y:
		return true
	if drawn_id == y and neighbor_id == x:
		return true
	return false

func _is_tile_bound(drawn_id, neighbor_id):
	if drawn_id == neighbor_id:
		return true

	for rule in RULES:
		if bind_tiles(drawn_id, neighbor_id, rule[0], rule[1]):
			return true
			
	return false
