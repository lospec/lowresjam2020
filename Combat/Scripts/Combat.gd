extends Node

onready var combatMenu = $CombatMenu
onready var enemyCombat = $EnemyCombat
onready var playerCombat = $PlayerCombat
onready var combatLabel = $CombatMenu/Label

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
	
	combatMenu.ShowCombatLabel()
	
	if (win == 0): yield(Tie(playerAction), "completed")
	if (win == 1): yield(PlayerWin(playerAction), "completed")
	if (win == 2): yield(EnemyWin(playerAction), "completed")
	
	combatMenu.ResetUI()

func PlayerWin(playerAction):
	combatLabel.text = "Player win case is still in progress"
	yield(get_tree().create_timer(1.5), "timeout")

func EnemyWin(enemyAction):
	combatLabel.text = "Enemy win case is still in progress"
	yield(get_tree().create_timer(1.5), "timeout")

func Tie(action):
	print(action)
	var enemyDmg = enemyCombat.GetDamage();
	var playerDmg = playerCombat.GetDamage();
	
	match (action):
		CombatAction.QUICK:
			combatLabel.text = "Both of you attacks"
			yield(get_tree().create_timer(1.5), "timeout")
			enemyCombat.hp -= playerDmg
			combatLabel.text = "The Enemy take " + str(playerDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")
			playerCombat.hp -= enemyDmg
			combatLabel.text = "You take " + str(enemyDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")		
		
		CombatAction.COUNTER:
			combatLabel.text = "You prepare to counter"
			yield(get_tree().create_timer(1.5), "timeout")
			combatLabel.text = "But nothing happened"
			yield(get_tree().create_timer(1.5), "timeout")
		
		CombatAction.HEAVY:
			combatLabel.text = "You charges up your attack"
			yield(get_tree().create_timer(1.5), "timeout")
			combatLabel.text = "The enemy also charges up!"
			yield(get_tree().create_timer(1.5), "timeout")
			playerDmg = playerDmg / 2
			enemyCombat.hp -= playerDmg
			combatLabel.text = "The Enemy take " + str(playerDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")
			enemyDmg = enemyDmg / 2
			playerCombat.hp -= enemyDmg
			combatLabel.text = "You take " + str(enemyDmg) + " damage"
			yield(get_tree().create_timer(1.5), "timeout")
		
		_:
			yield(get_tree(), "idle_frame")

func _on_AttackButtons_pressed(attackType):
	attackType = attackType.to_lower()
	TakeTurn(StringToCombatAction.get(attackType, CombatAction.INVALID));

