using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Thumbnail : Area2D
{
	[Export] private int BASE_TROOPS = 10;
	[Export] private int MIN_TROOPS_FOR_ATTACK = 10;
	[Export] private double REGEN_INTERVAL = 1.0f;
	[Export] private int REGEN_AMMOUNT = 1;
	[Export] private double ATTACK_INTERVAL = 3.0f;
	[Export] private int ATTACK_AMMOUNT = 2;

	[Export] private ThumbnailType thumbnailType = ThumbnailType.NORMAL;
	[Export] private Occupier occupier = Occupier.NEUTRAL;

	[Export] private float SelectRotationSpeed = -2.0f;

	private double regenTimer = 0f;
	private double attackTimer = 0f;

	public enum Occupier
	{
		NEUTRAL, PLAYER, ENEMY
	}

	public enum ThumbnailType
	{
		NORMAL, MOTHER_SHIP, SHIP
	}

	private bool selected = false;
	private List<Connection> outgoingConnections = [];
	private int troops = 0;

	[Export] public Zoomable LinkedZoomable;

	[Signal]
	public delegate void ThumbnailSelectionChangedEventHandler(Thumbnail thumbnail, bool selected);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		troops = BASE_TROOPS;
		UpdateSelectionState(false);
		UpdateGraphics();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (thumbnailType == ThumbnailType.NORMAL)
		{
			troops = LinkedZoomable.ShipThumbnail.GetTroops();
			occupier = LinkedZoomable.ShipThumbnail.GetOccupier();
		}
		else if (occupier != Occupier.NEUTRAL)
		{
			regenTimer += delta;
			attackTimer += delta;
			if (regenTimer > REGEN_INTERVAL)
			{
				troops += REGEN_AMMOUNT;
				regenTimer -= REGEN_INTERVAL;
			}
			if (attackTimer > ATTACK_INTERVAL)
			{
				DoAttack();
				attackTimer -= ATTACK_INTERVAL;
			}
		}
		
		UpdateHighlightRotation(delta);
		UpdateGraphics();
	}

	private void DoAttack()
	{
		if (troops > MIN_TROOPS_FOR_ATTACK && outgoingConnections.Count > 0) // && occupier != Occupier.NEUTRAL)
		{
			int attackAmmount = Math.Min(ATTACK_AMMOUNT, troops - MIN_TROOPS_FOR_ATTACK);
			troops -= attackAmmount;
			while (attackAmmount > 0)
			{
				foreach (Connection connection in outgoingConnections)
				{
					attackAmmount--;
					connection.GetTargetThumbnail().LinkedZoomable.ShipThumbnail.ReceiveTroops(1, occupier);
				}
			}
		}
	}


	/* 
	 * ===========
	 * == INPUT ==
	 * ===========
	 */

	public override void _InputEvent(Viewport viewport, InputEvent inEvent, int shapeIdx)
	{
		if (inEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Left)
		{
			if (mouseEvent.Pressed)
			{
				UpdateSelectionState(!selected);
			}
		}
	}


	/* 
	 * ============
	 * == HELPER ==
	 * ============
	 */

	public void UpdateSelectionState(bool select)
	{
		selected = select;
		Sprite2D highlight = GetNode<Sprite2D>("Highlight");
		if (selected)
		{
			highlight.Show();
		}
		else
		{
			highlight.Hide();
		}
		EmitSignal(SignalName.ThumbnailSelectionChanged, this, selected);
	}

	public void UpdateGraphics()
	{
		Label label = GetNode<Label>("Troops");
		label.Text = troops.ToString();
		label.LabelSettings = AssetManager.Instance.GetTroopsLabelSettings(occupier);

		if (thumbnailType == ThumbnailType.NORMAL)
		{
			SetBackgrounds();
		}
		else
		{
			GetNode<Sprite2D>("Ship").Texture = AssetManager.Instance.GetShipTexture(thumbnailType, occupier);
		}
	}

	public bool IsSelected()
	{
		return selected;
	}

	public void AddOutgoingConnection(Connection connection)
	{
		outgoingConnections.Add(connection);
	}

	public void RemoveOutgoingConnection(Connection connection)
	{
		outgoingConnections.Remove(connection);
	}

	public bool HasTargetThumbnail(Thumbnail thumbnail)
	{
		foreach (Connection connection in outgoingConnections)
		{
			if (connection.GetTargetThumbnail() == thumbnail)
			{
				return true;
			}
		}
		return false;
	}

	public Thumbnail.Occupier GetOccupier()
	{
		return occupier;
	}

	public Thumbnail.ThumbnailType GetThumbnailType()
	{
		return thumbnailType;
	}

	public void ReceiveTroops(int nofTroops, Occupier sender)
	{
		if (sender != occupier)
		{
			int newTroopCount = troops - nofTroops;
			if (newTroopCount <= 0)
			{
				occupier = sender;
				for (int i = outgoingConnections.Count - 1; i >= 0; i--)
				{
					outgoingConnections[i].RemoveConnection();
				}
			}
			troops = Math.Abs(newTroopCount);
		}
		else
		{
			troops += nofTroops;
		}
	}

	public void SetTroops(int nofTroops)
	{
		troops = nofTroops;
	}

	public int GetTroops()
	{
		return troops;
	}

	public void SetBackgrounds()
	{
		GetNode<Sprite2D>("Background").Texture = AssetManager.Instance.GetZoomableTexture(occupier, LinkedZoomable.ZoomableShapeType);
		LinkedZoomable.GetNode<Sprite2D>("Background").Texture = AssetManager.Instance.GetZoomableTexture(occupier, LinkedZoomable.ZoomableShapeType);
	}

	public void UpdateHighlightRotation(double delta)
	{
		float oldRotation = GetNode<Sprite2D>("Highlight").Rotation;
		float dRotation = (float)(delta / 1.0f * SelectRotationSpeed);
		float newRotation = (oldRotation + dRotation) % 360.0f;
		GetNode<Sprite2D>("Highlight").Rotation = newRotation;
	
	}
}
