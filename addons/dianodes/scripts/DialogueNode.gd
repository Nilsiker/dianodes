@tool
class_name DialogueNode
extends GraphNode

var _data: BaseNodeData

#region GODOT METHODS	
func _ready():
	if _data == null: push_error("no data on node")
	
	position_offset = _data.position
	size = _data.size

	position_offset_changed.connect(_on_position_offset_changed)
	resized.connect(_on_resized)
#endregion

#region UI HANDLERS
func _on_resized():
	_data.size = size

func _on_position_offset_changed():
	_data.position = position_offset
#endregion
