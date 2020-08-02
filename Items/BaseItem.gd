extends Node

var item_name: String
var buyable: bool = false
var sellable: bool = false
var value: int

func _init():
	Global.items[item_name] = self
