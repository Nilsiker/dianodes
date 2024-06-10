@tool
class_name LineNode
extends DialogueNode

signal option_removed(slot: int)

@export var data: LineNodeData:
	get: return _data
	set(value): _data = value

@export var name_edit: LineEdit
@export var portrait: TextureRect
@export var line_edit: TextEdit

var option_scene = preload("res://addons/dianodes/scenes/dialogue_option.tscn")

#region GODOT METHODS	

func _ready():
	super._ready()
	name_edit.text_changed.connect(_on_name_edit_changed)

	title = "Line"
	
	line_edit.text = data.line
	portrait.texture = data.portrait
	line_edit.text_changed.connect(_on_line_edit_changed)
	
	data.option_added.connect(_on_data_option_added)
	data.option_removed.connect(_on_data_option_removed)
	
	
func _exit_tree():
	data.option_added.disconnect(_on_data_option_added)
	data.option_removed.disconnect(_on_data_option_removed)

#endregion

#region DATA HANDLERS

func _on_data_option_added():
	var num_children = get_children().size()
	var options = get_children().filter(func(c): return c is DialogueOption)
	var num_options = get_children().filter(func(c): return c is DialogueOption).size()
	
	options.back().set_show_close_button(false)
	
	var option = option_scene.instantiate()
	add_child(option)
	move_child(option, num_children-1)
	
	option.removed.connect(_on_option_removed)
	

func _on_data_option_removed():
	var options = get_children().filter(func(c): return c is DialogueOption)
	options.pop_back().queue_free()
	if options.size() > 1:
		options.back().set_show_close_button(true)
		

#endregion


#region UI METHODS

func remove_option(slot):
	var options = get_children().filter(func(c): return c is TextEdit)
	options[slot].queue_free()
	
#endregion

#region UI HANDLERS

func _on_name_edit_changed(text):
	data.name = text
	
func _on_line_edit_changed():
	data.line = line_edit.text

func _on_add_option_button_pressed():
	data.add_option()

func _on_option_removed():
	data.remove_option()

#endregion
