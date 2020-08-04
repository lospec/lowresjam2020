extends Control

const AI_State = preload("res://AI/AI_State.gd")


class Actions:
	const MoveToTarget = preload("res://AI/Actions/AI_Action_MoveToTarget.gd")
	const ReturnToOrigin = preload("res://AI/Actions/AI_Action_ReturnToOrigin.gd")
	const SearchForPlayer = preload("res://AI/Actions/AI_Action_SearchForPlayer.gd")
	const WaitTime = preload("res://AI/Actions/AI_Action_WaitTime.gd")


class Conditions:
	const FarFromOrigin = preload("res://AI/Conditions/AI_Condition_FarFromOrigin.gd")
	const HasTarget = preload("res://AI/Conditions/AI_Condition_HasTarget.gd")
	const TargetInRange = preload("res://AI/Conditions/AI_Condition_TargetInRange.gd")


onready var action_list = [
	Actions.MoveToTarget, Actions.ReturnToOrigin, Actions.SearchForPlayer, Actions.WaitTime
]

onready var condition_list = [
	Conditions.FarFromOrigin, Conditions.HasTarget, Conditions.TargetInRange
]

onready var pixel_perfect = $"/root/PixelPerfectScaling"
var _initial_screen_size

static func _get_name_of_script(script: Script):
	var path = script.resource_path
	var name = path.split("/")[-1].split("_")[-1].split(".")[0]
	return name

onready var BASE_BUTTON = $BASE_BUTTON
onready var BASE_REMOVABLE_BUTTON = $BASE_REMOVABLE_BUTTON

# Main
onready var main_control = $Main
onready var main_new_behaviour_button = $Main/VBoxContainer/NewBehaviourButton

# behaviour
onready var behaviour_control = $Behaviour
onready var behaviour_name_edit = $Behaviour/VBoxContainer/LineEdit
onready var behaviour_states_container = $Behaviour/ScrollContainer/GridContainer
onready var behaviour_add_state_button = $Behaviour/AddStateButton

# state
onready var state_control = $State
onready var state_behaviour_label = $State/BehaviourLabel
onready var state_add_action_button = $State/AddActionButton
onready var state_add_transition_button = $State/AddTransitionButton
onready var state_action_container = $State/ActionContainer/GridContainer
onready var state_transition_container = $State/TransitionContainer/GridContainer
onready var state_back_button = $State/BackButton
onready var state_state_name_edit = $State/VBoxContainer/LineEdit

# action
onready var action_control = $Action
onready var action_behaviour_label = $Action/BehaviourLabel
onready var action_state_label = $Action/StateLabel
onready var action_action_name_edit = $Action/VBoxContainer/LineEdit
onready var action_action_container = $Action/ActionContainer/GridContainer
onready var action_property_container = $Action/PropertyContainer/GridContainer
onready var action_confirm_buttom = $Action/ConfirmButton

onready var all_controls = [main_control, behaviour_control, state_control, action_control]

var _init_actions = false
var _selected_action: Resource

static func _get_valid_name(name: String, arr: Array) -> String:
	var real_name: String
	if name[-1] != '_':
		var result = true
		for item in arr:
			var resource: Resource = item
			if resource.resource_name == name:
				result = false
				break
		if result:
			return name

	for i in range(0, 100):
		real_name = name + str(i)
		var result = true
		for item in arr:
			var resource: Resource = item
			if resource.resource_name == real_name:
				result = false
				break
		if result:
			break
	return real_name


func _ready():
	_init_window()
	_show_control(main_control)
	_init_main_control()


func _show_control(control: Control):
	for item in all_controls:
		if item == control:
			item.show()
		else:
			item.hide()


func _init_main_control():
	main_new_behaviour_button.connect("button_down", self, "_on_main_new_behaviour_button_pressed")


func _init_behaviour_control(ai_behaviour: AI_Behaviour):
	_show_control(behaviour_control)
	behaviour_name_edit.text = ai_behaviour.resource_name
	behaviour_name_edit.connect(
		"text_changed", self, "_on_behaviour_name_edit_changed", [ai_behaviour]
	)
	behaviour_add_state_button.connect(
		"button_down", self, "_on_behaviour_new_state_button_pressed", [ai_behaviour]
	)
	for item in ai_behaviour.states:
		var state: AI_State = item
		var _button = _create_new_state_button(ai_behaviour, state)


func _get_button(removable_button: Control) -> Button:
	return removable_button.get_child(0) as Button


func _get_remove_button(removable_button: Control) -> Button:
	return removable_button.get_child(1) as Button


func _create_new_state_button(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	var removable = BASE_REMOVABLE_BUTTON.duplicate()
	var button = _get_button(removable)
	var remove = _get_remove_button(removable)
	button.text = ai_state.resource_name
	button.connect("button_down", self, "_on_behaviour_state_button_pressed", [ai_behaviour, ai_state])
	remove.connect(
		"button_down", self, "_remove_behaviour_state_button", [ai_behaviour, ai_state, removable]
	)
	removable.show()
	behaviour_states_container.add_child(removable)

func _on_behaviour_state_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	_clear_all_behaviour_state_button()
	_init_state_control(ai_behaviour, ai_state)

func _remove_behaviour_state_button(ai_behaviour: AI_Behaviour, ai_state: AI_State, removable: Control):
	var states: Array = ai_behaviour.states
	for i in range(0, states.size()):
		if states[i] == ai_state:
			states.remove(i)
			behaviour_states_container.remove_child(removable)
			break


func _on_behaviour_new_state_button_pressed(ai_behaviour: AI_Behaviour):
	var state = _create_new_state(ai_behaviour)
	_create_new_state_button(ai_behaviour, state)


func _clear_all_behaviour_state_button():
	behaviour_name_edit.disconnect("text_changed", self, "_on_behaviour_name_edit_changed")
	behaviour_add_state_button.disconnect(
		"button_down", self, "_on_behaviour_new_state_button_pressed"
	)
	_clear_behaviour_containers()
	
func _clear_behaviour_containers():
	var children: Array = behaviour_states_container.get_children()
	for i in range(0, children.size()):
		behaviour_states_container.remove_child(children[i])
	children.clear()
	
func _clear_state_containers():
	var children: Array = state_action_container.get_children()
	for i in range(0, children.size()):
		state_action_container.remove_child(children[i])
	children.clear()
	
	children = state_transition_container.get_children()
	for i in range(0, children.size()):
		state_transition_container.remove_child(children[i])
	children.clear()

static func _clear_node(node: Node):
	var children: Array = node.get_children()
	for i in range(0, children.size()):
		node.remove_child(children[i])
	children.clear()

func _init_state_control(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	_show_control(state_control)
	state_behaviour_label.text = ai_behaviour.resource_name
	state_back_button.connect("button_down", self, "_on_state_back_button_pressed", [ai_behaviour])
	state_state_name_edit.text = ai_state.resource_name
	state_state_name_edit.connect(
		"text_changed", self, "_on_state_name_changed", [ai_behaviour, ai_state]
	)
	# TODO: Actions
	state_add_action_button.connect("button_down", self, "_on_state_new_action_button_pressed", [ai_behaviour, ai_state])
	
	# TODO: Transitions
	
	# TODO: Get All Actions and Transitions
	for item in ai_state.actions:
		var action = item as Resource
		_add_action_to_state_container(ai_behaviour, ai_state, action)
		
func _add_action_to_state_container(ai_behaviour: AI_Behaviour, ai_state: AI_State, action: Resource):
	var removable = BASE_REMOVABLE_BUTTON.duplicate()
	var button = _get_button(removable)
	var remove = _get_remove_button(removable)
	button.text = action.resource_name
	button.connect("button_down", self, "_on_state_action_button_pressed", [ai_behaviour, ai_state, action])
	remove.connect(
		"button_down", self, "_remove_state_action_button", [ai_state, action, removable]
	)
	removable.show()
	state_action_container.add_child(removable)

func _remove_state_action_button(ai_state: AI_State, ai_action: Resource,removable: Control):
	var actions: Array = ai_state.actions
	for i in range(0, actions.size()):
		if actions[i] == ai_action:
			actions.remove(i)
			state_action_container.remove_child(removable)
			break


func _disconnect_state_control():
	state_back_button.disconnect("button_down", self, "_on_state_back_button_pressed")
	state_state_name_edit.disconnect("text_changed", self, "_on_state_name_changed")
	state_add_action_button.disconnect("button_down", self, "_on_state_new_action_button_pressed")
	# TODO: Disconnects!
	
func _on_state_new_action_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	_disconnect_state_control()
	_clear_state()
	_init_action_control(ai_behaviour, ai_state)
	

func _on_state_action_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_action: Resource):
	_disconnect_state_control()
	_clear_state()
	_init_action_control(ai_behaviour, ai_state, ai_action)

	
func _clear_state():
	_clear_node(state_action_container)
	_clear_node(state_transition_container)

func _disconnect_action_control():
	# TODO
	action_confirm_buttom.disconnect("button_down", self, "_on_action_confirm_buttom_pressed")
	action_action_name_edit.disconnect("text_changed", self, "_on_action_name_changed")

func _on_action_confirm_buttom_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_action: Resource):
	if  ai_action:
		ai_state.actions.remove(ai_state.actions.find(ai_action))
	ai_state.actions.append(_selected_action)
	_init_state_control(ai_behaviour,ai_state)
	_disconnect_action_control()
	_selected_action = null


func _create_button(name: String, parent_node : Node = null) -> Button:
	var button = BASE_BUTTON.duplicate()
	button.show()
	button.text = name
	if parent_node:
		parent_node.add_child(button)
	return button

func _init_action_control(ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_action: Resource = null):	
	_show_control(action_control)
	
	if ai_action:
		_selected_action = ai_action
		action_action_name_edit.text = ai_action.resource_name
		pass
		
	action_confirm_buttom.connect("button_down", self, "_on_action_confirm_buttom_pressed", [ai_behaviour, ai_state, ai_action])
	action_action_name_edit.connect("text_changed", self, "_on_action_name_changed", [ai_state])
	
	if _init_actions:
		return
		
	_init_actions = true
	for item in action_list:
		var action = item as Resource
		var name = _get_name_of_script(action)
		var button = _create_button(name, action_action_container)
		button.connect("button_down", self, "_on_action_action_button_pressed", [action])

func _on_action_name_changed(new_text: String, ai_state: AI_State):
	if not _selected_action:
		return 
	_selected_action.resource_name = _get_valid_name(new_text, ai_state.actions)


func _on_action_action_button_pressed(action: Script):
	_selected_action = action.new()
	var name = _get_name_of_script(action)
	_selected_action.resource_name = name
	action_action_name_edit.text = name
	_set_action_properties(action)
	
func _set_action_properties(action: Script):
	pass
	
func _on_state_new_transition_button_pressed():
	pass
	
func _init_transition_control():
	pass

func _on_state_name_changed(new_text: String, ai_behaviour: AI_Behaviour, ai_state: AI_State):
	ai_state.resource_name = _get_valid_name(new_text, ai_behaviour.states)


func _on_state_back_button_pressed(ai_behaviour: AI_Behaviour):
	_disconnect_state_control()
	_init_behaviour_control(ai_behaviour)
	_clear_state_containers()
	_clear_state()


func _create_new_state(ai_behaviour: AI_Behaviour):
	var state = AI_State.new()
	var name = "New_State_"
	state.resource_name = _get_valid_name(name, ai_behaviour.states)
	ai_behaviour.states.append(state)
	return state


func _create_new_behaviour():
	var behaviour = AI_Behaviour.new()
	behaviour.resource_name = "New_AI_Behaviour"
	return behaviour


func _on_behaviour_name_edit_changed(new_text: String, ai_behaviour: AI_Behaviour):
	ai_behaviour.resource_name = new_text


func _on_main_new_behaviour_button_pressed():
	var behaviour = _create_new_behaviour()
	_init_behaviour_control(behaviour)


func _init_window():
	get_tree().disconnect("screen_resized", pixel_perfect, "_on_screen_resized")
	var size = Vector2(256, 256)
	get_tree().set_screen_stretch(SceneTree.STRETCH_MODE_2D, SceneTree.STRETCH_ASPECT_KEEP, size)
	var root = get_tree().root
	root.size = size
	OS.set_window_size(size * 4)


func _exit_tree():
	var size = Vector2(64, 64)
	get_tree().set_screen_stretch(
		SceneTree.STRETCH_MODE_VIEWPORT, SceneTree.STRETCH_ASPECT_KEEP, size
	)
	var root = get_tree().root
	if get_tree().connect("screen_resized", pixel_perfect, "_on_screen_resized") != OK:
		push_error("Some problem reconnecting pixel perfect cam")
	root.size = size
	OS.set_window_size(size)
