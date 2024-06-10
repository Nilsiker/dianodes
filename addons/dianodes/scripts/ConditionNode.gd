@tool
class_name ConditionNode
extends DialogueNode

@export var data: ConditionNodeData:
	get: return _data
	set(value): _data = value

#region GODOT METHODS	
func _ready():
	super._ready()
	$NameEdit.text_changed.connect(_on_name_edit_text_changed)
#endregion

#region UI HANDLERS
func _on_name_edit_text_changed(text):
	data._condition_name = text
#endregion
