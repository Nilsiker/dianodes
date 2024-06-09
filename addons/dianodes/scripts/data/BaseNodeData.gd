@tool
class_name BaseNodeData
extends Resource

@export var name: String
@export var guid: String
@export var position: Vector2
@export var size: Vector2

func _init():
	guid = str(get_instance_id())