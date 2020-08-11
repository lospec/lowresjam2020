extends MarginContainer

# Public Variables
var character_name: String

# Onready Variables
onready var character_button = $Character
onready var hover_texture_rect = $Control/Hover


func update_character() -> bool:
	var tex = AtlasTexture.new()
	tex.atlas = load("res://Entities/Player/spritesheets/%s_Overworld.png"
			% character_name)
	if tex.atlas == null:
		return false
	tex.region = Rect2(0, 0, 8, 12)
	character_button.texture_normal = tex
	return true


func _on_Character_mouse_entered():
	hover_texture_rect.visible = true


func _on_Character_mouse_exited():
	hover_texture_rect.visible = false
