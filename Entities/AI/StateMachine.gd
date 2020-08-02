extends Node

const BaseEntity = preload("res://Entities/BaseEntity/BaseEntity.gd")

export (bool) var active
export (Resource) var behaviour

onready var entity := get_parent() as BaseEntity
onready var _circle_area = $CircleArea2D
onready var _collision_circle_shape = $CircleArea2D/CollisionShape2D

var current_state: AI_State = null
var target: BaseEntity
var distance_to_target: float setget , _get_distance_to_target
var distance_to_origin: float setget , _get_distance_to_origin
var _collision_circle_size: float setget _set_collision_circle_size, _get_collision_circle_size

var origin_position: Vector2


func _ready():
	origin_position = entity.position
	current_state = behaviour.start_state


func _process(_delta):
	_update_current_state(_delta)


func _get_collision_circle_size() -> float:
	return _collision_circle_shape.shape.radius


func _set_collision_circle_size(value: float):
	_collision_circle_shape.shape.radius = value


func _get_distance_to_origin() -> float:
	return entity.position.distance_to(origin_position)


func _get_distance_to_target() -> float:
	if target:
		return entity.position.distance_to(target.transform.get_origin())
	return -1.0


func _update_current_state(_delta):
	if not active:
		return
	current_state.update_state(self, _delta)


func transition_to_state(state_index):
	if state_index == -1:
		return false
		
	var state = behaviour.states[state_index]
	if not state:
		return false
	current_state = state
	return true


func find_bodies_in_range(range_size: float):
	_collision_circle_size = range_size
	return _circle_area.get_overlapping_bodies()
