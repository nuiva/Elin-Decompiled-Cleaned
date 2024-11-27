using System;

public class SkinRootRedirect : SkinRoot
{
	public override SkinConfig Config
	{
		get
		{
			SkinConfig result;
			if ((result = this._config) == null)
			{
				result = (this._config = Core.Instance.ui.widgets.configs[this.idWidget].skin);
			}
			return result;
		}
	}

	public string idWidget;
}
