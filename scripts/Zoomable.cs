using System.Collections.Generic;
using Godot;
using Godot.Collections;

public struct FadeSpriteOperation
{
	public float TargetAlpha;
	public float TimeRemaining;
	public Sprite2D Sprite;

	public FadeSpriteOperation(float targetAlpha, float timeRemaining, Sprite2D sprite) : this()
	{
		TargetAlpha = targetAlpha;
		TimeRemaining = timeRemaining;
		Sprite = sprite;
	}
}

public partial class Zoomable : Node2D
{

	[Export] public Array<Thumbnail> Thumbnails;
	[Export] public bool Enabled { get; set; } = false;

	[Export] public float ZoomInTime { get; set; } = 1.0f;
	[Export] public float ZoomOutTime { get; set; } = 1.0f;
	[Export] public float ZoomInMult { get; set; } = 8.0f;
	[Export] public float ZoomOutMult { get; set; } = 2.0f;
	[Export] public float ParentBackgroundAlpha { get; set; } = 0.4f;
	private Thumbnail _nearestThumbnail;
	private bool _waitingForZoomAndPan = false;
	private bool _waitingForZoomIn = false;
	private bool _waitingForZoomOut = false;
	private List<FadeSpriteOperation> _fadeOperations = [];

	private Thumbnail selectedThumbnail = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!Enabled) Hide();

		Camera.Instance.ZoomInOverLimit += OnCameraZoomInOverLimit;
		Camera.Instance.ZoomOutOverLimit += OnCameraZoomOutOverLimit;
		Camera.Instance.ZoomAndPanOperationDone += OnCameraZoomAndPanOperationDone;
		foreach (Thumbnail thumbnail in Thumbnails)
		{
			thumbnail.ThumbnailSelectionChanged += OnThumbnailSelectionChanged;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_fadeOperations.Count == 0) return;

		List<FadeSpriteOperation> markedForErase = [];
		for (int i = 0; i < _fadeOperations.Count; i++)
		{
			FadeSpriteOperation fadeOperation = _fadeOperations[i];

			float dA = fadeOperation.TargetAlpha - fadeOperation.Sprite.SelfModulate.A;
			float timeFraction = (float)(delta / fadeOperation.TimeRemaining);

			Color modulate = fadeOperation.Sprite.SelfModulate;
			modulate.A += timeFraction * dA;

			fadeOperation.TimeRemaining -= (float)delta;

			if (fadeOperation.TimeRemaining < 0)
			{
				modulate.A = fadeOperation.TargetAlpha;
				markedForErase.Add(fadeOperation);
			}

			fadeOperation.Sprite.SelfModulate = modulate;
			_fadeOperations[i] = fadeOperation;
		}

		foreach (FadeSpriteOperation fadeOperation in markedForErase)
		{
			_fadeOperations.Remove(fadeOperation);
		}
	}

	public void OnCameraZoomInOverLimit()
	{
		if (!Enabled) return;

		// Fade parentZoomable to zero
		_fadeOperations.Add(new FadeSpriteOperation(0.0f, ZoomInTime,
		GetNode<Sprite2D>("Background").GetNode<Sprite2D>("ParentZoomable")));

		_waitingForZoomAndPan = true;
		_waitingForZoomIn = true;

		_nearestThumbnail = FindNearestThumbnail();
		Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomMax * ZoomInMult, _nearestThumbnail.GlobalPosition, ZoomInTime);
	}

	public void OnCameraZoomOutOverLimit()
	{
		if (!Enabled || ZoomStack.Instance.ZoomableStack.Count == 0) return;

		_waitingForZoomAndPan = true;
		_waitingForZoomOut = true;

		Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomMin / ZoomOutMult, new Vector2(0.0f, 0.0f), ZoomOutTime);
	}

	public void OnCameraZoomAndPanOperationDone()
	{
		if (!_waitingForZoomAndPan) return;

		// Zoomed in, now switch to child
		if (_waitingForZoomIn)
		{
			// Hide self
			Hide();
			Enabled = false;
			_waitingForZoomAndPan = false;
			_waitingForZoomIn = false;

			// Push self onto the ZoomStack
			ZoomStack.Instance.ZoomableStack.Push(this);

			// Show linked Zoomable (neares Thumbnail)
			_nearestThumbnail.LinkedZoomable.Show();
			_nearestThumbnail.LinkedZoomable.Enabled = true;

			// Reset Camera to correct position and zoom, and zoom in some more
			float targetZoom = Camera.Instance.Zoom.X * _nearestThumbnail.Scale.X;
			Camera.Instance.Position = new Vector2(0.0f, 0.0f);
			Camera.Instance.Zoom = new Vector2(targetZoom, targetZoom);
			Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomInitial, new Vector2(0.0f, 0.0f), ZoomInTime);

			// Update the ParentZoomableSprite of the linked Zoomable (nearest Thumbnail)
			Sprite2D parentZoomableSprite = _nearestThumbnail.LinkedZoomable.GetNode<Sprite2D>("Background").GetNode<Sprite2D>("ParentZoomable");
			parentZoomableSprite.Texture = (Texture2D)GetNode<Sprite2D>("Background").Texture;
			parentZoomableSprite.ZIndex = -1;

			float bgScale = 1 / _nearestThumbnail.Scale.X;
			parentZoomableSprite.Scale = new Vector2(bgScale, bgScale);
			parentZoomableSprite.Position = -1.0f * _nearestThumbnail.Position * bgScale;

			Color modulate = parentZoomableSprite.SelfModulate;
			modulate.A = 1.0f;
			parentZoomableSprite.SelfModulate = modulate;
		}

		if (_waitingForZoomOut && ZoomStack.Instance.ZoomableStack.Count > 0)
		{
			// Hide self
			Hide();
			Enabled = false;
			_waitingForZoomAndPan = false;
			_waitingForZoomOut = false;

			// Pop parent from zoom stack
			Zoomable parentZoomable = ZoomStack.Instance.ZoomableStack.Pop();

			// Show parent Zoomable
			parentZoomable.Show();
			parentZoomable.Enabled = true;

			// Reset Camera to correct position and zoom, and zoom in some more
			Thumbnail representative = new Thumbnail();
			foreach (Thumbnail thumbnail in parentZoomable.Thumbnails)
			{
				if (thumbnail.LinkedZoomable == this)
				{
					representative = thumbnail;
				}
			}
			float targetZoom = Camera.Instance.Zoom.X / representative.Scale.X;
			Camera.Instance.Position = representative.Position * parentZoomable.Scale.X;
			Camera.Instance.Zoom = new Vector2(targetZoom, targetZoom);
			Camera.Instance.ZoomAndPanToOverTime(Camera.Instance.ZoomInitial, new Vector2(0.0f, 0.0f), ZoomOutTime);

			// Update the parent Zoomable of our own parent Zoomable
			Sprite2D parentZoomableSprite = parentZoomable.GetNode<Sprite2D>("Background").GetNode<Sprite2D>("ParentZoomable");
			if (ZoomStack.Instance.ZoomableStack.Count == 0)
			{
				parentZoomableSprite.Texture = null;
			}
			else
			{
				parentZoomableSprite.Texture = (Texture2D)GetNode<Sprite2D>("Background").Texture;
				parentZoomableSprite.ZIndex = -1;

				float bgScale = 1 / _nearestThumbnail.Scale.X;
				parentZoomableSprite.Scale = new Vector2(bgScale, bgScale);
				parentZoomableSprite.Position = -1.0f * _nearestThumbnail.Position * bgScale;

				// Fade parentZoomable to full
				Color modulate = parentZoomableSprite.SelfModulate;
				modulate.A = 0.0f;
				parentZoomableSprite.SelfModulate = modulate;
				_fadeOperations.Add(new FadeSpriteOperation(1.0f, ZoomOutTime, parentZoomableSprite));

			}
		}
	}

	public void OnThumbnailSelectionChanged(Thumbnail thumbnail, bool selected)
	{
		if (selected)
		{
			if (selectedThumbnail == null)
			{
				selectedThumbnail = thumbnail;
			}
			else
			{
				ConnectThumbnails(selectedThumbnail, thumbnail);
				selectedThumbnail.UpdateSelectionState(false);
				thumbnail.UpdateSelectionState(false);
				selectedThumbnail = null;
			}
		}
		else
		{
			selectedThumbnail = null;
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

	private void ConnectThumbnails(Thumbnail first, Thumbnail second)
	{
		if (!first.HasTargetThumbnail(second))
		{
			Connection connection = AssetManager.Instance.GetConnectionScene().Instantiate<Connection>();
			connection.Init(first, second);
			connection.ZIndex = 10;
			AddChild(connection);
			first.AddOutgoingConnection(connection);
		}
	}
}
