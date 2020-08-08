extends MarginContainer

var combat_util = preload("res://Combat/CombatUtil.gd")

onready var compare_container = $CompareContainer
onready var line_label = $LineLabel
onready var win_container = $WinResultContainer
onready var player_action_label = $CompareContainer/PlayerTurnContainer/ActionLabel
onready var enemy_action_label = $CompareContainer/EnemyTurnContainer/ActionLabel
onready var win_actor_label = $WinResultContainer/ActorLabel
onready var win_action_label = $WinResultContainer/ActionLabel

func show_turn_compare(playerAction, enemyAction, duration = 0):
	player_action_label.text = combat_util.GetActionName(playerAction)
	player_action_label.modulate = combat_util.GetActionColor(playerAction)
	
	enemy_action_label.text = combat_util.GetActionName(enemyAction)
	enemy_action_label.modulate = combat_util.GetActionColor(enemyAction)
	
	line_label.visible = true
	compare_container.visible = true
	
	if duration > 0:
		yield(get_tree().create_timer(duration), "timeout")
		line_label.visible = false
		compare_container.visible = false

func set_win_label(actor: String, action: int):
	win_actor_label.text = actor
	win_action_label.text = combat_util.GetActionName(action)
	win_action_label.modulate = combat_util.GetActionColor(action)

func show_win_result(playerAction, enemyAction, duration = 0):
	if playerAction == combat_util.Combat_Action.FLEE:
		yield(get_tree(), "idle_frame")
		return
	
	if enemyAction == combat_util.Combat_Action.FLEE:
		yield(get_tree(), "idle_frame")
		return
	
	var win = combat_util.ActionCompare(playerAction, enemyAction)
	match win:
		0: set_win_label("TIE", playerAction)
		1: set_win_label("Player Win", playerAction)
		2: set_win_label("Enemy Win", enemyAction)
		_: set_win_label("ERROR", combat_util.Combat_Action.INVALID)
	
	win_container.visible = true
	
	if duration > 0:
		yield(get_tree().create_timer(duration), "timeout")
		win_container.visible = false
