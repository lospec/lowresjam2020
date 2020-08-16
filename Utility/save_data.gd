extends Node

var world_position := Vector2(2578, 1517)
var character_name := "Jason"
var coins := 100
var inventory := [
	"Stick", "Hotdog",
]
var equipped_weapon := "Stick"
var equipped_armor: String
var max_health := 20
var health := 20

var chest_contents = []

var guild_level := 1
var coins_deposited := 0
