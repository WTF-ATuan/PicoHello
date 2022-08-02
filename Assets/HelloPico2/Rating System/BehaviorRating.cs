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
		private float CompareOffsetTier(float offset){
			if(offset < 0) return 0;

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