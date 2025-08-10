using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Thumbnail : Area2D
{
	private static int BASE_TROOPS = 10;
	private static int MIN_TROOPS_FOR_ATTACK = 10;
	private static double REGEN_INTERVAL = 1.0f;
	private static int REGEN_AMMOUNT = 1;
	private static double ATTACK_INTERVAL = 3.0f;
	private static int ATTACK_AMMOUNT = 2;

	private double regenTimer = 0f;
	private double attackTimer = 0f;

	public enum Occupier
	{
		NEUTRAL, PLAYER, ENEMY
	}

	private bool selected = false;
	private List<Connection> outgoingConnections = [];
	private Occupier occupier = Occupier.NEUTRAL;
	private int troops = BASE_TROOPS;

	[Export] public Zoomable LinkedZoomable;

	[Signal]
	public delegate void ThumbnailSelectionChangedEventHandler(Thumbnail thumbnail, bool selected);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateSelectionState(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		regenTimer += delta;
		attackTimer += delta;
		if (regenTimer > REGEN_INTERVAL) // && occupier != Occupier.NEUTRAL)
		{
			troops += REGEN_AMMOUNT;
			regenTimer -= REGEN_INTERVAL;
		}
		if (attackTimer > ATTACK_INTERVAL)
		{
			DoAttack();
			attackTimer -= ATTACK_INTERVAL;
		}
		UpdateTroopLabel();
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
					connection.GetTargetThumbnail().ReceiveTroops(1, occupier);
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
			EmitSignal(SignalName.ThumbnailSelectionChanged, this, selected);
		}
		else
		{
			highlight.Hide();
		}
	}

	public void UpdateTroopLabel()
	{
		GetNode<Label>("Troops").Text = troops.ToString();
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

	public void ReceiveTroops(int nofTroops, Occupier sender)
	{
		if (sender != occupier)
		{
			int newTroopCount = troops - nofTroops;
			if (newTroopCount <= 0)
			{
				occupier = sender;
			}
			troops = Math.Abs(newTroopCount);
		}
		else
		{
			troops += nofTroops;
		}
	}
}
