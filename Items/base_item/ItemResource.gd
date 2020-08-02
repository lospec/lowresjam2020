extends Resource
class_name ItemResource

export (String) var item_name
export (Texture) var item_texture

func apply(item):
	item.animated_sprite.frames = item_texture
