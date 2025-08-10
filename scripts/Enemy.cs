using Godot;
using System;

public partial class Enemy : Node
{
	private float _moveTimer = 0.0f;
	[Export] public float TimeBetweenMoves = 20.0f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_moveTimer > TimeBetweenMoves)
		{
			foreach (Zoomable zoomable in GetNode<Main>("..").Zoomables)
			{
				foreach (Thumbnail thumbnail in zoomable.Thumbnails)
				{
					if (thumbnail.LinkedZoomable.ShipThumbnail.GetOccupier() != Thumbnail.Occupier.ENEMY
						&& zoomable.ShipThumbnail.GetOccupier() == Thumbnail.Occupier.ENEMY)
					{
						if (!zoomable.ShipThumbnail.HasTargetThumbnail(thumbnail))
						{
							zoomable.ConnectThumbnails(zoomable.ShipThumbnail, thumbnail);
							goto connection_added;
						}
					}
				}
			}
			connection_added:
			_moveTimer = 0.0f;
		}
		else
		{
			_moveTimer += (float)delta;
		}

	}
}
