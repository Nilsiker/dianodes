class_name DialogueOptionButton
extends Button

var index = -1
signal option_pressed(index: int)

func _init(index, line):
	self.index = index
	text = line if line else "..."	# todo handle <next> and <end> dynamically through connection check? 
	add_theme_font_size_override("font_size", 36)

func _ready():
	pressed.connect(func(): option_pressed.emit(index))
