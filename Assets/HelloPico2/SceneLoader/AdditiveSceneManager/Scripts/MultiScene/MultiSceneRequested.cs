namespace HelloPico2.SceneLoader.AdditiveSceneManager.Scripts.MultiScene{
	public class MultiSceneRequested{
		public SceneObject SceneObject{ get; }

		public LoadOptions LoadOption = LoadOptions.Load;

		public MultiSceneRequested(SceneObject sceneObject){
			SceneObject = sceneObject;
		}

		public enum LoadOptions{
			Load,
			BackgroundLoad,
			ActiveBackground,
			UnLoad
		}
	}
}