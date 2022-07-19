namespace HelloPico2.LevelTool
{
    public interface ITrackInteractableState
    {
        public void WhenCollideWith(HelloPico2.InteractableObjects.InteractType type);
    }
}