using System;
using UnityEngine;

public class ScreenOverlay : EMono
{
	private void Update()
	{
		this.offsetAnime += this.speed * Core.gameDelta;
		Vector3 vector = EMono.screen.position * -1f;
		vector.x += vector.z * this.heightSpeed.x;
		vector.y += vector.z * this.heightSpeed.y;
		vector.z = 0f;
		Vector3 vector2 = Quaternion.Euler(this.cameraAngle) * vector;
		this._renderer.sharedMaterial.mainTextureOffset = this.offsetAnime + new Vector2(vector2.x * this.cameraSpeed.x, vector2.y * this.cameraSpeed.y);
	}

	public Vector2 speed;

	public Vector2 cameraSpeed;

	public Vector2 heightSpeed;

	public Vector3 cameraAngle;

	public MeshRenderer _renderer;

	private Vector2 offsetAnime;
}
