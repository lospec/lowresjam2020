extends Button

# SUPA SIMPLE TRANSITION ANIMATION DEMONSTRATION
# MMXX @ ZIK (wow GDScript feels really new & revolutionary to me :grin:)

# Transition state variable that determines if
# the animator should play fade-in or the fade-out animation
var transition_fadein = true;

# Table/Dict for converting transition state to text
var transition_tostr = {true: "IN", false: "OUT"};

# Table/Dict for converting transition state to animation name
var transition_toanim = {true: "FadeIn", false: "FadeOut"};

# AnimationPlayer node in transition-rendering texturerect
onready var transition_anim_player = get_parent().get_node("TransitionQuad/AnimationPlayer");

# Called when the node enters the scene tree for the first time.
func _ready():
	# Play the animation according to transition state
	transition_anim_player.play(transition_toanim[transition_fadein]);
	
	# Reset transition state & Update the button text
	transition_fadein = false;
	self.text = transition_tostr[transition_fadein];

# Called when the user presses the fadein/out button
func _on_Button_pressed():
	# Play the animation according to transition state
	transition_anim_player.play(transition_toanim[transition_fadein]);
	
	# Switch the transition state and change the button's text accordingly
	if transition_fadein:
		transition_fadein = false;
	else:
		transition_fadein = true;
	self.text = transition_tostr[transition_fadein];	
