using Newtonsoft.Json;

public class HotItemActionMode : HotItem
{
	[JsonProperty]
	public string id;

	public override string Name => id.lang();

	public override string pathSprite => "icon_" + id;

	public override bool KeepVisibleWhenHighlighted => true;

	public static void Execute(string id)
	{
		switch (id)
		{
		case "Inspect":
			if (!EClass.scene.actionMode.IsBuildMode)
			{
				BuildMenu.Toggle();
			}
			else
			{
				ActionMode.Inspect.Activate(toggle: false);
			}
			break;
		case "Cut":
			ActionMode.Cut.Activate(toggle: false);
			break;
		case "Mine":
			ActionMode.Mine.Activate(toggle: false);
			break;
		case "StateEditor":
			ActionMode.StateEditor.Activate(toggle: false);
			break;
		case "Dig":
			ActionMode.Dig.Activate(TaskDig.Mode.Default);
			break;
		case "DigFloor":
			ActionMode.Dig.Activate(TaskDig.Mode.RemoveFloor);
			break;
		case "RemoveDesignation":
			ActionMode.RemoveDesignation.Activate(toggle: false);
			break;
		case "Deconstruct":
			ActionMode.Deconstruct.Activate(toggle: false);
			break;
		case "EditArea":
			ActionMode.EditArea.Activate(toggle: false);
			break;
		case "Picker":
			ActionMode.Picker.Activate(toggle: false);
			break;
		case "Terrain":
			ActionMode.Terrain.Activate(toggle: false);
			break;
		case "Populate":
			ActionMode.Populate.Activate(toggle: false);
			break;
		case "EditMarker":
			ActionMode.EditMarker.Activate(toggle: false);
			break;
		case "Visibility":
			ActionMode.Visibility.Activate(toggle: false);
			break;
		case "Cinema":
			ActionMode.Cinema.Activate(toggle: false);
			break;
		case "FlagCell":
			ActionMode.FlagCell.Activate(toggle: false);
			break;
		case "ExitBuild":
			ActionMode.DefaultMode.Activate(toggle: false);
			break;
		case "Copy":
			ActionMode.Copy.Activate(toggle: false);
			break;
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		Execute(id);
	}

	public override bool ShouldHighlight()
	{
		ActionMode actionMode = EClass.scene.actionMode;
		switch (id)
		{
		case "Inspect":
			if (actionMode != ActionMode.Inspect)
			{
				return actionMode == ActionMode.Build;
			}
			return true;
		case "Cut":
			return actionMode == ActionMode.Cut;
		case "Mine":
			return actionMode == ActionMode.Mine;
		case "Dig":
			if (actionMode == ActionMode.Dig)
			{
				return ActionMode.Dig.mode == TaskDig.Mode.Default;
			}
			return false;
		case "DigFloor":
			if (actionMode == ActionMode.Dig)
			{
				return ActionMode.Dig.mode == TaskDig.Mode.RemoveFloor;
			}
			return false;
		case "RemoveDesignation":
			return actionMode == ActionMode.RemoveDesignation;
		case "StateEditor":
			return actionMode == ActionMode.StateEditor;
		case "Deconstruct":
			return actionMode == ActionMode.Deconstruct;
		case "Picker":
			return actionMode == ActionMode.Picker;
		case "EditArea":
			if (actionMode != ActionMode.EditArea && actionMode != ActionMode.CreateArea)
			{
				return actionMode == ActionMode.ExpandArea;
			}
			return true;
		case "Terrain":
			return actionMode == ActionMode.Terrain;
		case "EditMarker":
			return actionMode == ActionMode.EditMarker;
		case "Populate":
			return actionMode == ActionMode.Populate;
		case "Visibility":
			return actionMode == ActionMode.Visibility;
		case "Cinema":
			return actionMode == ActionMode.Cinema;
		case "Copy":
			return actionMode == ActionMode.Copy;
		case "FlagCell":
			return actionMode == ActionMode.FlagCell;
		default:
			return false;
		}
	}
}
