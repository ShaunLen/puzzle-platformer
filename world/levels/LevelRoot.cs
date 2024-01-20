using System.Collections.Generic;
using Godot;
using CodeManager = PuzzlePlatformer.ui.code.CodeManager;

namespace PuzzlePlatformer.world.levels;

[GlobalClass]
public abstract partial class LevelRoot : Node2D
{
	[ExportCategory("Level Information")]
	[Export] public string LevelName { get; set; }
	[Export(PropertyHint.MultilineText)] public string LevelDesc { get; set; }
	[Export(PropertyHint.MultilineText)] public string InitialCode { get; set; }
	[Export(PropertyHint.Range, "1,2,0.2")]  public float DefaultZoom { get; set; }
	[Export] private Node2D _respawnPosition;
	[Export] private protected TileMap GroundTilemap;
	[Export] private protected TileMap BackgroundTilemap; 
	[Export] private Color _backgroundColor = new(143, 208, 255, 255);
	
	public Vector2 LevelBounds;
	
	public List<requirements.Requirement> Requirements { get; set; }

	public override void _Ready()
	{
		LevelBounds = GroundTilemap.GetUsedRect().Size * GroundTilemap.CellQuadrantSize;
		DefineRequirements();
		AddInitialCode();
		BackgroundTilemap.Modulate = _backgroundColor;
	}
	
	public Vector2 GetRespawnPosition()
	{
		return _respawnPosition.Position;
	}

	private void AddInitialCode()
	{
		// TODO: Add settings which has 3 levels of pre code population
		
		if(!LevelManager.Instance.CodelessLevel)
			CodeManager.Instance.SetCode(InitialCode);
	}
	
	protected abstract void DefineRequirements();
}
