@tool
extends EditorPlugin

var _editor_scene: PackedScene = preload ("res://addons/dianodes/scenes/dialogue_editor.tscn")
var _icon = preload ("res://addons/dianodes/icons/dialogue.svg")

var _editor: DialogueEditor

func _enter_tree():
	_reload()

func _exit_tree():
	pass

func _make_visible(visible):
	if visible:
		_reload()
	
	_editor.visible = visible;

func _handles(object):
	return object is Dialogue

func _edit(object):
	_editor.edit(object)

func _has_main_screen(): return true
func _get_plugin_icon(): return _icon
func _get_plugin_name(): return "Dianodes"

func _reload():
	if _editor:
		_editor.queue_free()
	_editor = _editor_scene.instantiate()
	_editor.visible = false
	get_editor_interface().get_editor_main_screen().add_child(_editor)
