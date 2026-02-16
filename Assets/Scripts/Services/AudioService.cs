using UnityEngine;

public class AudioService : MonoBehaviour, IAudioService
{
	public bool isSoundEnable = true;
	public bool isMusicEnable = true;

	public bool IsSoundEnabled { get => isSoundEnable; set => isSoundEnable = value; }
	public bool IsMusicEnabled { get => isMusicEnable; set => isMusicEnable = value; }

	#region Gameplay Specific
	public AudioClip shapePlaceSound;
	public AudioClip shapeMoveSound;
	public AudioClip explosionSound;
	public AudioClip blockSpawnSound;
	public AudioClip timeCountDownSound;
	public AudioClip shapeRotateSound;
	#endregion

	public AudioClip gameMusic;
	public AudioClip uiClick;
	public AudioClip winSound;
	public AudioClip loseSound;
	public AudioClip popUpOpen;
	public AudioClip popUpClose;

	#region Audio Sources Fields
	public AudioSource musicSource;
	public AudioSource soundSource;
	#endregion

	/// <summary>
	/// Central method to play a sound clip. Handles the mute check in one place.
	/// </summary>
	public void PlaySound(AudioClip clip)
	{
		if (!isSoundEnable || clip == null) return;
		soundSource.PlayOneShot(clip);
	}

	#region Sound FX Methods
	public void PlayLoseSound()
	{
		StopGameMusic();
		PlaySound(loseSound);
	}

	public void PlayUIClick() => PlaySound(uiClick);

	public void PlayWinSound()
	{
		StopGameMusic();
		PlaySound(winSound);
	}

	public void PlaySplashScreenSound() { }
	public void PlayPopUpOpenSound() => PlaySound(popUpOpen);
	public void PlayPopUpCloseSound() => PlaySound(popUpClose);
	public void PlayShapePlaceSound() => PlaySound(shapePlaceSound);
	public void PlayShapeMoveSound() => PlaySound(shapeMoveSound);
	public void PlayExplosionSound() => PlaySound(explosionSound);
	public void PlayBlockSpawnSound() => PlaySound(blockSpawnSound);
	public void PlayTimeCountDownSound() => PlaySound(timeCountDownSound);
	public void PlayShapeRotateSound() => PlaySound(shapeRotateSound);

	public void SetSoundFxVolume(float value)
	{
		float temp = value + soundSource.volume;
		if (temp >= 0 && temp <= 1)
			soundSource.volume = temp;
	}
	#endregion

	#region Music Methods

	public void EnableGameMusic(bool enable)
	{
		isMusicEnable = enable;

		if (isMusicEnable)
			PlayGameMusic();
		else
			StopGameMusic();
	}

	public void PlayGameMusic()
	{
		musicSource.clip = gameMusic;
		musicSource.Play();
	}

	public void RestartGameMusic()
	{
		StopGameMusic();
		musicSource.clip = gameMusic;
		musicSource.Play();
	}

	public void StopGameMusic()
	{
		musicSource.Stop();
	}

	public void SetSoundMusicVolume(float value)
	{
		float temp = value + musicSource.volume;
		if (temp >= 0 && temp <= 1)
			musicSource.volume = temp;
	}
	#endregion
}