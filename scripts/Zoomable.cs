using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zoomable : Node2D
{

	[Export] public Node2D ZoomableInstance;
	[Export] public Array<Thumbnail> Thumbnails;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Connect(Camera.SignalName.ZoomInOverLimit, Callable.From(OnCameraZoomInOverLimit));
		// Connect(Camera.SignalName.ZoomOutOverLimit, Callable.From(OnCameraZoomOutOverLimit));
		Camera.Instance.ZoomInOverLimit += OnCameraZoomInOverLimit;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnCameraZoomInOverLimit()
	{
		GD.Print("ziol");
		FindNearestThumbnail();
	}

	public void OnCameraZoomOutOverLimit()
	{
		GD.Print("zool");
	}

	private Thumbnail FindNearestThumbnail()
	{
		Thumbnail result = new Thumbnail();
		float minDistance = float.MaxValue;
		foreach (Thumbnail thumbnail in Thumbnails)
		{
			Vector2 distanceVector = Camera.Instance.Position - thumbnail.Position;
			GD.Print("found: ", distanceVector.Length(), minDistance);
			if (distanceVector.Length() < minDistance)
			{
				result = thumbnail;
			}
		}
		GD.Print("closest ", result.Position);
		return result;
	}

}
