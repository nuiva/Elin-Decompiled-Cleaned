public class SkinRootRedirect : SkinRoot
{
	public string idWidget;

	public override SkinConfig Config => _config ?? (_config = Core.Instance.ui.widgets.configs[idWidget].skin);
}
