using HelloPico2.InputDevice.Scripts;

public interface IGainEnergyFeedback
{
    void OnNotify(HelloPico2.PlayerController.Arm.ArmData armdata);
}