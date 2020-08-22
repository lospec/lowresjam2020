extends CanvasLayer

# Constants
enum Transition_Type {
	SHRINKING_CIRCLE,
	MULTIPLE_SQUARES,
	MULTIPLE_CIRCLES_FILLED,
	LINES,
	SWIRL,
	BLOCKS,
}
const TRANSITION_TEXTURES = {
	Transition_Type.SHRINKING_CIRCLE: preload("res://Transitions/shrinking_circle.png"),
	Transition_Type.MULTIPLE_SQUARES: preload("res://Transitions/multiple_squares.png"),
	Transition_Type.MULTIPLE_CIRCLES_FILLED: preload("res://Transitions/multiple_circles_filled.png"),
	Transition_Type.LINES: preload("res://Transitions/lines.png"),
	Transition_Type.SWIRL: preload("res://Transitions/lines.png"),
	Transition_Type.BLOCKS: preload("res://Transitions/blocks.png"),
}


# Onready Variables
onready var transition = $Transition
onready var transition_shader = transition.material
onready var tween = $Tween


func change_scene(scene_path: String,
		transition_type: int, duration: float,
		invert: bool = false,
		color: Color = Color.black,
		transition_feather: float = 0.2,
		tween_trans_type: int = Tween.TRANS_LINEAR,
		tween_ease_type: int = Tween.EASE_IN_OUT) -> void:
	var error = get_tree().change_scene(scene_path)
	if error != OK:
		print_debug("Error %s occured while attempting to change scene." % error)
	
	start_transition(transition_type, duration, invert, color,
			transition_feather, tween_trans_type, tween_ease_type)


func change_scene_double_transition(scene_path: String,
		transition_type: int, duration: float,
		invert: bool = false,
		color: Color = Color.black,
		transition_feather: float = 0.2,
		tween_trans_type: int = Tween.TRANS_LINEAR,
		tween_ease_type: int = Tween.EASE_IN_OUT) -> void:
	
	start_transition(transition_type, duration, not invert,
			color, transition_feather,
			tween_trans_type, tween_ease_type)
	yield(tween, "tween_completed")
	change_scene(scene_path, transition_type, duration, invert,
			color, transition_feather,
			tween_trans_type, tween_ease_type)


func start_transition(transition_type: int, duration: float,
		invert: bool = false,
		color: Color = Color.black,
		transition_feather: float = 0.2,
		tween_trans_type: int = Tween.TRANS_LINEAR,
		tween_ease_type: int = Tween.EASE_IN_OUT):
	transition_shader.set_shader_param("mask_tex", TRANSITION_TEXTURES[transition_type])
	transition_shader.set_shader_param("transition_invert_mask", invert)
	transition_shader.set_shader_param("transition_col", color)
	transition_shader.set_shader_param("transition_feather", transition_feather)
	transition_shader.set_shader_param("transition_time", 1)
	
	tween.interpolate_property(transition_shader,
			"shader_param/transition_time",
			1, 0, duration, tween_trans_type, tween_ease_type)
	tween.start()
