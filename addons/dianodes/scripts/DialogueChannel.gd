@tool
extends Node

signal updated(data: LineNodeData, variables: Dictionary)
signal ended

var dialogue: Dialogue = null
var callables: Dictionary = {}
var variables: Dictionary = {}
var current_node: BaseNodeData = null

func start_dialogue(dialogue: Dialogue, callbacks={}, variables={}):
	self.dialogue = dialogue
	self.callables = callbacks
	self.variables = variables
	if dialogue:
		current_node = _find_start_node()
		print("current_node ", current_node)
		updated.emit(current_node, variables)

func progress(slot: int):
	var conn = dialogue.connections.filter(func(conn):
		return conn.from_node == current_node.guid and conn.from_port == slot
	)

	if conn.size() != 1:
		print("no singular connection found for ", current_node.guid, " and slot ", slot, ", ending dialogue")
		ended.emit()
		return
	
	var to = dialogue.nodes.filter(func(n):
		return n.guid == conn[0].to_node
	)

	if to.size() != 1:
		print("no singular node found with name ", conn[0].to_node, ", ending dialogue")
		ended.emit()
		return

	current_node = to[0]

	if current_node is LineNodeData:
		updated.emit(current_node, variables)
	elif current_node is ConditionNodeData:
		var try_condition: Callable = callables[current_node._condition_name]
		progress(try_condition.call())
	elif current_node is EventNodeData:
		var event_callback: Callable = callables[current_node._event_name]
		event_callback.call()
		progress(0)
	# todo add Label and Goto nodes for less messy flows :)

func _find_start_node():
	var start_conn = dialogue.connections.filter(func(conn): return conn.from_node == "Start")
	var start_node = dialogue.nodes.filter(func(n): return n.guid == start_conn[0].to_node)
	return start_node[0]
