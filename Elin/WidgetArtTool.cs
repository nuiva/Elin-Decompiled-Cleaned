public class WidgetArtTool : Widget
{
	public UISlider sliderSpeed;

	public UISlider sliderBrightness;

	public UISlider sliderLight;

	public UISlider sliderTime;

	public float zoom = 1f;

	public float hour = 12f;

	public CinemaConfig conf => EMono.player.cinemaConfig;

	public override void OnActivate()
	{
		_ = EMono.scene.camSupport;
		sliderSpeed.SetSlider(conf.speed, delegate(float a)
		{
			conf.speed = (int)a;
			return "Speed (" + (int)a + ")";
		});
		sliderBrightness.SetSlider(conf.brightness, delegate(float a)
		{
			conf.brightness = (int)a;
			return "Brightness (" + (int)a + "%)";
		});
		sliderLight.SetSlider(conf.light, delegate(float a)
		{
			conf.light = (int)a;
			return "Light (" + (int)a + "%)";
		});
		GameDate d = EMono.world.date;
		sliderTime.SetSlider(d.hour, delegate(float a)
		{
			Weather.Condition currentCondition = EMono.world.weather.CurrentCondition;
			if (d.hour != (int)a)
			{
				d.hour = (int)a - 1;
				d.AdvanceHour();
				EMono.world.weather.SetCondition(currentCondition);
			}
			EMono._map.RefreshFOV(EMono.pc.pos.x, EMono.pc.pos.z, 20, recalculate: true);
			EMono.screen.RefreshAll();
			return "Time (" + d.hour + ")";
		}, notify: false);
		sliderTime.SetActive(EMono.debug.enable);
		this.RebuildLayout(recursive: true);
	}

	public void Reset()
	{
		EMono.player.cinemaConfig = new CinemaConfig();
		sliderBrightness.value = conf.brightness;
		sliderLight.value = conf.light;
		sliderSpeed.value = conf.speed;
	}
}
