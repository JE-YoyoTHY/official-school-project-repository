using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "ScreenShake/New Profile")]
public class ScreenShakeProfile : ScriptableObject
{
	[Header("Impulse Source Settings")]
	public float impulseTime = 0.2f;
	public float impulseForce = 1.0f;
	public Vector3 defaultVelocity = new Vector3(0f, -1.0f, 0f);
	public AnimationCurve impulseCurve;


	[Header("Impulse Listener Settings")]
	public float listenerAmplitude = 1.0f;
	public float listenerFrequency = 1.0f;
	public float listenerDuration = 1.0f;

}
