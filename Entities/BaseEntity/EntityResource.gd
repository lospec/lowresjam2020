extends Resource
class_name EntityResource

export (String) var entity_name
export (SpriteFrames) var entity_sprite_frames

func apply(entity):
	entity.animated_sprite.frames = entity_sprite_frames
