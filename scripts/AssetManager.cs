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
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public PackedScene GetConnectionScene()
	{
		return (PackedScene) GetResource("connection");
	}
}
