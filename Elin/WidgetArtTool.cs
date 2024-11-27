using System;

public class WidgetArtTool : Widget
{
	public CinemaConfig conf
	{
		get
		{
			return EMono.player.cinemaConfig;
		}
	}

	public override void OnActivate()
	{
		CameraSupport camSupport = EMono.scene.camSupport;
		this.sliderSpeed.SetSlider((float)this.conf.speed, delegate(float a)
		{
			this.conf.speed = (int)a;
			return "Speed (" + ((int)a).ToString() + ")";
		}, -1, -1, true);
		this.sliderBrightness.SetSlider((float)this.conf.brightness, delegate(float a)
		{
			this.conf.brightness = (int)a;
			return "Brightness (" + ((int)a).ToString() + "%)";
		}, -1, -1, true);
		this.sliderLight.SetSlider((float)this.conf.light, delegate(float a)
		{
			this.conf.light = (int)a;
			return "Light (" + ((int)a).ToString() + "%)";
		}, -1, -1, true);
		GameDate d = EMono.world.date;
		this.sliderTime.SetSlider((float)d.hour, delegate(float a)
		{
			Weather.Condition currentCondition = EMono.world.weather.CurrentCondition;
			if (d.hour != (int)a)
			{
				d.hour = (int)a - 1;
				d.AdvanceHour();
				EMono.world.weather.SetCondition(currentCondition, 20, false);
			}
			EMono._map.RefreshFOV(EMono.pc.pos.x, EMono.pc.pos.z, 20, true);
			EMono.screen.RefreshAll();
			return "Time (" + d.hour.ToString() + ")";
		}, false);
		this.sliderTime.SetActive(EMono.debug.enable);
		this.RebuildLayout(true);
	}

	public void Reset()
	{
		EMono.player.cinemaConfig = new CinemaConfig();
		this.sliderBrightness.value = (float)this.conf.brightness;
		this.sliderLight.value = (float)this.conf.light;
		this.sliderSpeed.value = (float)this.conf.speed;
	}

	public UISlider sliderSpeed;

	public UISlider sliderBrightness;

	public UISlider sliderLight;

	public UISlider sliderTime;

	public float zoom = 1f;

	public float hour = 12f;
}
