@tool
class_name NodeCreatorPopup
extends PopupMenu

signal node_data_created(node: BaseNodeData)

var _last_create_pos: Vector2

# Called when the node enters the scene tree for the first time.
func _ready():
	index_pressed.connect(_on_index_pressed)

func open(open_pos: Vector2, create_pos: Vector2):
	position = open_pos
	_last_create_pos = create_pos
	popup()

func _on_index_pressed(index):
	var data: BaseNodeData
	match index:
		0: data = LineNodeData.new()
		1: data = ConditionNodeData.new()
		2: data = EventNodeData.new()

	data.position = _last_create_pos
	node_data_created.emit(data)
	
