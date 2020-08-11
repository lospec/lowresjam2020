extends Node

const BaseEntity = preload("res://Entities/BaseEntity/BaseEntity.gd")
const AI_State = preload("res://AI/AI_State.gd")

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

signal on_collision(_stateMachine, slide_count)


func _ready():
	origin_position = entity.position
	behaviour.set_starting_state(self)


func _process(_delta):
	_update_current_state(_delta)
	if entity.get_slide_count() > 0:
		emit_signal("on_collision", self)


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
	entity.velocity = Vector2.ZERO
	current_state.update_state(self, _delta)


func transition_to_state(state_index):
	if state_index == -1:
		return false

	var state = behaviour.states[state_index]
	if not state:
		return false
	current_state = state
	current_state._on_start(self)
	return true


func find_bodies_in_range(range_size: float):
	if _collision_circle_size != range_size:
		_collision_circle_size = range_size
	return _circle_area.get_overlapping_bodies()
