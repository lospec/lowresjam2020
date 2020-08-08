extends Reference
class_name PriorityQueue

var _list = []

var minimum = 9223372036854775807
var count = 0


func enqueue(value, priority):
	count += 1

	if priority < minimum:
		minimum = priority

	while priority >= len(_list):
		_list.append(null)

	value.next_with_same_priority = _list[priority]
	_list[priority] = value


func dequeue():
	count -= 1
	for i in range(minimum, len(_list)):
		minimum = i
		var value = _list[i]
		if value:
			_list[i] = value.next_with_same_priority
			return value
	return null


func clear():
	_list.clear()
	count = 0
	pass
