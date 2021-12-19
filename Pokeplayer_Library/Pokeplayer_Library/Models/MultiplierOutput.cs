using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Class model for the output of an multieplier output
/// Used to structure the message of the multiplier
/// </summary>

namespace PokePlayer_Library.Models {
	public class MultiplierOutput {
		public double Multiplier { get; }
		public string Message { get; }
		public bool Crit { get; }

		public MultiplierOutput(double multiplier, string message, bool crit) {
			this.Multiplier = multiplier;
			this.Message = message;
			this.Crit = crit;
		}
	}
}
