using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public partial class AssetManager : ResourcePreloader
{

	public static AssetManager Instance { get; private set; }

	private static Dictionary<Thumbnail.Occupier, Dictionary<Zoomable.ShapeType, String>> spriteLookup = new()
	{
		{   Thumbnail.Occupier.PLAYER,
			new()
			{
				{ Zoomable.ShapeType.A, "so-a-b"},
				{ Zoomable.ShapeType.B, "so-b-b"},
				{ Zoomable.ShapeType.C, "so-c-b"},
				{ Zoomable.ShapeType.D, "so-d-b"},
			}
		},
		{   Thumbnail.Occupier.ENEMY,
			new()
			{
				{ Zoomable.ShapeType.A, "so-a-r"},
				{ Zoomable.ShapeType.B, "so-b-r"},
				{ Zoomable.ShapeType.C, "so-c-r"},
				{ Zoomable.ShapeType.D, "so-d-r"},
			}
		},
		{   Thumbnail.Occupier.NEUTRAL,
			new()
			{
				{ Zoomable.ShapeType.A, "so-a-n"},
				{ Zoomable.ShapeType.B, "so-b-n"},
				{ Zoomable.ShapeType.C, "so-c-n"},
				{ Zoomable.ShapeType.D, "so-d-n"},
			}
		},
		
	};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		AddResource("connection", GD.Load<PackedScene>("res://scenes/connection.tscn"));
		AddResource("troops-label-neutral", GD.Load<LabelSettings>("res://labels/troops-label-neutral.tres"));
		AddResource("troops-label-player", GD.Load<LabelSettings>("res://labels/troops-label-player.tres"));
		AddResource("troops-label-enemy", GD.Load<LabelSettings>("res://labels/troops-label-enemy.tres"));

		AddResource("so-a-b", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectA-B.png"));
		AddResource("so-b-b", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectB-B.png"));
		AddResource("so-c-b", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectC-B.png"));
		AddResource("so-d-b", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectD-B.png"));
		AddResource("so-a-r", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectA-R.png"));
		AddResource("so-b-r", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectB-R.png"));
		AddResource("so-c-r", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectC-R.png"));
		AddResource("so-d-r", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectD-R.png"));
		AddResource("so-a-n", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectA-N.png"));
		AddResource("so-b-n", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectB-N.png"));
		AddResource("so-c-n", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectC-N.png"));
		AddResource("so-d-n", GD.Load<CompressedTexture2D>("res://sprites/space_obj/SpaceObjectD-N.png"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public PackedScene GetConnectionScene()
	{
		return (PackedScene)GetResource("connection");
	}

	public CompressedTexture2D GetZoomableTexture(Thumbnail.Occupier occupier, Zoomable.ShapeType shapeType)
	{
		return (CompressedTexture2D)GetResource(spriteLookup[occupier][shapeType]);
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
