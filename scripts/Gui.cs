using Godot;
using System;

public partial class Gui : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Label mousePosition = GetNode<CanvasLayer>("Debug").GetNode<Label>("MousePosition");
		String mousePositionString = GetGlobalMousePosition().ToString() + " / " + GetLocalMousePosition().ToString();
		mousePosition.Text = mousePositionString;

		Label zoomLevel = GetNode<CanvasLayer>("Debug").GetNode<Label>("ZoomLevel");
		String zoomLevelString = Camera.Instance.Zoom.ToString();
		zoomLevel.Text = zoomLevelString;
	}
}
