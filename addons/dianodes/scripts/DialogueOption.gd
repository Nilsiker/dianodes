@tool
class_name DialogueOption
extends HBoxContainer

signal removed
signal updated(index, text)

var index = 0

var text: String = "":
	get: return text
	set(value): $OptionEdit.text = value

func _ready():
	$OptionEdit.text_changed.connect(func(): updated.emit(index, $OptionEdit.text))
	$Button.pressed.connect(_on_close_button_pressed)

func set_show_close_button(show):
	$Button.visible = show

func _on_close_button_pressed(): 
	removed.emit()
