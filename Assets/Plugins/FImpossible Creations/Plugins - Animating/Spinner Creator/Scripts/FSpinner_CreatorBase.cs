using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace FIMSpace.FSpinner
{
    /// <summary>
    /// FM: Class to animate smooth spinner for loading etc.
    /// Use parameters to create unique animations, there are millions of possibilibies!
    /// </summary>
    public abstract class FSpinner_CreatorBase : MonoBehaviour
    {
        [Header("Base Parameters")]
        public GameObject BigDotPrefab;
        public GameObject SmallDotPrefab;

        [Tooltip("Big dots count which each is animated separately")]
        [Range(0, 24)]
        public int BigDots = 12;

        [Tooltip("Small dots count which are only rotated in one transform")]
        [Range(0, 24)]
        public int SmallDots = 8;

        [Header("Placement Parameters")]
        public float RotationSpeed = 50f;
        public float BigDotsRadius = 50.0f;
        public float SmallDotsRadius = 10.0f;

        [Header("Animation Parameters")]
        public float AnimationSpeed = 1f;
        public float BigRadiusVariation = 10f;
        public float IterationOffset = 1f;
        public float SinusMultiplier = 1f;
        public float CosinusMultiplier = 1f;

        [Header("Additional Parameters")]
        public float WaversSpeed = 1f;

        public float BigDotsRadiusWaver = 0f;
        protected FSpinner_CreatorWaver bigDotsWaver;

        public float SmallDotsRadiusWaver = 0f;
        protected FSpinner_CreatorWaver smallDotsWaver;

        public float BigRadiusVariationWaver = 0f;
        protected FSpinner_CreatorWaver bigRadiusWaver;

        public float SinusMultiplierWaver = 0f;
        protected FSpinner_CreatorWaver sinusWaver;

        public float CosinusMultiplierWaver = 0f;
        protected FSpinner_CreatorWaver cosinusWaver;

        protected float time = 0f;

        protected RectTransform rect;
        protected RectTransform bigDotsContainer;
        protected RectTransform smallDotsContainer;

        protected List<Image> dotsBigImages;
        protected List<Image> dotsSmallImages;
        protected Image dotsCenter;

        /// <summary> Re-creating animated images, wanted it to be in OnValidate but it have troubles with instantiation here </summary>
        protected bool difference = false;

        protected virtual void Start()
        {
            rect = GetComponent<RectTransform>();

            if (BigDotPrefab)
            {
                bigDotsContainer = new GameObject(name + "-BigDots").AddComponent<RectTransform>();
                bigDotsContainer.SetParent(transform, false);
                bigDotsContainer.sizeDelta = rect.sizeDelta;
                bigDotsContainer.anchoredPosition = Vector3.zero;
            }

            if (SmallDotPrefab)
            {
                smallDotsContainer = new GameObject(name + "-SmallDots").AddComponent<RectTransform>();
                smallDotsContainer.SetParent(transform, false);
                smallDotsContainer.sizeDelta = rect.sizeDelta;
                smallDotsContainer.anchoredPosition = Vector3.zero;
            }

            dotsBigImages = new List<Image>();
            dotsSmallImages = new List<Image>();

            Transform cent = transform.Find("Spinner Center");
            if (cent) dotsCenter = cent.GetComponent<Image>();

            smallDotsWaver = new FSpinner_CreatorWaver();
            bigDotsWaver = new FSpinner_CreatorWaver();
            bigRadiusWaver = new FSpinner_CreatorWaver();
            sinusWaver = new FSpinner_CreatorWaver();
            cosinusWaver = new FSpinner_CreatorWaver();

            AssignObjects();
        }

        /// <summary>
        /// Creating images for spinner based on component's parameters
        /// </summary>
        public virtual void AssignObjects()
        {
            // Clear previous objects
            for (int i = dotsBigImages.Count - 1; i >= 0; i--) Destroy(dotsBigImages[i].gameObject);
            dotsBigImages.Clear();

            for (int i = dotsSmallImages.Count - 1; i >= 0; i--) Destroy(dotsSmallImages[i].gameObject);
            dotsSmallImages.Clear();

            if (BigDotPrefab)
            {
                // Creating dot objects in spheric manner, each of them will be animated separately
                if (BigDots > 0)
                {
                    // Creating each dot in equal distance to each other and form them onto sphere
                    float step = 360 / BigDots;

                    for (int i = 0; i < BigDots; i++)
                    {
                        GameObject o = Instantiate(BigDotPrefab, bigDotsContainer);
                        o.transform.rotation = Quaternion.Euler(0f, 0f, i * step);

                        // Assigning right main parent transform
                        RectTransform rect = o.GetComponent<RectTransform>();
                        rect.anchoredPosition = Vector3.zero;

                        dotsBigImages.Add(o.transform.GetChild(0).GetComponent<Image>());
                        if (dotsCenter) o.GetComponentInChildren<Image>().color = dotsCenter.color;
                    }
                }
            }

            if (SmallDotPrefab)
            {
                if (SmallDots > 0)
                {
                    float step = 360 / SmallDots;

                    for (int i = 0; i < SmallDots; i++)
                    {
                        GameObject o = Instantiate(SmallDotPrefab, smallDotsContainer);
                        o.transform.rotation = Quaternion.Euler(0f, 0f, i * step);

                        RectTransform rect = o.GetComponent<RectTransform>();
                        rect.anchoredPosition = Vector3.zero;

                        dotsSmallImages.Add(o.transform.GetChild(0).GetComponent<Image>());
                        if (dotsCenter) o.GetComponentInChildren<Image>().color = dotsCenter.color;
                    }
                }

                for (int i = 0; i < dotsSmallImages.Count; i++) dotsSmallImages[i].rectTransform.anchoredPosition = new Vector2(0f, SmallDotsRadius);
            }

            time = Random.Range(-Mathf.PI, Mathf.PI);
        }

        /// <summary>
        /// When we want change objects count in realtime we need to create or destroy them and assign again
        /// </summary>
        protected virtual void OnValidate()
        {
            // Checking difference in objects' count to execute this only when needed
            difference = false;

            if (dotsBigImages == null || dotsSmallImages == null) return;

            if (BigDots != dotsBigImages.Count) difference = true;
            else
            if (SmallDots != dotsSmallImages.Count) difference = true;

            for (int i = 0; i < dotsSmallImages.Count; i++) dotsSmallImages[i].rectTransform.anchoredPosition = new Vector2(0f, SmallDotsRadius);
        }

        /// <summary>
        /// Using trigonometric functions with modificators from component to create unique and cool looking animation
        /// </summary>
        protected virtual void Update()
        {
            time += Time.unscaledDeltaTime * AnimationSpeed;
            if (!Application.isPlaying) if (Time.unscaledDeltaTime == 0f) time += 0.015625f * AnimationSpeed;

            // Additional options calculations if used
            float bigD = 0f, smallD = 0f, bigRadD = 0f, sinD = 0f, cosD = 0f;

            if (BigDotsRadiusWaver != 0f)
            {
                bigDotsWaver.timeSpeed = WaversSpeed;
                bigD = bigDotsWaver.GetValue() * BigDotsRadiusWaver;
            }

            if (SmallDotsRadiusWaver != 0f)
            {
                smallDotsWaver.timeSpeed = WaversSpeed;
                smallD = smallDotsWaver.GetValue() * SmallDotsRadiusWaver;
            }

            if (BigRadiusVariationWaver != 0f)
            {
                bigDotsWaver.timeSpeed = WaversSpeed;
                bigRadD = bigRadiusWaver.GetValue() * BigRadiusVariationWaver;
            }

            if (SinusMultiplierWaver != 0f)
            {
                sinusWaver.timeSpeed = WaversSpeed;
                sinD = sinusWaver.GetValue() * SinusMultiplierWaver;
            }

            if (CosinusMultiplierWaver != 0f)
            {
                cosinusWaver.timeSpeed = WaversSpeed;
                cosD = cosinusWaver.GetValue() * CosinusMultiplierWaver;
            }

            // Rotating all dots in one transform
            if (bigDotsContainer) bigDotsContainer.Rotate(0f, 0f, Time.unscaledDeltaTime * RotationSpeed);

            if (smallDotsContainer)
            {
                smallDotsContainer.Rotate(0f, 0f, Time.unscaledDeltaTime * -RotationSpeed * 1.5f);
                if (SmallDotsRadiusWaver != 0f) for (int i = 0; i < dotsSmallImages.Count; i++) dotsSmallImages[i].rectTransform.anchoredPosition = new Vector2(0f, SmallDotsRadius + smallD);
            }

            // Animating each dot separately for special animation
            for (int i = 0; i < dotsBigImages.Count; i++)
            {
                dotsBigImages[i].rectTransform.anchoredPosition =

                    new Vector2(
                        (

                        Mathf.Cos(time * 5.5f + (float)i * IterationOffset)) * (CosinusMultiplier + cosD)

                        ,

                        ((BigDotsRadius + bigD) + (Mathf.Sin(time * 5.5f + (float)i * IterationOffset) * (BigRadiusVariation + bigRadD))) * (SinusMultiplier + sinD)

                        );
            }

            if (difference)
            {
                AssignObjects();
                difference = false;
            }
        }

        /// <summary>
        /// Small helper class to manage animating additional variables
        /// </summary>
        protected class FSpinner_CreatorWaver
        {
            public float time;
            public float timeSpeed = 1f;

            public FSpinner_CreatorWaver()
            {
                time = Random.Range(-10f, 10f);
            }

            public float GetValue()
            {
#if UNITY_EDITOR
                if (!Application.isPlaying) if (Time.unscaledDeltaTime == 0f) time += 0.015625f * timeSpeed;
#endif
                time += Time.unscaledDeltaTime * timeSpeed;
                return Mathf.Sin(time);
            }
        }

    }
}