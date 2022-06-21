using System;

namespace HelloPico2.TutorialSystem{
	public class TutorialBehaviorDetected{
		public Type DetectedType{ get; }
		public BehaviorTrackedData TrackedData{ get; }

		public TutorialBehaviorDetected(Type detectedType, BehaviorTrackedData trackedData){
			DetectedType = detectedType;
			TrackedData = trackedData;
		}
	}
}