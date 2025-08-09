using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zoomable : Node2D
{

	[Export] public Node2D ZoomableInstance;
	[Export] public Array<Thumbnail> Thumbnails;
	[Export] public bool Enabled { get; set; } = false;

	[Export] public float ZoomInTimeSeconds { get; set; } = 2.0f;
	private Thumbnail _nearestThumbnail;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Camera.Instance.ZoomInOverLimit += OnCameraZoomInOverLimit;
		Camera.Instance.ZoomOutOverLimit += OnCameraZoomOutOverLimit;
		Camera.Instance.ZoomAndPanOperationDone += OnCameraZoomAndPanOperationDone;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnCameraZoomInOverLimit()
	{
		if (!Enabled) return;

		_nearestThumbnail = FindNearestThumbnail();

		Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomMax * 2.0f, _nearestThumbnail.Position, 2.0f);
	}

	public void OnCameraZoomOutOverLimit()
	{
		if (!Enabled) return;

		GD.Print("zool");
	}

	public void OnCameraZoomAndPanOperationDone()
	{
		if (!Enabled) return;

		GD.Print("zapod");
	}

	private Thumbnail FindNearestThumbnail()
	{
		Thumbnail result = new Thumbnail();
		float minDistance = float.MaxValue;
		foreach (Thumbnail thumbnail in Thumbnails)
		{
			Vector2 distanceVector = Camera.Instance.Position - thumbnail.Position;
			if (distanceVector.Length() < minDistance)
			{
				result = thumbnail;
				minDistance = distanceVector.Length();
			}
		}
		return result;
	}

}
