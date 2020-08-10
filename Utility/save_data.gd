extends Node

var world_position: Vector2
var coins := 100
var inventory := [
	"Stick", "Gem", "Hotdog", "Leather",
	"Knife", "Gem", "Stick", "Stick",
	"Stick", "Stick", "Stick", "Stick",
	"Stick",
	]
var equipped_weapon := "Stick"
var equipped_armor: String
var max_health := 10
var health := 10

var chest_contents = []

var guild_level := 1
var coins_deposited := 0
