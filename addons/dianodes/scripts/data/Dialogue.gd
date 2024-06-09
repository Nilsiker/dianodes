@tool
class_name Dialogue
extends Resource

signal node_added(node: BaseNodeData)
signal connection_added(conn: Dictionary)
signal node_removed(node: BaseNodeData)
signal connection_removed(conn: Dictionary)

@export var zoom: float
@export var scroll_offset: Vector2
@export var nodes: Array[BaseNodeData] = []
@export var connections: Array[Dictionary] = []

func add_node(node: BaseNodeData):
	nodes.append(node)
	node_added.emit(node)

	print("data: added node ", node, " to ", resource_path)

func remove_node_by_guid(guid: String):
	var found = nodes.filter(func(n): return n.guid == guid)
	remove_node(found.pop_back())

func remove_node(node: BaseNodeData):
	nodes.remove_at(nodes.find(node))
	_remove_connections_to(node.guid)
	node_removed.emit(node)

func add_connection(conn: Dictionary):
	connections.append(conn)
	connection_added.emit(conn)

func remove_connection(conn: Dictionary):
	var found = connections.filter(func(c):
		return c["from_node"] == conn["from_node"] and c["to_node"] == conn["to_node"] and c["from_port"] == conn["from_port"] and c["to_port"] == conn["to_port"]
	)
	if found[0]:
		connections.erase(found[0])
		connection_removed.emit(conn)

func _remove_connections_to(node: StringName):
	print("removing connections for ", node)
	for conn in connections.filter(func(c): return c["to_node"] == node or c["from_node"] == node):
		remove_connection(conn)
