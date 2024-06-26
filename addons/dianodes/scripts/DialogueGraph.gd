@tool
class_name DialogueGraph
extends GraphEdit

@export var data: Dialogue = null

@onready var popup: NodeCreatorPopup = $NodeCreatorPopup

var _line_scene: PackedScene = preload ("res://addons/dianodes/scenes/line_node.tscn")
var _condition_scene: PackedScene = preload ("res://addons/dianodes/scenes/condition_node.tscn")
var _event_scene: PackedScene = preload ("res://addons/dianodes/scenes/event_node.tscn")

var _last_right_click_pos: Vector2

#region GODOT METHODS

func _ready():
	popup_request.connect(_on_popup_request)
	popup.node_data_created.connect(_on_popup_node_created)

	connection_request.connect(_on_connection_request)
	disconnection_request.connect(_on_disconnection_request)
	delete_nodes_request.connect(_on_delete_nodes_request)
	
	scroll_offset_changed.connect(_on_scroll_offset_changed)

func _gui_input(event):
	if not data: return
	if not event is InputEventMouseButton: return
	if event.button_index == MOUSE_BUTTON_WHEEL_DOWN or event.button_index == MOUSE_BUTTON_WHEEL_UP:
		data.zoom = zoom

#endregion

#region UI HANDLERS

func _on_popup_request(at_position: Vector2):
	var open_pos := get_screen_position() + at_position
	var create_pos = (at_position + scroll_offset) / zoom
	popup.open(open_pos, create_pos)

func _on_popup_node_created(node):
	if not data: return
	data.add_node(node)

func _on_connection_request(from_node, from_port, to_node, to_port):
	if not data: return
	data.add_connection(
		{
			"from_node"=from_node,
			"from_port"=from_port,
			"to_node"=to_node,
			"to_port"=to_port
		}
	)

func _on_disconnection_request(from_node, from_port, to_node, to_port):
	if not data: return
	data.remove_connection(
		{
			"from_node"=from_node,
			"from_port"=from_port,
			"to_node"=to_node,
			"to_port"=to_port
		}
	)

func _on_delete_nodes_request(nodes):
	if not data: return
	for node in nodes:
		data.remove_node_by_guid(node)

func _on_scroll_offset_changed(offset):
	if not data: return
	data.scroll_offset = offset
	
func _on_node_option_removed(name, slot):
	data.remove_connection_by_name_and_slot(name, slot)

#endregion

#region UI HELPERS

func _render_view():
	var created: Node
	for node_data in data.nodes:
		if node_data is LineNodeData:
			created = _line_scene.instantiate()
			created.option_removed.connect(_on_node_option_removed)
		elif node_data is ConditionNodeData:
			created = _condition_scene.instantiate()
		elif node_data is EventNodeData:
			created = _event_scene.instantiate()
		created.data = node_data
		add_child(created)
		created.name = node_data.guid
	
	for conn in data.connections:
		connect_node(conn["from_node"], conn["from_port"], conn["to_node"], conn["to_port"])
	
	zoom = data.zoom
	scroll_offset = data.scroll_offset

func _clear_view():
	for conn in get_connection_list():
		disconnect_node(conn["from_node"], conn["from_port"], conn["to_node"], conn["to_port"])
	for child in self.get_children():
		if child is DialogueNode:
			child.free()

#endregion

#region DATA HANDLERS

func register(data: Dialogue):
	# todo unregister this might be unnecessary once i don't reload the editor. 
	# now it throws annoying but harmless errors on graph load
	unregister()
	if not data: return
	
	data.node_added.connect(_on_data_node_added)
	data.node_removed.connect(_on_data_node_removed)
	data.connection_added.connect(_on_connection_added)
	data.connection_removed.connect(_on_disconnection_removed)
	
	self.data = data
	print("registered ", data)
	
	_render_view()

func unregister():
	if not self.data: return
	print("unregistering ", data)
	data.node_added.disconnect(_on_data_node_added)
	data.node_removed.disconnect(_on_data_node_removed)
	data.connection_added.disconnect(_on_connection_added)
	data.connection_removed.disconnect(_on_disconnection_removed)
	data = null
	
	_clear_view()

func _on_data_node_added(data: BaseNodeData):
	var node: Node
	if data is LineNodeData:
		node = _line_scene.instantiate()
		node.option_removed.connect(_on_node_option_removed)
	elif data is ConditionNodeData:
		node = _condition_scene.instantiate()
	elif data is EventNodeData:
		node = _event_scene.instantiate()

	node.data = data
	node.name = data.guid
	add_child(node)

func _on_data_node_removed(data: BaseNodeData):
	for node in get_children():
		if node is DialogueNode and node.data == data:
			node.free()
			break

func _on_connection_added(conn: Dictionary):
	connect_node(
		conn["from_node"], conn["from_port"], conn["to_node"], conn["to_port"]
	)

func _on_disconnection_removed(conn: Dictionary):
	disconnect_node(
		conn["from_node"], conn["from_port"], conn["to_node"], conn["to_port"]
	)
	
#endregion
