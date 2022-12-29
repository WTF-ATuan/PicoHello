using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using Sirenix.OdinInspector;

namespace HelloPico2.InteractableObjects
{
    public class SpiritTimeline : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Playables.PlayableDirector[] _PlayableDirectors;
        Coroutine process;
        public float ActivateTimeline(string timelineName)
        {
            var playable = GetTimeline(timelineName);

            if (playable)
            {
                if (process != null)
                {
                    StopCoroutine(process);
                    playable.gameObject.SetActive(false);
                }

                process = StartCoroutine(TrackTimeline(playable));
                Debug.Log(playable.name);
                playable.gameObject.SetActive(true);
                //playable.Play();

                return (float)playable.duration;
            }
            else
            {
                throw new System.Exception("Coundn't find timeline " + timelineName + " from SpiritTimeline.");
                return 0;
            }

        }
        private UnityEngine.Playables.PlayableDirector GetTimeline(string timelineName) {
            for (int i = 0; i < _PlayableDirectors.Length; i++)
            {
                if (_PlayableDirectors[i].name == timelineName)
                {
                    return _PlayableDirectors[i]; 
                }
            }
            return null;
        }
        private IEnumerator TrackTimeline(UnityEngine.Playables.PlayableDirector timeline) {
            Debug.Log(timeline.duration);
            Debug.Log(timeline.time);
            yield return new WaitUntil(() => (timeline.time - timeline.duration) >= -0.1f);
            timeline.Stop();
            timeline.gameObject.SetActive(false);
        }
    }
}
