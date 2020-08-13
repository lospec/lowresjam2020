extends MarginContainer

# Constants
const CHARACTER_RESOURCE = preload("res://UI/character_selection/character.tscn")
const SCROLL_AMOUNT = 40

# Public Variables
var selected_character_name: String

# Private Variables
var _scroll_left_held := false
var _scroll_right_held := false
var _scroll_value := 0.0

# Onready Variables
onready var characters_scroll = $MarginContainer/VBoxContainer/CenterContainer/CharactersScroll
onready var characters = characters_scroll.get_node("Characters")
onready var select_vbox = $MarginContainer/VBoxContainer/VBoxContainer2/SelectVBox
onready var name_label = select_vbox.get_node("VBoxContainer/Label")
onready var buttons = [
	$MarginContainer/VBoxContainer/VBoxContainer2/HBoxContainer/ScrollLeft,
	$MarginContainer/VBoxContainer/VBoxContainer2/HBoxContainer/ScrollRight,
	$MarginContainer/VBoxContainer/VBoxContainer2/SelectVBox/Select,
]


func _ready():
	select_vbox.visible = false
	update_characters()
	
	for button in buttons:
		button.connect("mouse_entered", self, "_on_Button_mouse_entered",
				[button])
		button.connect("pressed", self, "_on_Button_pressed",
				[button])


func update_characters():
	for character_name in Data.character_data:
		var character_data = Data.character_data[character_name]
		
		if character_data.guild_level > SaveData.guild_level:
			continue
		
		var character = CHARACTER_RESOURCE.instance()
		characters.add_child(character)
		character.character_name = character_name
		if not character.update_character():
			character.queue_free()
			continue
		character.character_button.connect("pressed", self,
				"_on_Character_pressed", [character])


func _on_ScrollLeft_gui_input(event):
	if event is InputEventMouseButton and \
			event.button_index == BUTTON_LEFT:
		_scroll_left_held = event.pressed


func _on_ScrollRight_gui_input(event):
	if event is InputEventMouseButton and \
			event.button_index == BUTTON_LEFT:
		_scroll_right_held = event.pressed


func _process(delta):
	if _scroll_left_held:
		_scroll_value -= SCROLL_AMOUNT * delta
	if _scroll_right_held:
		_scroll_value += SCROLL_AMOUNT * delta
	_scroll_value = max(_scroll_value, 0)
	characters_scroll.scroll_horizontal = _scroll_value
	if int(_scroll_value) > characters_scroll.scroll_horizontal:
		_scroll_value = characters_scroll.scroll_horizontal


func _on_Character_pressed(character):
	select_vbox.visible = true
	selected_character_name = character.character_name
	name_label.text = character.character_name
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
			character.rect_global_position, -15)


func _on_Select_pressed():
	SaveData.character_name = selected_character_name
	Transitions.change_scene_double_transition("res://World/World.tscn",
		Transitions.Transition_Type.SHRINKING_CIRCLE, 0.2)


func _on_Button_pressed(button):
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK,
			button.rect_global_position, -15)


func _on_Button_mouse_entered(button):
	AudioSystem.play_sfx(AudioSystem.SFX.BUTTON_CLICK_SHORT,
			button.rect_global_position, -20)
