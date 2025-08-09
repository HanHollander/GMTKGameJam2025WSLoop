using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Camera : Camera2D
{

	[Export] public float ZoomSpeed { get; set; } = 1.2f;
	[Export] public float ZoomMin { get; set; } = 0.1f;
	[Export] public float ZoomMax { get; set; } = 10.0f;

	public bool IsPressed = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent inputEvent)
	{
		// Pan
		if (inputEvent is InputEventMouseMotion motionEvent && IsPressed)
		{
			Position -= motionEvent.Relative / Zoom.X;
		}

		if (inputEvent is InputEventMouseButton buttonEvent)
		{
			if (buttonEvent.ButtonIndex == MouseButton.Left)
			{
				IsPressed = buttonEvent.IsPressed();
			}
			if (buttonEvent.ButtonIndex == MouseButton.WheelUp)
			{
				Vector2 mousePosition = GetLocalMousePosition();
				Zoom = Vector2.One * Math.Min(ZoomMax, Zoom.X * ZoomSpeed);
				Vector2 newMousePosition = GetLocalMousePosition();
				Offset += mousePosition - newMousePosition;
			}
			if (buttonEvent.ButtonIndex == MouseButton.WheelDown)
			{
				Vector2 mousePosition = GetLocalMousePosition();
				Zoom = Vector2.One * Math.Max(ZoomMin, Zoom.X * (1 / ZoomSpeed));
				Vector2 newMousePosition = GetLocalMousePosition();
				Offset += mousePosition - newMousePosition;
			}
		}
    }
}
