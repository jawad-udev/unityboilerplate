using Lofelt.NiceVibrations;

public interface IVibrationService
{
    bool IsVibrationEnabled { get; set; }
    void VibratePhone(HapticPatterns.PresetType vibrationType);
    void VibratePhoneConstantly();
    void StopVibration();
}
