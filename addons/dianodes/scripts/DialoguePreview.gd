@tool
class_name DialoguePreview
extends MarginContainer

var dialogue_scene = preload ("res://addons/dianodes/examples/example_dialogue_container.tscn")

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func start(dialogue: Dialogue):
	var scene = dialogue_scene.instantiate()
	add_child(scene)
	scene.tree_exited.connect(func(): visible=false)
	visible = true

	print("Running ", dialogue.resource_path, " in editor.")
	DialogueChannel.start_dialogue(dialogue, {
		"persuaded": func(): return randf() > 0.5,
		"print_hello": func(): print("hello! :) :)")
	}, {
		"zone_name": ["Bad Boy Burrows", "[rainbow]Good Guy Gardens[/rainbow]", "The Pretty Poolhouse"].pick_random(),
		"weather_descriptor": ["cloudy", "rainy", "windy", "foggy", "sunny"].pick_random()
	})

func _gui_input(event):
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_LEFT:
		visible = false
		for c in get_children():
			c.queue_free()
