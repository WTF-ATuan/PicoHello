namespace HelloPico2.Rating_System{
	public class BehaviorRating{
		private readonly AngleDetector _detector;

		private readonly RatingSetting _setting;

		public BehaviorRating(AngleDetector detector, RatingSetting setting){
			_detector = detector;
			_setting = setting;
		}


		public void Rate(){
			var angle = _detector.GetAngleOfTarget();
			var offset = _detector.GetOffsetOfTarget();
			var angleTier = CompareAngleTier(angle);
			var offsetTier = CompareOffsetTier(offset);
			var tier = (angleTier + offsetTier) / 2;
		}

		/// <summary>
		///     if you get 0 that is not in any Tier
		///     the Tier is between 0 to max
		/// </summary>
		/// <param name="angle"></param>
		/// <returns></returns>
		private int CompareAngleTier(float angle){
			var angleRange = _setting.GetAngleRange();
			for(var i = 0; i < angleRange.Count; i++){
				var currentRange = angleRange[i];
				var minValue = currentRange.x;
				var maxValue = currentRange.y;
				if(angle >= minValue && angle <= maxValue) return i + 1;
			}

			return 0;
		}

		/// <summary>
		///     if you get 0 that is not in any Tier
		///     the Tier is between 0 to max
		/// </summary>
		/// <param name="offset"></param>
		/// <returns></returns>
		private int CompareOffsetTier(float offset){
			var offsetRange = _setting.GetOffsetRange();
			for(var i = 0; i < offsetRange.Count; i++){
				var currentRange = offsetRange[i];
				var minValue = currentRange.x;
				var maxValue = currentRange.y;
				if(offset >= minValue && offset <= maxValue) return i + 1;
			}

			return 0;
		}
	}
}