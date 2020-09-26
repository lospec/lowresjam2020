extends Node

const DEFAULT_WORLD_POSITION := Vector2(2578, 1517)
const DEFAULT_COINS := 100
const DEFAULT_INVENTORY := [
	"Stick", "Hotdog", "Leather", "Chainmail"
]
const DEFAULT_WEAPON := "Stick"
const DEFAULT_ARMOR := ""
const DEFAULT_HEALTH := 20

var world_position := DEFAULT_WORLD_POSITION
var character_name := "Jason"
var coins := DEFAULT_COINS
var inventory := DEFAULT_INVENTORY
var equipped_weapon := DEFAULT_WEAPON
var equipped_armor := DEFAULT_ARMOR
var max_health := DEFAULT_HEALTH
var health := DEFAULT_HEALTH

var chest_contents = []

var guild_level := 1
var coins_deposited := 0
