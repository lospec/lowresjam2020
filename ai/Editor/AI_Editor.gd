extends Control

const AI_State = preload("res://AI/AI_State.gd")
const AI_Transition = preload("res://AI/AI_Transition.gd")


class Actions:
	const MoveToTarget = preload("res://AI/Actions/AI_Action_MoveToTarget.gd")
	const ReturnToOrigin = preload("res://AI/Actions/AI_Action_ReturnToOrigin.gd")
	const SearchForPlayer = preload("res://AI/Actions/AI_Action_SearchForPlayer.gd")
	const WaitTime = preload("res://AI/Actions/AI_Action_WaitTime.gd")
	const Wander = preload("res://AI/Actions/AI_Action_Wander.gd")
	const RunFromTarget = preload("res://AI/Actions/AI_Action_RunFromTarget.gd")


class Conditions:
	const FarFromOrigin = preload("res://AI/Conditions/AI_Condition_FarFromOrigin.gd")
	const HasTarget = preload("res://AI/Conditions/AI_Condition_HasTarget.gd")
	const TargetInRange = preload("res://AI/Conditions/AI_Condition_TargetInRange.gd")


onready var action_list = [
	Actions.MoveToTarget,
	 Actions.ReturnToOrigin,
	 Actions.SearchForPlayer,
	 Actions.WaitTime,
	 Actions.Wander,
	 Actions.RunFromTarget
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
onready var BASE_OPTIONS_BUTTON = $BASE_OPTIONS_BUTTON

# Main
onready var main_control = $Main
onready var main_new_behaviour_button = $Main/VBoxContainer/NewBehaviourButton
onready var main_load_behaviour_button = $Main/VBoxContainer/LoadBehaviourButton
onready var main_file_dialog = $Main/FileDialog

# behaviour
onready var behaviour_control = $Behaviour
onready var behaviour_name_edit = $Behaviour/VBoxContainer/LineEdit
onready var behaviour_states_container = $Behaviour/ScrollContainer/GridContainer
onready var behaviour_add_state_button = $Behaviour/AddStateButton
onready var behaviour_save_button = $Behaviour/SaveButton

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
onready var action_confirm_button = $Action/ConfirmButton

# condition
onready var condition_control = $Condition
onready var condition_behaviour_state_label = $Condition/BehaviourStateLabel
onready var condition_transition_label = $Condition/TransitionLabel
onready var condition_condition_name_edit = $Condition/VBoxContainer/LineEdit
onready var condition_condition_container = $Condition/ConditionContainer/GridContainer
onready var condition_property_container = $Condition/PropertyContainer/GridContainer
onready var condition_confirm_button = $Condition/ConfirmButton

# Transition
onready var transition_control = $Transition
onready var transition_confirm_button = $Transition/ConfirmButton
onready var transition_transition_name_edit = $Transition/VBoxContainer/LineEdit
onready var transition_behaviour_label = $Transition/BehaviourLabel
onready var transition_state_label = $Transition/StateLabel
onready var transition_condition_button = $Transition/ConditionButton
onready var transition_true_state_container = $Transition/TrueStateContainer/VBoxContainer
onready var transition_false_state_container = $Transition/FalseStateContainer/VBoxContainer
onready var transition_error_message = $Transition/ErrorMessage

onready var type_node_provider = {
	TYPE_BOOL: $TYPE_PROVIDER/BOOL, TYPE_INT: $TYPE_PROVIDER/INT, TYPE_REAL: $TYPE_PROVIDER/REAL
}

onready var all_controls = [
	main_control,
	behaviour_control,
	state_control,
	action_control,
	transition_control,
	condition_control
]

var _init_actions = false
var _init_conditions = false
var _selected_action: Resource
var _selected_condition: Resource

static func _get_valid_name(name: String, arr: Array) -> String:
	
	var real_name: String
	if name.length() > 0 and name[-1] != '_':
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
	main_load_behaviour_button.connect("button_down", self, "_on_main_load_behaviour_button_pressed")
	
func _on_main_load_behaviour_button_pressed():
	main_file_dialog.popup()
	main_file_dialog.connect("file_selected", self, "_on_main_file_selected")


func _on_main_file_selected(path: String):
	main_file_dialog.hide()
	var behaviour = ResourceLoader.load(path)
	_init_behaviour_control(behaviour)


func _init_behaviour_control(ai_behaviour: AI_Behaviour):
	_show_control(behaviour_control)
	behaviour_name_edit.text = ai_behaviour.resource_name
	behaviour_name_edit.connect(
		"text_changed", self, "_on_behaviour_name_edit_changed", [ai_behaviour]
	)
	behaviour_add_state_button.connect(
		"button_down", self, "_on_behaviour_new_state_button_pressed", [ai_behaviour]
	)
	
	behaviour_save_button.connect("button_down", self, "_on_save_behaviour_button_pressed", [ai_behaviour])
	for item in ai_behaviour.states:
		var state: AI_State = item
		var _button = _create_new_state_button(ai_behaviour, state)
		
func _on_save_behaviour_button_pressed(ai_behaviour: AI_Behaviour):
	if not ai_behaviour:
		return
	
	var path = "res://AI/Resources/" + ai_behaviour.resource_name + ".tres"
	print("saving resource to %s" % path)
	if ResourceLoader.exists(path):
		var dir = Directory.new()
		dir.remove(path)
		print("overwriting")
	ResourceSaver.save(path, ai_behaviour) 
	

func _get_button(removable_button: Control) -> Button:
	return removable_button.get_child(0) as Button


func _get_remove_button(removable_button: Control) -> Button:
	return removable_button.get_child(1) as Button


func _create_new_state_button(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	var removable = BASE_REMOVABLE_BUTTON.duplicate()
	var button = _get_button(removable)
	var remove = _get_remove_button(removable)
	button.text = ai_state.resource_name
	button.connect(
		"button_down", self, "_on_behaviour_state_button_pressed", [ai_behaviour, ai_state]
	)
	remove.connect(
		"button_down", self, "_remove_behaviour_state_button", [ai_behaviour, ai_state, removable]
	)
	removable.show()
	behaviour_states_container.add_child(removable)


func _on_behaviour_state_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	_disconnect_behaviour_control()
	_clear_node(behaviour_states_container)
	_init_state_control(ai_behaviour, ai_state)


func _remove_behaviour_state_button(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, removable: Control
):
	var states: Array = ai_behaviour.states
	for i in range(0, states.size()):
		if states[i] == ai_state:
			states.remove(i)
			behaviour_states_container.remove_child(removable)
			break


func _on_behaviour_new_state_button_pressed(ai_behaviour: AI_Behaviour):
	var state = _create_new_state(ai_behaviour)
	_create_new_state_button(ai_behaviour, state)


func _disconnect_behaviour_control():
	behaviour_name_edit.disconnect("text_changed", self, "_on_behaviour_name_edit_changed")
	behaviour_add_state_button.disconnect(
		"button_down", self, "_on_behaviour_new_state_button_pressed"
	)
	behaviour_save_button.disconnect("button_down", self, "_on_save_behaviour_button_pressed")


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
	state_add_action_button.connect(
		"button_down", self, "_on_state_new_action_button_pressed", [ai_behaviour, ai_state]
	)
	# TODO: Transitions
	state_add_transition_button.connect(
		"button_down", self, "_on_state_new_transition_button_pressed", [ai_behaviour, ai_state]
	)

	# TODO: Get All Actions and Transitions
	for item in ai_state.actions:
		var action = item as Resource
		_add_action_to_state_container(ai_behaviour, ai_state, action)

	for item in ai_state.transitions:
		var transition = item as AI_Transition
		_add_transition_to_state_container(ai_behaviour, ai_state, transition)


func _add_action_to_state_container(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, action: Resource
):
	if not action:
		return

	var removable = BASE_REMOVABLE_BUTTON.duplicate()
	var button = _get_button(removable)
	var remove = _get_remove_button(removable)
	button.text = action.resource_name
	button.connect(
		"button_down", self, "_on_state_action_button_pressed", [ai_behaviour, ai_state, action]
	)
	remove.connect(
		"button_down", self, "_remove_state_action_button", [ai_state, action, removable]
	)
	removable.show()
	state_action_container.add_child(removable)


func _add_transition_to_state_container(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition
):
	if not ai_transition:
		return
	var removable = BASE_REMOVABLE_BUTTON.duplicate()
	var button = _get_button(removable)
	var remove = _get_remove_button(removable)
	button.text = ai_transition.resource_name
	button.connect(
		"button_down",
		self,
		"_on_state_transition_button_pressed",
		[ai_behaviour, ai_state, ai_transition]
	)
	remove.connect(
		"button_down", self, "_remove_state_transition_button", [ai_state, ai_transition, removable]
	)
	removable.show()
	state_transition_container.add_child(removable)


func _remove_state_transition_button(
	ai_state: AI_State, ai_transition: AI_Transition, removable: Control
):
	var transitions: Array = ai_state.transitions
	var idx = transitions.find(ai_transition)
	if idx != -1:
		transitions.remove(idx)
		state_transition_container.remove_child(removable)


func _remove_state_action_button(ai_state: AI_State, ai_action: Resource, removable: Control):
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
	state_add_transition_button.disconnect(
		"button_down", self, "_on_state_new_transition_button_pressed"
	)
	# TODO: Disconnects!


func _on_state_new_action_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	_disconnect_state_control()
	_clear_node(state_action_container)
	_clear_node(state_transition_container)
	_init_action_control(ai_behaviour, ai_state)


func _on_state_action_button_pressed(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_action: Resource
):
	_disconnect_state_control()
	_clear_node(state_action_container)
	_clear_node(state_transition_container)
	_init_action_control(ai_behaviour, ai_state, ai_action)


func _on_state_transition_button_pressed(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition = null
):
	_disconnect_state_control()
	_clear_node(state_action_container)
	_clear_node(state_transition_container)
	_init_transition_control(ai_behaviour, ai_state, ai_transition)


func _disconnect_action_control():
	# TODO
	action_confirm_button.disconnect("button_down", self, "_on_action_confirm_button_pressed")
	action_action_name_edit.disconnect("text_changed", self, "_on_action_name_changed")


func _on_action_confirm_button_pressed(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_action: Resource
):
	if ai_action:
		ai_state.actions.remove(ai_state.actions.find(ai_action))
	if _selected_action:
		ai_state.actions.append(_selected_action)
		
	_init_state_control(ai_behaviour, ai_state)
	_disconnect_action_control()
	_clear_node(action_property_container)
	_selected_action = null


func _create_button(name: String, parent_node: Node = null) -> Button:
	var button = BASE_BUTTON.duplicate()
	button.show()
	button.text = name
	if parent_node:
		parent_node.add_child(button)
	return button


func _init_action_control(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_action: Resource = null
):
	_show_control(action_control)

	action_behaviour_label.text = ai_behaviour.resource_name
	action_state_label.text = ai_state.resource_name
	action_action_name_edit.text = ""

	if ai_action:
		_selected_action = ai_action
		action_action_name_edit.text = ai_action.resource_name
		_set_action_properties(ai_action)

	action_confirm_button.connect(
		"button_down",
		self,
		"_on_action_confirm_button_pressed",
		[ai_behaviour, ai_state, ai_action]
	)
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
	_set_action_properties(_selected_action)


func _set_action_properties(action: Resource):
	_clear_node(action_property_container)
	var p_infos: Array = action.get_property_list()
	for i in range(0, p_infos.size()):
		if p_infos[i]["name"] == "Script Variables":
			p_infos = p_infos.slice(i + 1, p_infos.size() - 1)
			break
	for i in range(p_infos.size(), 0, -1):
		if p_infos[i - 1]["name"][0] == "_":
			p_infos.remove(i - 1)

	for p_info in p_infos:
		_action_add_property(action, p_info["name"], p_info["type"])


func _action_add_property(action: Resource, name: String, type: int):
	var node: Control
	var property_field: Label
	match type:
		TYPE_INT:
			node = type_node_provider[TYPE_INT].duplicate()
			var int_field: SpinBox = _get_value_field(node)
			int_field.value = action.get(name)
			int_field.connect("value_changed", self, "_validate_int", [action, name])
		TYPE_BOOL:
			node = type_node_provider[TYPE_BOOL].duplicate()
			var bool_field: CheckBox = _get_value_field(node)
			bool_field.pressed = action.get(name)
			bool_field.connect("toggled", self, "_validate_bool", [action, name])
		TYPE_REAL:
			node = type_node_provider[TYPE_REAL].duplicate()
			var float_field: SpinBox = _get_value_field(node)
			float_field.value = action.get(name)
			float_field.connect("value_changed", self, "_validate_float", [action, name])
		_:
			return

	node.show()
	property_field = _get_property_field(node)
	property_field.text = name
	action_property_container.add_child(node)


func _get_property_field(node: Control):
	return node.get_child(0)


func _get_value_field(node: Control):
	return node.get_child(1)


func _validate_int(value: float, action: Resource, p_name: String):
	action.set(p_name, int(value))


func _validate_float(value: float, action: Resource, p_name: String):
	action.set(p_name, float(value))


func _validate_bool(value: bool, action: Resource, p_name: String):
	action.set(p_name, value)


func _on_state_new_transition_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State):
	_disconnect_state_control()
	_clear_node(state_action_container)
	_clear_node(state_transition_container)
	_init_transition_control(ai_behaviour, ai_state)

func _on_transition_condition_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition, ai_condition: Resource):
	_disconnect_transition_control()
	_clear_node(transition_true_state_container)
	_clear_node(transition_false_state_container)
	_init_condition_control(ai_behaviour, ai_state, ai_transition, ai_condition)

func _init_condition_control(ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition, ai_condition: Resource):
	_show_control(condition_control)
	
	condition_condition_name_edit.text = ""
	if ai_condition:
		_selected_condition = ai_condition
		condition_condition_name_edit.text = ai_condition.resource_name
		_set_condition_properties(ai_condition)
	
	condition_behaviour_state_label.text = "{} - {}".format([ai_behaviour.resource_name, ai_state.resource_name], "{}")
	condition_transition_label.text = ai_transition.resource_name
	
	condition_condition_name_edit.connect("text_changed", self, "_on_condition_name_changed")
	condition_confirm_button.connect("button_down", self, "_on_condition_confirm_button_pressed", [ai_behaviour, ai_state, ai_transition])

	if _init_conditions:
		return
	
	_init_conditions = true
	for item in condition_list:
		var condition = item as Resource
		var name = _get_name_of_script(condition)
		var button = _create_button(name, condition_condition_container)
		button.connect("button_down", self, "_on_condition_condition_button_pressed", [condition])

func _on_condition_condition_button_pressed(condition: Resource):
	_selected_condition = condition.new()
	var name = _get_name_of_script(condition)
	_selected_condition.resource_name = name
	condition_condition_name_edit.text = name
	_set_condition_properties(_selected_condition)

func _set_condition_properties(ai_condition: Resource):
	_clear_node(condition_property_container)
	var p_infos: Array = ai_condition.get_property_list()
	for i in range(0, p_infos.size()):
		if p_infos[i]["name"] == "Script Variables":
			p_infos = p_infos.slice(i + 1, p_infos.size() - 1)
			break
	for i in range(p_infos.size(), 0, -1):
		if p_infos[i - 1]["name"][0] == "_":
			p_infos.remove(i - 1)

	for p_info in p_infos:
		_condition_add_property(ai_condition, p_info["name"], p_info["type"])

func _condition_add_property(ai_condition: Resource, name: String, type: int):
	var node: Control
	var property_field: Label
	match type:
		TYPE_INT:
			node = type_node_provider[TYPE_INT].duplicate()
			var int_field: SpinBox = _get_value_field(node)
			int_field.value = ai_condition.get(name)
			int_field.connect("value_changed", self, "_validate_int", [ai_condition, name])
		TYPE_BOOL:
			node = type_node_provider[TYPE_BOOL].duplicate()
			var bool_field: CheckBox = _get_value_field(node)
			bool_field.pressed = ai_condition.get(name)
			bool_field.connect("toggled", self, "_validate_bool", [ai_condition, name])
		TYPE_REAL:
			node = type_node_provider[TYPE_REAL].duplicate()
			var float_field: SpinBox = _get_value_field(node)
			float_field.value = ai_condition.get(name)
			float_field.connect("value_changed", self, "_validate_float", [ai_condition, name])
		_:
			return

	node.show()
	property_field = _get_property_field(node)
	property_field.text = name
	condition_property_container.add_child(node)

func _disconnect_condition_control():
	condition_condition_name_edit.disconnect("text_changed", self, "_on_condition_name_changed")
	condition_confirm_button.disconnect("button_down", self, "_on_condition_confirm_button_pressed")

func _on_condition_confirm_button_pressed(ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition):
	ai_transition.condition = _selected_condition
	_init_transition_control(ai_behaviour, ai_state, ai_transition)
	_disconnect_condition_control()
	_clear_node(condition_property_container)
	_selected_condition = null

func _on_condition_name_changed(new_text: String):
	if not _selected_condition:
		return
	_selected_condition.resource_name = new_text

func _condition_control():
	pass


func _init_transition_control(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition = null
):
	_show_control(transition_control)

	transition_error_message.hide()

	transition_behaviour_label.text = ai_behaviour.resource_name
	transition_state_label.text = ai_state.resource_name

	if not ai_transition:
		ai_transition = AI_Transition.new()
		ai_transition.resource_name = _get_valid_name("New_Transition_", ai_state.transitions)
		ai_transition.true_state_index = -1
		ai_transition.false_state_index = -1

	transition_transition_name_edit.text = ai_transition.resource_name
	transition_transition_name_edit.connect(
		"text_changed", self, "_on_transition_name_changed", [ai_state, ai_transition]
	)
	transition_confirm_button.connect(
		"button_down",
		self,
		"_on_transition_confirm_button_pressed",
		[ai_behaviour, ai_state, ai_transition]
	)

	_set_true_false_state_options(ai_behaviour, ai_state, ai_transition)
		
	transition_condition_button.text = "+"
	if ai_transition.condition:
		transition_condition_button.text = ai_transition.condition.resource_name
	
	transition_condition_button.connect("button_down", self, "_on_transition_condition_button_pressed", [ai_behaviour, ai_state, ai_transition, ai_transition.condition])


func _set_true_false_state_options(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition
):
	var true_group: ButtonGroup = ButtonGroup.new()
	var false_group: ButtonGroup = ButtonGroup.new()
	for i in range(-1, ai_behaviour.states.size()):
		var state: AI_State = ai_behaviour.states[i] if i != -1 else null
		if state == ai_state:
			continue

		var option = BASE_OPTIONS_BUTTON.duplicate()
		option.show()
		_get_option_label(option).text = state.resource_name if state else "continue"

		var true_option = option
		var false_option = option.duplicate()

		var true_checkbox = _get_option_checkbox(true_option)
		var false_checkbox = _get_option_checkbox(false_option)

		true_checkbox.connect(
			"toggled", self, "_on_true_state_option_toggled", [ai_behaviour, state, ai_transition]
		)
		false_checkbox.connect(
			"toggled", self, "_on_false_state_option_toggled", [ai_behaviour, state, ai_transition]
		)

		true_checkbox.group = true_group
		false_checkbox.group = false_group

		true_checkbox.pressed = i == ai_transition.true_state_index
		false_checkbox.pressed = i == ai_transition.false_state_index

		transition_true_state_container.add_child(true_option)
		transition_false_state_container.add_child(false_option)


func _on_true_state_option_toggled(
	value: bool, ai_behavior: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition
):
	transition_error_message.hide()
	if not value:
		return
	var idx = ai_behavior.states.find(ai_state)
	ai_transition.true_state_index = idx


func _on_false_state_option_toggled(
	value: bool, ai_behavior: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition
):
	transition_error_message.hide()
	if not value:
		return
	var idx = ai_behavior.states.find(ai_state)
	ai_transition.false_state_index = idx


static func _get_option_label(option: Control) -> Label:
	return option.get_child(1) as Label

static func _get_option_checkbox(option: Control) -> CheckBox:
	return option.get_child(0) as CheckBox


func _on_state_name_changed(new_text: String, ai_behaviour: AI_Behaviour, ai_state: AI_State):
	ai_state.resource_name = _get_valid_name(new_text, ai_behaviour.states)


func _on_transition_name_changed(new_text: String, ai_state: AI_State, ai_transition: AI_Transition):
	ai_transition.resource_name = _get_valid_name(new_text, ai_state.transitions)


func _on_transition_confirm_button_pressed(
	ai_behaviour: AI_Behaviour, ai_state: AI_State, ai_transition: AI_Transition
):
	if (
		ai_transition.true_state_index == ai_transition.false_state_index
		and ai_transition.true_state_index != -1
		and ai_transition.false_state_index != -1
	):
		transition_error_message.show()
		return

	if ai_transition:
		var idx = ai_state.transitions.find(ai_transition)
		if idx == -1:
			print("appending new transition")
			ai_state.transitions.append(ai_transition)
		ai_state.transitions[idx] = ai_transition
		
	_init_state_control(ai_behaviour, ai_state)
	_disconnect_transition_control()
	_clear_node(transition_true_state_container)
	_clear_node(transition_false_state_container)


func _disconnect_transition_control():
	transition_transition_name_edit.disconnect("text_changed", self, "_on_transition_name_changed")
	transition_confirm_button.disconnect(
		"button_down", self, "_on_transition_confirm_button_pressed"
	)
	transition_condition_button.disconnect("button_down", self, "_on_transition_condition_button_pressed")


func _on_state_back_button_pressed(ai_behaviour: AI_Behaviour):
	_disconnect_state_control()
	_init_behaviour_control(ai_behaviour)
	_clear_node(state_action_container)
	_clear_node(state_transition_container)


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
