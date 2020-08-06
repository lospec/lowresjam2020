extends StaticBody2D


func _on_DoorDetection_body_entered(body):
	print("%s at guild hall door!" % body.name)
