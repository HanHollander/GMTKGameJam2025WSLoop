using Godot;
using System;
using System.Collections.Generic;

public partial class Connection : Line2D
{

	private static double ANIMATION_DURATION = 2.0;

	private Thumbnail source;
	private Thumbnail target;
	private List<Connection> infoConnections = [];
	private bool animate = false;
	private double animationTime = 0.0;

	public void Init(Thumbnail source, Thumbnail target, bool animate = false)
	{
		this.source = source;
		this.target = target;
		this.animate = animate;
		AddPoint(source.Position);
		if (animate)
		{
			AddPoint(source.Position);
		}
		else
		{
			AddPoint(target.Position);
		}
	}

	public override void _Input(InputEvent inEvent)
	{
		if (inEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right
		&& mouseEvent.Pressed
		&& GetNode<Zoomable>("..").Enabled // Is on screen
		&& MouseIsOnLine()
		&& this.source.GetThumbnailType() != Thumbnail.ThumbnailType.NORMAL
		&& this.source.GetOccupier() == Thumbnail.Occupier.PLAYER)
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
		if (animate)
		{
			animationTime += delta;
			if (animationTime >= ANIMATION_DURATION)
			{
				SetPointPosition(1, target.Position);
				animate = false;
			}
			else
			{
				double progress = animationTime / ANIMATION_DURATION;
				float dx = target.Position.X - source.Position.X;
				float dy = target.Position.Y - source.Position.Y;
				SetPointPosition(1, new Vector2((float)(dx * progress), (float)(dy * progress)));
			}
		}
	}

	public void RemoveConnection()
	{
		source.RemoveOutgoingConnection(this);
		foreach (Connection connection in infoConnections)
		{
			connection.QueueFree();
		}
		QueueFree();
	}

	public Thumbnail GetTargetThumbnail()
	{
		return target;
	}

	public void AddInfoConnection(Connection connection)
	{
		infoConnections.Add(connection);
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
