@tool
class_name EventNode
extends DialogueNode

@export var data: EventNodeData:
	get: return _data
	set(value): _data = value

#region GODOT METHODS	
func _ready():
	super._ready()
	$NameEdit.text_changed.connect(_on_name_edit_text_changed)
	$NameEdit.text = data._event_name
#endregion

#region UI HANDLERS
func _on_name_edit_text_changed(text):
	data._event_name = text
#endregion
