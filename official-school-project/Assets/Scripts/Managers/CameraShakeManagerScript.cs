using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeManagerScript : MonoBehaviour
{
	//singleton
	public static CameraShakeManagerScript instance {  get; private set; }

	private CinemachineImpulseDefinition impulseDefinition;
	private CinemachineImpulseListener impulseListener;

	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(this);
		}
		else
		{
			instance = this;
		}
	}

	//public void cameraShake(CinemachineImpulseSource impulseSource)
	//{
	//	impulseSource.GenerateImpulseWithForce(1.0f);
	//}

	public void cameraShakeWithProfile(ScreenShakeProfile profile,  CinemachineImpulseSource impulseSource)
	{
		setupScreenShakeSettings(profile, impulseSource);

		impulseSource.GenerateImpulseWithForce(profile.impulseForce);
	}

	public void cameraShakeWithProfileWithRandomDirection(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
	{
		setupScreenShakeSettings(profile, impulseSource);

		//random
		Vector2 randomDirection = new Vector2(Random.Range(-1.0f, 1f), Random.Range(-1.0f, 1f));
		impulseSource.m_DefaultVelocity = profile.defaultVelocity;

		impulseSource.GenerateImpulseWithForce(profile.impulseForce);
	}

	private void setupScreenShakeSettings(ScreenShakeProfile profile, CinemachineImpulseSource impulseSource)
	{
		impulseDefinition = impulseSource.m_ImpulseDefinition;

		//change the impulse source setting
		impulseDefinition.m_ImpulseDuration = profile.impulseTime;
		impulseSource.m_DefaultVelocity = profile.defaultVelocity;
		impulseDefinition.m_CustomImpulseShape = profile.impulseCurve;

		//change the lister settings
		impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
		impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
		impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;
	}


	public void changeImpulseListener(CinemachineImpulseListener newListener)
	{
		impulseListener = newListener;
	}

}
