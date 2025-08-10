using Godot;
using System;

public partial class Connection : Line2D
{

	private Thumbnail source;
	private Thumbnail target;

	public void Init(Thumbnail source, Thumbnail target)
	{
		this.source = source;
		this.target = target;
		AddPoint(source.Position);
		AddPoint(target.Position);
	}

	public override void _Input(InputEvent inEvent)
	{
		if (inEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right
		&& mouseEvent.Pressed
		&& GetNode<Zoomable>("..").Enabled // Is on screen
		&& MouseIsOnLine()
		)// && first.GetOccupier() == Thumbnail.Occupier.PLAYER)
		{

			RemoveConnection();
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void RemoveConnection()
	{
		source.RemoveOutgoingConnection(this);
		QueueFree();
	}

	public Thumbnail GetTargetThumbnail()
	{
		return target;
	}

	private bool MouseIsOnLine()
	{
		Vector2 mouseLocation = GetGlobalMousePosition();
		Vector2 line1 = source.GlobalPosition;
		Vector2 line2 = target.GlobalPosition;
		Vector2 pointOnLine = Geometry2D.GetClosestPointToSegment(mouseLocation, line1, line2);
		return pointOnLine.DistanceTo(mouseLocation) < 100;
	}
}
