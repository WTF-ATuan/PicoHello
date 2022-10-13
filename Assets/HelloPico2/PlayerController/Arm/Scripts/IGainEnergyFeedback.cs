using HelloPico2.InputDevice.Scripts;
using HelloPico2.PlayerController.Arm;

public interface IGainEnergyFeedback
{
    void OnNotify(ArmData armdata, EnergyBallBehavior energyBallBehavior);
}