using System;
using Newtonsoft.Json;

public class HotItemActionMode : HotItem
{
	public override string Name
	{
		get
		{
			return this.id.lang();
		}
	}

	public override string pathSprite
	{
		get
		{
			return "icon_" + this.id;
		}
	}

	public override bool KeepVisibleWhenHighlighted
	{
		get
		{
			return true;
		}
	}

	public static void Execute(string id)
	{
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1491228771U)
		{
			if (num <= 996747083U)
			{
				if (num <= 547102523U)
				{
					if (num != 482302120U)
					{
						if (num != 547102523U)
						{
							return;
						}
						if (!(id == "Deconstruct"))
						{
							return;
						}
						ActionMode.Deconstruct.Activate(false, false);
						return;
					}
					else
					{
						if (!(id == "EditArea"))
						{
							return;
						}
						ActionMode.EditArea.Activate(false, false);
						return;
					}
				}
				else if (num != 561074963U)
				{
					if (num != 996747083U)
					{
						return;
					}
					if (!(id == "Dig"))
					{
						return;
					}
					ActionMode.Dig.Activate(TaskDig.Mode.Default);
					return;
				}
				else
				{
					if (!(id == "Populate"))
					{
						return;
					}
					ActionMode.Populate.Activate(false, false);
					return;
				}
			}
			else if (num <= 1143685331U)
			{
				if (num != 1027767735U)
				{
					if (num != 1143685331U)
					{
						return;
					}
					if (!(id == "EditMarker"))
					{
						return;
					}
					ActionMode.EditMarker.Activate(false, false);
					return;
				}
				else
				{
					if (!(id == "Inspect"))
					{
						return;
					}
					if (!EClass.scene.actionMode.IsBuildMode)
					{
						BuildMenu.Toggle();
						return;
					}
					ActionMode.Inspect.Activate(false, false);
					return;
				}
			}
			else if (num != 1198054235U)
			{
				if (num != 1286637829U)
				{
					if (num != 1491228771U)
					{
						return;
					}
					if (!(id == "Cut"))
					{
						return;
					}
					ActionMode.Cut.Activate(false, false);
					return;
				}
				else
				{
					if (!(id == "Visibility"))
					{
						return;
					}
					ActionMode.Visibility.Activate(false, false);
					return;
				}
			}
			else
			{
				if (!(id == "FlagCell"))
				{
					return;
				}
				ActionMode.FlagCell.Activate(false, false);
				return;
			}
		}
		else if (num <= 2316551543U)
		{
			if (num <= 2056661452U)
			{
				if (num != 1703884388U)
				{
					if (num != 2056661452U)
					{
						return;
					}
					if (!(id == "RemoveDesignation"))
					{
						return;
					}
					ActionMode.RemoveDesignation.Activate(false, false);
					return;
				}
				else
				{
					if (!(id == "Copy"))
					{
						return;
					}
					ActionMode.Copy.Activate(false, false);
					return;
				}
			}
			else if (num != 2207663855U)
			{
				if (num != 2316551543U)
				{
					return;
				}
				if (!(id == "StateEditor"))
				{
					return;
				}
				ActionMode.StateEditor.Activate(false, false);
				return;
			}
			else
			{
				if (!(id == "Picker"))
				{
					return;
				}
				ActionMode.Picker.Activate(false, false);
				return;
			}
		}
		else if (num <= 2818509998U)
		{
			if (num != 2778370147U)
			{
				if (num != 2818509998U)
				{
					return;
				}
				if (!(id == "Terrain"))
				{
					return;
				}
				ActionMode.Terrain.Activate(false, false);
				return;
			}
			else
			{
				if (!(id == "ExitBuild"))
				{
					return;
				}
				ActionMode.DefaultMode.Activate(false, false);
				return;
			}
		}
		else if (num != 3137301143U)
		{
			if (num != 3848897750U)
			{
				if (num != 3870556090U)
				{
					return;
				}
				if (!(id == "Cinema"))
				{
					return;
				}
				ActionMode.Cinema.Activate(false, false);
				return;
			}
			else
			{
				if (!(id == "Mine"))
				{
					return;
				}
				ActionMode.Mine.Activate(false, false);
				return;
			}
		}
		else
		{
			if (!(id == "DigFloor"))
			{
				return;
			}
			ActionMode.Dig.Activate(TaskDig.Mode.RemoveFloor);
			return;
		}
	}

	public override void OnClick(ButtonHotItem b, Hotbar h)
	{
		HotItemActionMode.Execute(this.id);
	}

	public override bool ShouldHighlight()
	{
		ActionMode actionMode = EClass.scene.actionMode;
		string text = this.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(text);
		if (num <= 1286637829U)
		{
			if (num <= 996747083U)
			{
				if (num <= 547102523U)
				{
					if (num != 482302120U)
					{
						if (num == 547102523U)
						{
							if (text == "Deconstruct")
							{
								return actionMode == ActionMode.Deconstruct;
							}
						}
					}
					else if (text == "EditArea")
					{
						return actionMode == ActionMode.EditArea || actionMode == ActionMode.CreateArea || actionMode == ActionMode.ExpandArea;
					}
				}
				else if (num != 561074963U)
				{
					if (num == 996747083U)
					{
						if (text == "Dig")
						{
							return actionMode == ActionMode.Dig && ActionMode.Dig.mode == TaskDig.Mode.Default;
						}
					}
				}
				else if (text == "Populate")
				{
					return actionMode == ActionMode.Populate;
				}
			}
			else if (num <= 1143685331U)
			{
				if (num != 1027767735U)
				{
					if (num == 1143685331U)
					{
						if (text == "EditMarker")
						{
							return actionMode == ActionMode.EditMarker;
						}
					}
				}
				else if (text == "Inspect")
				{
					return actionMode == ActionMode.Inspect || actionMode == ActionMode.Build;
				}
			}
			else if (num != 1198054235U)
			{
				if (num == 1286637829U)
				{
					if (text == "Visibility")
					{
						return actionMode == ActionMode.Visibility;
					}
				}
			}
			else if (text == "FlagCell")
			{
				return actionMode == ActionMode.FlagCell;
			}
		}
		else if (num <= 2207663855U)
		{
			if (num <= 1703884388U)
			{
				if (num != 1491228771U)
				{
					if (num == 1703884388U)
					{
						if (text == "Copy")
						{
							return actionMode == ActionMode.Copy;
						}
					}
				}
				else if (text == "Cut")
				{
					return actionMode == ActionMode.Cut;
				}
			}
			else if (num != 2056661452U)
			{
				if (num == 2207663855U)
				{
					if (text == "Picker")
					{
						return actionMode == ActionMode.Picker;
					}
				}
			}
			else if (text == "RemoveDesignation")
			{
				return actionMode == ActionMode.RemoveDesignation;
			}
		}
		else if (num <= 2818509998U)
		{
			if (num != 2316551543U)
			{
				if (num == 2818509998U)
				{
					if (text == "Terrain")
					{
						return actionMode == ActionMode.Terrain;
					}
				}
			}
			else if (text == "StateEditor")
			{
				return actionMode == ActionMode.StateEditor;
			}
		}
		else if (num != 3137301143U)
		{
			if (num != 3848897750U)
			{
				if (num == 3870556090U)
				{
					if (text == "Cinema")
					{
						return actionMode == ActionMode.Cinema;
					}
				}
			}
			else if (text == "Mine")
			{
				return actionMode == ActionMode.Mine;
			}
		}
		else if (text == "DigFloor")
		{
			return actionMode == ActionMode.Dig && ActionMode.Dig.mode == TaskDig.Mode.RemoveFloor;
		}
		return false;
	}

	[JsonProperty]
	public string id;
}
