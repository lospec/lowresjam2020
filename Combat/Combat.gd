extends Node

onready var combat_menu = $CombatMenu
onready var enemy_combat = $EnemyCombat
onready var player_combat = $PlayerCombat
onready var combat_label = $CombatMenu/VBoxContainer/PlayerHUD/ChoiceHUD/CenterContainer/CombatLabel

enum CombatAction { QUICK, COUNTER, HEAVY, INVALID = -1 }
var StringToCombatAction = {
	"quick": CombatAction.QUICK,
	"counter": CombatAction.COUNTER,
	"heavy": CombatAction.HEAVY}
func GetActionWeakness(action):
	match (action):
		CombatAction.QUICK:
			return CombatAction.COUNTER
			
		CombatAction.COUNTER:
			return CombatAction.HEAVY
			
		CombatAction.HEAVY:
			return CombatAction.QUICK
	
	return CombatAction.INVALID
func ActionCompare(action1, action2):
	if (action1 == action2):
		return 0
	if (GetActionWeakness(action2) == action1):
		return 1
	if (GetActionWeakness(action1) == action2):
		return 2
	return -1

func TakeTurn(playerAction):
	var enemyAction = playerAction#enemyCombat.GetAction()
	var win = ActionCompare(playerAction, enemyAction)
	
	combat_menu.ShowCombatLabel()
	
	if (win == 0): yield(Tie(playerAction), "completed")
	if (win == 1): yield(PlayerWin(playerAction), "completed")
	if (win == 2): yield(EnemyWin(playerAction), "completed")
	
	combat_menu.ResetUI()

func PlayerWin(playerAction):
	combat_label.text = "Player win case is still in progress"
	yield(get_tree().create_timer(1.5), "timeout")

func EnemyWin(enemyAction):
	combat_label.text = "Enemy win case is still in progress"
	yield(get_tree().create_timer(1.5), "timeout")

func Tie(action):
	var enemyDmg = enemy_combat.GetDamage();
	var playerDmg = player_combat.GetDamage();
	
	match (action):
		CombatAction.QUICK:
			combat_label.text = "Both of you attack"
			yield(get_tree().create_timer(1.5), "timeout")
			enemy_combat.hp -= playerDmg
			combat_label.text = "The Enemy takes " + str(playerDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")
			player_combat.hp -= enemyDmg
			combat_label.text = "You take " + str(enemyDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")		
		
		CombatAction.COUNTER:
			combat_label.text = "You prepare to counter"
			yield(get_tree().create_timer(1.5), "timeout")
			combat_label.text = "But nothing happened"
			yield(get_tree().create_timer(1.5), "timeout")
		
		CombatAction.HEAVY:
			combat_label.text = "You charges up your attack"
			yield(get_tree().create_timer(1.5), "timeout")
			combat_label.text = "The enemy also charges up!"
			yield(get_tree().create_timer(1.5), "timeout")
			playerDmg = playerDmg / 2
			enemy_combat.hp -= playerDmg
			combat_label.text = "The Enemy take " + str(playerDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")
			enemyDmg = enemyDmg / 2
			player_combat.hp -= enemyDmg
			combat_label.text = "You take " + str(enemyDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")
		
		_:
			yield(get_tree(), "idle_frame")

func _on_AttackButtons_pressed(attackType):
	attackType = attackType.to_lower()
	TakeTurn(StringToCombatAction.get(attackType, CombatAction.INVALID));
