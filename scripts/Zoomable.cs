using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zoomable : Node2D
{

	[Export] public Array<Thumbnail> Thumbnails;
	[Export] public bool Enabled { get; set; } = false;

	[Export] public float ZoomInTimeSeconds { get; set; } = 2.0f;
	private Thumbnail _nearestThumbnail;
	private bool _waitingForZoomAndPan;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!Enabled) Hide();

		Camera.Instance.ZoomInOverLimit += OnCameraZoomInOverLimit;
		Camera.Instance.ZoomOutOverLimit += OnCameraZoomOutOverLimit;
		Camera.Instance.ZoomAndPanOperationDone += OnCameraZoomAndPanOperationDone;
		foreach (Thumbnail thumbnail in Thumbnails)
		{
			thumbnail.ThumbnailSelected += OnThumbnailSelect;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnCameraZoomInOverLimit()
	{
		if (!Enabled) return;

		_nearestThumbnail = FindNearestThumbnail();
		_waitingForZoomAndPan = true;

		Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomMax * 24.0f, _nearestThumbnail.GlobalPosition, 1.0f);
	}

	public void OnCameraZoomOutOverLimit()
	{
		if (!Enabled) return;

		GD.Print("zool");
	}

	public void OnCameraZoomAndPanOperationDone()
	{
		if (!_waitingForZoomAndPan) return;

		GD.Print("Hide ", ", ", Camera.Instance.Zoom, ", ", Scale, ", ", _nearestThumbnail.Scale);
		Hide();
		Enabled = false;

		float targetZoom = Camera.Instance.Zoom.X * _nearestThumbnail.Scale.X;

		GD.Print(targetZoom);

		_nearestThumbnail.LinkedZoomable.Show();
		_nearestThumbnail.LinkedZoomable.Enabled = true;

		_waitingForZoomAndPan = false;

		Camera.Instance.Position = new Vector2(0.0f, 0.0f);
		Camera.Instance.Zoom = new Vector2(targetZoom, targetZoom);
		Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomInitial, new Vector2(0.0f, 0.0f), 1.0f);
	}

	public void OnThumbnailSelect()
	{
		Thumbnail first = null;
		Thumbnail second = null;

		foreach (Thumbnail thumbnail in Thumbnails)
		{
			if (thumbnail.IsSelected())
			{
				if (first == null)
				{
					first = thumbnail;
				}
				else
				{
					second = thumbnail;
					break;
				}
			}
		}

		if (second != null)
		{
			
			first.UpdateSelectionState(false);
			second.UpdateSelectionState(false);
			GD.Print("CONNECT");
		}
	}

	private Thumbnail FindNearestThumbnail()
	{
		Thumbnail result = new Thumbnail();
		float minDistance = float.MaxValue;
		foreach (Thumbnail thumbnail in Thumbnails)
		{
			Vector2 distanceVector = Camera.Instance.Position - thumbnail.GlobalPosition;
			GD.Print(Camera.Instance.Position, ", ", thumbnail.GlobalPosition);
			GD.Print(distanceVector.Length(), ", ", minDistance);
			if (distanceVector.Length() < minDistance)
			{
				result = thumbnail;
				minDistance = distanceVector.Length();
			}
		}
		return result;
	}

}
