extends MarginContainer


func _on_Item_pressed(item_name):
	print("ItemInfoMenu: %s" % item_name)
	visible = true


func _on_Back_pressed():
	visible = false


func _on_Equip_pressed():
	pass


func _on_Use_pressed():
	pass
