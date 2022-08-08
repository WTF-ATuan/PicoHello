using System.Diagnostics.CodeAnalysis;

namespace HelloPico2.Data_Structure{
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="F"> Finder Type </typeparam>
	/// <typeparam name="E"> Found Entity Type</typeparam>
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public abstract class AbstractFinder<F, E> where F : new(){
		public static F NewInstance(){
			return new F();
		}

		public abstract E Find();
	}
}