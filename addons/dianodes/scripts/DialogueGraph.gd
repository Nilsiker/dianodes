@tool
class_name DialogueGraph
extends GraphEdit

@export var data: Dialogue

@onready var popup: NodeCreatorPopup = $NodeCreatorPopup

var _node_scene: PackedScene = preload ("res://addons/dianodes/scenes/dialogue_node.tscn")
var _last_right_click_pos: Vector2

#region GODOT METHODS

func _ready():
	popup_request.connect(_on_popup_request)
	popup.node_data_created.connect(_on_popup_node_created)

	connection_request.connect(_on_connection_request)
	disconnection_request.connect(_on_disconnection_request)

#endregion

#region UI HANDLERS

func _on_popup_request(at_position: Vector2):
	var open_pos := get_screen_position() + at_position
	var create_pos = (at_position + scroll_offset) / zoom
	popup.open(open_pos, create_pos)

func _on_popup_node_created(node):
	data.add_node(node)

func _on_connection_request(from, to, from_port, to_port):
	data.add_connection(
		{
			"from_node"=from,
			"to_node"=to,
			"from_port"=from_port,
			"to_port"=to_port
		}
	)

func _on_disconnection_request(from, to, from_port, to_port):
	data.remove_connection(
		{
			"from_node"=from,
			"to_node"=to,
			"from_port"=from_port,
			"to_port"=to_port
		}
	)
#endregion

#region DATA HANDLERS

func register(data: Dialogue):
	unregister()

	data.node_added.connect(_on_data_node_added)
	data.node_removed.connect(_on_data_node_removed)
	data.connection_added.connect(_on_connection_added)
	data.connection_removed.connect(_on_disconnection_removed)
	
	self.data = data
	print("registered ", data)

	_render_view()

func unregister():
	if data:
		data.node_added.disconnect(_on_data_node_added)
		data.node_removed.disconnect(_on_data_node_removed)
		data.connection_added.disconnect(_on_connection_added)
		data.connection_removed.disconnect(_on_disconnection_removed)
		
		print("unregistered ", data)
		data = null
	
	_clear_view()

func _on_data_node_added(data: BaseNodeData):
	var node: DialogueNode = _node_scene.instantiate()
	node.data = data
	node.name = data.guid
	add_child(node)

func _on_data_node_removed(data: BaseNodeData):
	print(data)

func _on_connection_added(connection: Dictionary):
	connect_node(connection["from_node"], connection["to_node"], connection["from_port"], connection["to_port"])

func _on_disconnection_removed(connection: Dictionary):
	disconnect_node(connection["from_node"], connection["to_node"], connection["from_port"], connection["to_port"])
	
#endregion

#region UI HELPERS

func _render_view():
	for node_data in data.nodes:
		var created = _node_scene.instantiate()
		created.data = node_data
		created.name = str(node_data.guid)
		add_child(created)
	
	for conn in data.connections:
		connect_node(conn["from_node"], conn["to_node"], conn["from_port"], conn["to_port"])

func _clear_view():
	for child in self.get_children():
		if child is DialogueNode:
			child.free()

#endregion
