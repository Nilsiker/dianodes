@tool
class_name DialogueOption
extends HBoxContainer

signal removed
signal option_updated(text)

func _ready():
	$OptionEdit.text_changed.connect(func(): option_updated.emit($OptionEdit.text))
	$Button.pressed.connect(_on_close_button_pressed)

func set_show_close_button(show):
	$Button.visible = show

func _on_close_button_pressed(): 
	removed.emit()
