namespace HelloPico2.InteractableObjects{
	public class LightBeamLenghtUpdated{
		public float SingleLenght;
		public float TotalLenght;

		/// <summary>
		///     0 => Before Update
		///     1 => Updating
		///     2 => After Update
		/// </summary>
		public int UpdateState;
	}
}