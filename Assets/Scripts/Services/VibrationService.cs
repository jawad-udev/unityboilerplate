using Lofelt.NiceVibrations;

public class VibrationService : IVibrationService
{
    public bool IsVibrationEnabled { get; set; } = true;

    // Legacy field alias
    public bool isVibrationEnable { get => IsVibrationEnabled; set => IsVibrationEnabled = value; }

    public void VibratePhone(HapticPatterns.PresetType vibrationType)
    {
#if PLATFORM_ANDROID || UNITY_ANDROID
        if (IsVibrationEnabled)
            HapticPatterns.PlayPreset(vibrationType);
#endif
    }

    public void VibratePhoneConstantly()
    {
#if PLATFORM_ANDROID || UNITY_ANDROID
        if (IsVibrationEnabled)
            HapticPatterns.PlayConstant(0.1f, 0.1f, 15f);
#endif
    }

    public void StopVibration()
    {
#if PLATFORM_ANDROID || UNITY_ANDROID
        if (IsVibrationEnabled)
            HapticPatterns.PlayConstant(0f, 0f, 0f);
#endif
    }
}