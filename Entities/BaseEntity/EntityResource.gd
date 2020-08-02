extends Resource
class_name EntityResource

export (String) var entity_name
export (Texture) var entity_sprite

func apply(entity):
	entity.sprite.texture = entity_sprite
