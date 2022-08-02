namespace HelloPico2.Rating_System{
	public class BehaviorRating{
		private readonly AngleCalculator _calculator;

		private readonly RatingSetting _setting;

		public BehaviorRating(AngleCalculator calculator, RatingSetting setting){
			_calculator = calculator;
			_setting = setting;
		}

		public float GetOffsetRating(){
			var offset = _calculator.GetOffsetOfTarget();
			var offsetTier = CompareOffsetTier(offset);
			return offsetTier;
		}

		public float GetCompositeRating(){
			var angle = _calculator.GetAngleOfTarget();
			var offset = _calculator.GetOffsetOfTarget();
			var angleTier = CompareAngleTier(angle);
			var offsetTier = CompareOffsetTier(offset);
			var tier = (angleTier + offsetTier) / 2.0f;
			return tier;
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
				var minValue = currentRange.minValue;
				var maxValue = currentRange.maxValue;
				if(angle > minValue && angle <= maxValue) return currentRange.ratingPoint;
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
				var minValue = currentRange.minValue;
				var maxValue = currentRange.maxValue;
				if(offset > minValue && offset <= maxValue) return currentRange.ratingPoint;
			}

			return 0;
		}
	}
}