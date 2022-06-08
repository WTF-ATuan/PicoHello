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

		public float CurrentLengthLimit{ get; set; }
		public int MaxLengthLimit{ get; set; }
		public int MinLengthLimit{ get; set; }
		public float BlendWeight{ get; set; }
	}
}