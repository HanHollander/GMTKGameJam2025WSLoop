using Godot;
using System;

public partial class Thumbnail : Area2D
{

	private bool selected = false;

	[Export] public Zoomable LinkedZoomable;

	[Signal]
	public delegate void ThumbnailSelectedEventHandler();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		UpdateSelectionState(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
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
			EmitSignal(SignalName.ThumbnailSelected);
		}
		else
		{
			highlight.Hide();
		}
	}

	public bool IsSelected()
	{
		return selected;
	}
}
