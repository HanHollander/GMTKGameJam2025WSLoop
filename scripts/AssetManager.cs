using Godot;
using System;

public partial class AssetManager : ResourcePreloader
{

	public static AssetManager Instance { get; private set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		AddResource("connection", GD.Load<PackedScene>("res://scenes/connection.tscn"));
		AddResource("troops-label-neutral", GD.Load<LabelSettings>("res://labels/troops-label-neutral.tres"));
		AddResource("troops-label-player", GD.Load<LabelSettings>("res://labels/troops-label-player.tres"));
		AddResource("troops-label-enemy", GD.Load<LabelSettings>("res://labels/troops-label-enemy.tres"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public PackedScene GetConnectionScene()
	{
		return (PackedScene)GetResource("connection");
	}

	public LabelSettings GetTroopsLabelSettings(Thumbnail.Occupier occupier)
	{
		switch (occupier)
		{
			case Thumbnail.Occupier.NEUTRAL:
				return (LabelSettings)GetResource("troops-label-neutral");
			case Thumbnail.Occupier.PLAYER:
				return (LabelSettings)GetResource("troops-label-player");
			case Thumbnail.Occupier.ENEMY:
				return (LabelSettings)GetResource("troops-label-enemy");
			default:
				throw new Exception("Unnsupported occupier type");
		}
	}
}
