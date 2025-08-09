using Godot;
using System;

public partial class Connection : Line2D
{

	private Thumbnail first;
	private Thumbnail second;

	public void Init(Thumbnail first, Thumbnail second)
	{
		this.first = first;
		this.second = second;
		AddPoint(first.Position);
		AddPoint(second.Position);
	}

	public override void _Input(InputEvent inEvent)
	{
		if (inEvent is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
		{
			QueueFree();
		}
    }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
