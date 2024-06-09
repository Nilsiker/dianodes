@tool
class_name DialogueNode
extends GraphNode

@export var data: BaseNodeData

@export var name_edit: LineEdit

@export var portrait: TextureRect
@export var line_edit: TextEdit

#region GODOT METHODS

func _ready():
	if data == null: push_error("no data on node")
	
	position_offset = data.position
	size = data.size


	position_offset_changed.connect(_on_position_offset_changed)
	resized.connect(_on_resized)

	name_edit.text_changed.connect(_on_name_edit_changed)

	if data is LineNodeData:
		title = "Line"
		portrait.visible = true
		portrait.texture.changed.connect(_on_portrait_texture_changed)
	elif data is EventNodeData:
		title = "Event"
	elif data is ConditionNodeData:
		title = "Condition"

#endregion

#region UI HANDLERS

func _on_resized():
	data.size = size

func _on_position_offset_changed():
	data.position = position_offset

func _on_name_edit_changed(text):
	data.name = text

func _on_portrait_texture_changed():
	data.portrait = portrait.texture

#endregion
