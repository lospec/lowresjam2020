extends CanvasLayer

# Exported Variables
export(bool) var enabled setget set_enabled
export(Texture) var palette setget set_palette

# Onready Variables
onready var palette_swap_texture_rect = $TextureRect

func _ready():
	palette_swap_texture_rect.material.set_shader_param("enabled", enabled)
	palette_swap_texture_rect.material.set_shader_param("palette_tex", palette)

func set_enabled(val):
	enabled = val
	
	if not has_node("TextureRect"):
		return
	
	palette_swap_texture_rect.material.set_shader_param("enabled", enabled)

func set_palette(val):
	palette = val
	
	if not has_node("TextureRect"):
		return
	
	palette_swap_texture_rect.material.set_shader_param("palette_tex", palette)
