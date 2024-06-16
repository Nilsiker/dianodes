@tool
class_name LineNodeData
extends BaseNodeData

signal option_added
signal option_removed

@export var portrait: Texture2D
@export var line: String
@export var options: Array[String] = [""]

func add_option():
	options.append("")
	option_added.emit()

func remove_option():
	options.pop_back()
	option_removed.emit()
