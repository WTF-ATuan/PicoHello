using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FIMSpace.FSpinner
{
#if UNITY_EDITOR
    [ExecuteInEditMode]
#endif

    public class FSpinner_CreatorLivePreview : FSpinner_CreatorBase
    {
        public FSpinner_Creator Parent;
        public bool RemoveMe = false;

        private List<GameObject> generatedObjects;
        private List<GameObject> toRemoveEditor;

#if UNITY_EDITOR
        protected void OnEnable()
        {
            UnityEditor.EditorApplication.update += UpdateLivePreview;
        }

        protected void OnDisable()
        {
            UnityEditor.EditorApplication.update -= UpdateLivePreview;
        }
#endif

        protected override void Start()
        {
            if (Application.isPlaying)
            {
                RemoveMe = true;
                return;
            }

            generatedObjects = new List<GameObject>();

            if (!gameObject.GetComponent<CanvasGroup>()) gameObject.AddComponent<CanvasGroup>().alpha = 0.5f;
        }

        protected override void OnValidate() { }

        protected override void Update()
        {
            if ( Application.isPlaying)
            {
                CheckIfShouldBeRemoved();
            }
        }


        protected void UpdateLivePreview()
        {

#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isCompiling) return;
#endif

            if (toRemoveEditor != null)
            {
                for (int i = toRemoveEditor.Count-1; i >= 0; i--)
                {
                    if (toRemoveEditor[i] != null) GameObject.DestroyImmediate(toRemoveEditor[i]);
                    toRemoveEditor.RemoveAt(i);
                }
            }

            CheckIfShouldBeRemoved();

            if ( !RemoveMe)
            {
                if ( Parent )
                {
                    if (smallDotsWaver == null)
                    {
                        smallDotsWaver = new FSpinner_CreatorWaver();
                        bigDotsWaver = new FSpinner_CreatorWaver();
                        bigRadiusWaver = new FSpinner_CreatorWaver();
                        sinusWaver = new FSpinner_CreatorWaver();
                        cosinusWaver = new FSpinner_CreatorWaver();
                    }

                    if (smallDotsContainer == null)
                    {
                        rect = GetComponent<RectTransform>();

                        if (generatedObjects == null) generatedObjects = new List<GameObject>();

                        if (BigDotPrefab)
                        {
                            bigDotsContainer = new GameObject(name + "-BigDots").AddComponent<RectTransform>();
                            bigDotsContainer.SetParent(transform, false);
                            bigDotsContainer.sizeDelta = rect.sizeDelta;
                            bigDotsContainer.anchoredPosition = Vector3.zero;
                            if ( bigDotsContainer ) generatedObjects.Add(bigDotsContainer.gameObject);
                        }

                        if (SmallDotPrefab)
                        {
                            smallDotsContainer = new GameObject(name + "-SmallDots").AddComponent<RectTransform>();
                            smallDotsContainer.SetParent(transform, false);
                            smallDotsContainer.sizeDelta = rect.sizeDelta;
                            smallDotsContainer.anchoredPosition = Vector3.zero;
                            generatedObjects.Add(smallDotsContainer.gameObject);
                        }

                        dotsBigImages = new List<Image>();
                        dotsSmallImages = new List<Image>();

                        Transform cent = transform.Find("Spinner Center");
                        if (cent)
                        {
                            cent = GameObject.Instantiate(cent.gameObject).transform;
                            cent.name = "LivePreview Center";
                            dotsCenter = cent.GetComponent<Image>();
                            generatedObjects.Add(cent.gameObject);
                        }

                        AssignObjects();
                    }


                    BigDotPrefab = Parent.BigDotPrefab;
                    SmallDotPrefab = Parent.SmallDotPrefab;

                    SmallDots = Parent.SmallDots;
                    BigDots = Parent.BigDots;

                    RotationSpeed = Parent.RotationSpeed;
                    BigDotsRadius = Parent.BigDotsRadius;
                    SmallDotsRadius = Parent.SmallDotsRadius;

                    AnimationSpeed = Parent.AnimationSpeed;
                    BigRadiusVariation = Parent.BigRadiusVariation;
                    IterationOffset = Parent.IterationOffset;
                    SinusMultiplier = Parent.SinusMultiplier;
                    CosinusMultiplier = Parent.CosinusMultiplier;

                    WaversSpeed = Parent.WaversSpeed;
                    BigDotsRadiusWaver = Parent.BigDotsRadiusWaver;
                    SmallDotsRadiusWaver = Parent.SmallDotsRadiusWaver;
                    BigRadiusVariationWaver = Parent.BigRadiusVariationWaver;
                    SinusMultiplierWaver = Parent.SinusMultiplierWaver;
                    CosinusMultiplierWaver = Parent.CosinusMultiplierWaver;

                    base.Update();
                }
            }
        }

        private void CheckIfShouldBeRemoved()
        {
            if (Parent)
            {
                if (Parent.livePreview != this)
                {
                    RemoveMe = true;
                }
            }

            if (RemoveMe)
            {
                if (Application.isPlaying)
                    Destroy(gameObject);
                else
                    DestroyImmediate(gameObject);
            }
        }


        public void ValidateTrigger()
        {
            bool difference = false;
            if (dotsBigImages == null || dotsSmallImages == null) return;

            if (BigDots != dotsBigImages.Count) difference = true;
            else
            if (SmallDots != dotsSmallImages.Count) difference = true;

            if (difference)
            {
                if (toRemoveEditor == null) toRemoveEditor = new List<GameObject>();

                for (int i = 0; i < generatedObjects.Count; i++)
                    toRemoveEditor.Add(generatedObjects[i]);
            }
            else
            {
                if ( dotsSmallImages != null)
                    for (int i = 0; i < dotsSmallImages.Count; i++) dotsSmallImages[i].rectTransform.anchoredPosition = new Vector2(0f, SmallDotsRadius);
            }
        }


        protected void OnDestroy()
        {
            if (generatedObjects == null) return;

            if (Application.isPlaying)
            {
                for (int i = 0; i < generatedObjects.Count; i++)
                    if (generatedObjects[i] != null)
                        GameObject.Destroy(generatedObjects[i]);
            }
            else
            {
                for (int i = 0; i < generatedObjects.Count; i++)
                    if (generatedObjects[i] != null)
                        GameObject.DestroyImmediate(generatedObjects[i]);
            }
        }
    }
}