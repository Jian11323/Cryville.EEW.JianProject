using System;

namespace Cryville.EEW.JianProject {
	public enum JianKind {
		EarlyWarning,
		Bulletin,
		Tsunami,
		Weather,
		Other,
	}

	/// <summary>
	/// One event taken from a Jian Project API frame. Absent upstream fields stay null.
	/// </summary>
	public sealed class JianEvent {
		public JianKind Kind { get; init; }
		public string Agency { get; init; } = "";
		public string UnitId { get; init; } = "";
		public int? Serial { get; init; }
		public bool IsCancel { get; init; }
		public bool IsFinal { get; init; }
		public bool IsTraining { get; init; }
		public bool IsAssumption { get; init; }
		public string? Place { get; init; }
		public string? PlaceZh { get; init; }
		public string? ForwardAgency { get; init; }
		public string? Title { get; init; }
		public string? InfoType { get; init; }
		public double? Magnitude { get; init; }
		public double? DepthKm { get; init; }
		public double? Latitude { get; init; }
		public double? Longitude { get; init; }
		public DateTime? OriginUtc { get; init; }
		public string? Intensity { get; init; }
		public string? Detail { get; init; }
		public string Fingerprint { get; init; } = "";
	}
}
