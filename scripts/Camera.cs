using Godot;
using System;
using System.ComponentModel.DataAnnotations;

public struct ZoomAndPanOperation
{
	public float TargetZoom;
	public Vector2 TargetPosition;
	public float TimeRemaining;

    public ZoomAndPanOperation(float targetZoom, Vector2 targetPosition1, float timeRemaining) : this()
    {
        this.TargetZoom = targetZoom;
        this.TargetPosition = targetPosition1;
        this.TimeRemaining = timeRemaining;
    }
}

public partial class Camera : Camera2D
{
	public static Camera Instance { get; private set; }

	[Signal] public delegate void ZoomInOverLimitEventHandler();
	[Signal] public delegate void ZoomOutOverLimitEventHandler();
	[Signal] public delegate void ZoomAndPanOperationDoneEventHandler();

	[Export] public float ZoomSpeed { get; set; } = 1.05f;
	[Export] public float ZoomMin { get; set; } = 0.05f;
	[Export] public float ZoomMax { get; set; } = 0.25f;
	[Export] public float ZoomInitial { get; set; } = 0.07f;

	public bool Active { get; set; } = true;
	private bool _doZoomAndPanOperation = false;
	private ZoomAndPanOperation _zoomAndPanOperation;

	private bool _isPressed = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		Zoom = new Vector2(ZoomInitial, ZoomInitial);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_doZoomAndPanOperation)
		{
			float dZoom = _zoomAndPanOperation.TargetZoom - Zoom.X;
			Vector2 dPosition = _zoomAndPanOperation.TargetPosition - Position;

			float timeFraction = (float)(delta / _zoomAndPanOperation.TimeRemaining);

			float targetZoom = Zoom.X + (timeFraction * dZoom);
			Zoom = new Vector2(targetZoom, targetZoom);

			Position += timeFraction * dPosition;

			_zoomAndPanOperation.TimeRemaining -= (float)delta;

			if (_zoomAndPanOperation.TimeRemaining < 0.0f)
			{
				Zoom = new Vector2(_zoomAndPanOperation.TargetZoom, _zoomAndPanOperation.TargetZoom);
				Position = _zoomAndPanOperation.TargetPosition;
				_doZoomAndPanOperation = false;
				Active = true;
				EmitSignal(SignalName.ZoomAndPanOperationDone);
			}
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		if (!Active) return;

		// Pan
		if (inputEvent is InputEventMouseMotion motionEvent && _isPressed)
		{
			Position -= motionEvent.Relative / Zoom.X;
		}

		if (inputEvent is InputEventMouseButton buttonEvent)
		{
			if (buttonEvent.ButtonIndex == MouseButton.Left)
			{
				_isPressed = buttonEvent.IsPressed();
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
				Vector2 targetZoom = GetNewZoom(1 / ZoomSpeed);
				if (targetZoom.X > ZoomMin)
				{
					ZoomTo(targetZoom);
				}
				else
				{
					EmitSignal(SignalName.ZoomOutOverLimit);
				}
			}
		}
	}

	public Vector2 GetNewZoom(float zoomFactor)
	{
		return Zoom * zoomFactor;
	}

	public void ZoomTo(Vector2 targetZoom)
	{
		// Vector2 mousePosition = GetLocalMousePosition();
		Zoom = targetZoom;
		// Vector2 newMousePosition = GetLocalMousePosition();
		// Vector2 dPosition = mousePosition - newMousePosition;
		// Position += dPosition;
	}

	public void ZoomAndPanToOverTime(float targetZoom, Vector2 targetPosition, float timeSeconds)
	{
		_zoomAndPanOperation = new ZoomAndPanOperation(targetZoom, targetPosition, timeSeconds);
		Active = false;
		_doZoomAndPanOperation = true;
	}
}
