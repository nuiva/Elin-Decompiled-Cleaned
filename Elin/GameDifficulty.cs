using System;

[Serializable]
public class GameDifficulty : EClass
{
	public int ID
	{
		get
		{
			return EClass.setting.start.difficulties.IndexOf(this);
		}
	}

	public string Name
	{
		get
		{
			return Lang.GetList("difficulties")[this.ID];
		}
	}

	public bool allowManualSave;

	public bool allowRevive;

	public bool deleteGameOnDeath;
}
