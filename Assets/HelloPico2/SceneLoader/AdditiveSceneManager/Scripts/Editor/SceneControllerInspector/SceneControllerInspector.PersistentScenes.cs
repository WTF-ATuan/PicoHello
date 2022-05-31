public partial class SceneControllerInspector
{
    void RenderPersistentScenes()
    {
        RenderSectionHeader("Persistent Scenes",
            "These scenes will not be unloaded by the SceneController's UnloadScene() function.");        
        RenderSceneListPropertyField(persistentScenes);
    }   
}