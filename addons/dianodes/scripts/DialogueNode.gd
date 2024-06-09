@tool
class_name DialogueNode
extends GraphNode

signal option_removed(slot: int)

@export var data: BaseNodeData

@export var name_edit: LineEdit

@export var portrait: TextureRect
@export var line_edit: TextEdit

var option_scene = preload("res://addons/dianodes/scenes/dialogue_option.tscn")

#region GODOT METHODS

func _ready():
	if data == null: push_error("no data on node")
	
	name_edit.text = data.name
	position_offset = data.position
	size = data.size

	position_offset_changed.connect(_on_position_offset_changed)
	resized.connect(_on_resized)

	name_edit.text_changed.connect(_on_name_edit_changed)

	if data is LineNodeData:
		title = "Line"
		portrait.visible = true
		line_edit.visible = true
		$Option.visible = true
		
		line_edit.text = data.line
		portrait.texture = data.portrait
		#portrait.texture.changed.connect(_on_portrait_texture_changed)
		line_edit.text_changed.connect(_on_line_edit_changed)	
	elif data is EventNodeData:
		title = "Event"
	elif data is ConditionNodeData:
		title = "Condition"

#endregion

#region UI METHODS

func add_option():
	var num_children = get_children().size()
	var num_options = get_children().filter(func(c): return c is TextEdit).size()
	
	var option = option_scene.instantiate()
	add_child(option)
	
	
	

func remove_option(slot):
	var options = get_children().filter(func(c): return c is TextEdit)
	options[slot].queue_free()
	

#endregion

#region UI HANDLERS

func _on_resized():
	data.size = size

func _on_position_offset_changed():
	data.position = position_offset

func _on_name_edit_changed(text):
	data.name = text
	
func _on_line_edit_changed():
	data.line = line_edit.text

func _on_portrait_texture_changed():
	data.portrait = portrait.texture

func _on_add_option_button_pressed():
	add_option()

#endregion
