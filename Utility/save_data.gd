extends Node

var world_position := Vector2(2578, 1517)
var character_name := "Jason"
var coins := 100
var inventory := [
	"Stick", "Devilfork",
	]
var equipped_weapon := "Devilfork"
var equipped_armor: String
var max_health := 10
var health := 10

var chest_contents = []

var guild_level := 1
var coins_deposited := 0
