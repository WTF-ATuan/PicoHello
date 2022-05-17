using UnityEngine;

namespace Project{
	public static class ExtendVector3{
		public static bool IsGreaterOrEqual(this Vector3 local, Vector3 other){
			var xCond = local.x > other.x || Mathf.Approximately(local.x, other.x);
			var yCond = local.y > other.y || Mathf.Approximately(local.y, other.y);
			var zCond = local.z > other.z || Mathf.Approximately(local.z, other.z);

			return xCond && yCond && zCond;
		}

		public static bool IsLesserOrEqual(this Vector3 local, Vector3 other){
			var xCond = local.x < other.x || Mathf.Approximately(local.x, other.x);
			var yCond = local.y < other.y || Mathf.Approximately(local.y, other.y);
			var zCond = local.z < other.z || Mathf.Approximately(local.z, other.z);

			return xCond && yCond && zCond;
		}
	}
}