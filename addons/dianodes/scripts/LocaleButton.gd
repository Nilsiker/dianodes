@tool
class_name LocaleButton
extends OptionButton

# Called when the node enters the scene tree for the first time.
func _ready():
	for locale in TranslationServer.get_all_languages():
		add_item(locale)