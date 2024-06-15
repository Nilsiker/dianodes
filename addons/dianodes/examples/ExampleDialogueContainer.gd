@tool
class_name ExampleDialogueContainer
extends PanelContainer

@export var _name_label: Label
@export var _line_label: Label
@export var options: Control
@export var call: Callable

@export var _line_speed: float = 20

# Called when the node enters the scene tree for the first time.
func _ready():
	DialogueChannel.updated.connect(_on_dialogue_updated)
	DialogueChannel.ended.connect(func(): queue_free())

func _on_dialogue_ended():
	get_parent().visible = false
	queue_free()

func _on_dialogue_updated(node: LineNodeData, variables: Dictionary):
	print("progressed to node ", node.guid)
	var line = node.line
	for variable in variables:
		line = line.format(variables)
	_line_label.text = line
	_name_label.text = node.name
	
	start_animated_line_tween()
	
	options.visible = false
	for child in options.get_children():
		child.queue_free()
	for i in node.options.size():
		var option_button = DialogueOptionButton.new(i, node.options[i])
		options.add_child(option_button)
		option_button.option_pressed.connect(_on_option_pressed)

func _gui_input(event):
	if event is InputEventMouseButton and event.is_pressed() and event.button_index == MOUSE_BUTTON_LEFT:
		if tween: tween.kill()
		_line_label.visible_characters = _line_label.text.length()
		_show_options()

func _is_line_scrolling_finished(): return _line_label.visible_characters >= _line_label.text.length()

var tween: Tween
func start_animated_line_tween():
	_line_label.visible_characters = 0
	if tween: tween.kill()
	tween = create_tween()
	var duration = _line_label.text.length() / _line_speed
	print(duration)
	tween.tween_property(_line_label, "visible_characters", _line_label.text.length(), duration)
	tween.tween_callback(_show_options)

func _show_options():
	options.visible = true

func _on_option_pressed(index):
	DialogueChannel.progress(index)