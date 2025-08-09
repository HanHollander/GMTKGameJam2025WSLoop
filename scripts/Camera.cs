using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public partial class Camera : Camera2D
{
	[Signal] public delegate void ZoomInOverLimitEventHandler();
	[Signal] public delegate void ZoomOutOverLimitEventHandler();

	[Export] public float ZoomSpeed { get; set; } = 1.2f;
	[Export] public float ZoomMin { get; set; } = 0.2f;
	[Export] public float ZoomMax { get; set; } = 2.0f;

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
				Vector2 newZoom = GetNewZoom(ZoomSpeed);
				if (newZoom.X < ZoomMax)
				{
					ZoomTo(newZoom);
				}
				else
				{
					EmitSignal(SignalName.ZoomInOverLimit);
				}
			}
			if (buttonEvent.ButtonIndex == MouseButton.WheelDown)
			{
				Vector2 newZoom = GetNewZoom(1 / ZoomSpeed);
				if (newZoom.X > ZoomMin)
				{
					ZoomTo(newZoom);
				}
				else
				{
					EmitSignal(SignalName.ZoomOutOverLimit);
				}
			}
		}
	}

	private Vector2 GetNewZoom(float zoomFactor) {
		return Zoom * zoomFactor;
	}

	private void ZoomTo(Vector2 newZoom)
	{
	
		Vector2 mousePosition = GetLocalMousePosition();
		Zoom = newZoom;
		Vector2 newMousePosition = GetLocalMousePosition();
		Offset += mousePosition - newMousePosition;
	}
}
