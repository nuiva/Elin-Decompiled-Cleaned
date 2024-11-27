using System;
using UnityEngine;

public class RandomPositionMover : MonoBehaviour
{
	private void Start()
	{
		if (this.pickerInterval == 0f)
		{
			this.pickerInterval = 3f;
		}
		this.randomPointInCircle = Vector2.zero;
		base.InvokeRepeating("PickRandomPointInCircle", UnityEngine.Random.Range(0f, this.pickerInterval), this.pickerInterval);
	}

	private void PickRandomPointInCircle()
	{
		base.transform.position = this.player.transform.position;
		this.randomPointInCircle = base.transform.localPosition + UnityEngine.Random.insideUnitCircle * this.radius;
		base.transform.localPosition = this.randomPointInCircle;
	}

	private void Update()
	{
	}

	public float pickerInterval;

	public float radius;

	public GameObject player;

	public Vector2 randomPointInCircle;
}
