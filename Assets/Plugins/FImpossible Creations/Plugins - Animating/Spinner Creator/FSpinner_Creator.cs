using UnityEngine;

namespace FIMSpace.FSpinner
{
    /// <summary>
    /// FM: Class to animate smooth spinner for loading etc.
    /// Use parameters to create unique animations, there are millions of possibilibies!
    /// Version 1.1 - Supporting live preview and deriving from base class
    /// </summary>
    [AddComponentMenu("FImpossible Creations/Spinner Creator/FSpinner Creator")]
    public class FSpinner_Creator : FSpinner_CreatorBase
    {
        [Header("Preview animation in editor mode")]
        public bool PreviewInEditor = false;
        internal FSpinner_CreatorLivePreview livePreview;
        
        protected override void Start()
        {
            base.Start();

            if (livePreview) livePreview.RemoveMe = true;
        }

        protected override void OnValidate()
        {

            base.OnValidate();

#if UNITY_EDITOR
            if (UnityEditor.Selection.activeGameObject == gameObject) return;
            if (UnityEditor.EditorApplication.isCompiling) return;
#endif

            if (!Application.isPlaying)
            {
                if (PreviewInEditor)
                {
                    if (!livePreview)
                    {
                        GameObject livePrev = new GameObject("Live Preview For " + name);
                        livePrev.transform.SetParent(transform);
                        livePrev.transform.localPosition = Vector3.zero;
                        livePrev.transform.localRotation = Quaternion.identity;
                        livePrev.transform.localScale = Vector3.one;
                        livePrev.AddComponent<RectTransform>();

                        livePreview = livePrev.AddComponent<FSpinner_CreatorLivePreview>();
                        livePreview.Parent = this;
                    }
                    else
                    {
                        livePreview.ValidateTrigger();
                    }
                }
                else
                    if (livePreview) livePreview.RemoveMe = true;
            }
        }
    }
}