@tool
class_name DialogueEditor
extends Control

@export var _graph: DialogueGraph
@export var _files: DialogueFiles
@export var _preview: DialoguePreview

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func edit(dialogue: Dialogue):
	_graph.register(dialogue)
	$NoFileOverlay.visible = dialogue == null

func _on_run_pressed():
	print("start preview")
	_preview.start(_graph.data)