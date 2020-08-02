extends Node

# Public Variables
var chunk_scene = preload("res://World/Chunks/Plain Grass Chunk.tscn")
var current_chunk = null

# Onready Variables
onready var chunks_collection = $Chunks
onready var player = $Player

func _ready():
	var chunk = chunk_scene.instance()
	chunks_collection.add_child(chunk)
	current_chunk = chunk