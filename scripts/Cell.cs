using Godot;
using System;

public partial class Cell : Area2D
{

	private bool selected = false;
	private bool hovered = false;

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
			else if (mouseEvent.Pressed && !hovered)
			{
				UpdateSelectionState(false);
			}
		}
	}

	public override void _MouseEnter()
	{
		hovered = true;
	}

	public override void _MouseExit()
	{
		hovered = false;
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
	}

}
