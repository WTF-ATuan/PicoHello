using UnityEngine;

namespace HelloPico2.InteractableObjects{
	public class LightBeamLengthUpdated{
		public float SingleLength;
		public float TotalLength;

		public Vector3 TotalOffset;

		/// <summary>
		///     0 => Before Update
		///     1 => Updating
		///     2 => After Update
		/// </summary>
		public int UpdateState;
	}
}