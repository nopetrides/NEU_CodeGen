These Dialogue System speech bubble prefabs use TextMesh Pro instead of UI Text.

You must enable the Dialogue System's TextMesh Pro support by ticking the
TMP_PRESENT checkbox in the Dialogue System's Welcome Window.

To use one of these prefabs, add it to your character. The bubble is anchored to the lower 
right corner of the prefab's Canvas, so you'll want to position the prefab instance's
lower right corner above your character (e.g., set Pos X to about -2.4, Pos Y to 2).

The subtitle panel can show the bubble's tail at the left, middle, or center of the bubble.
By default, the tail starts at the right position. To specify a different position, add a
custom Number field named "Position" the dialogue entry and set it to 0 (left), 1 (middle),
or 2 (right).

Optional: If you want to use these with Text Animator for Unity, add Text Animator and 
Text Animator Player components to the subtitle panel's Subtitle Text GameObject.
