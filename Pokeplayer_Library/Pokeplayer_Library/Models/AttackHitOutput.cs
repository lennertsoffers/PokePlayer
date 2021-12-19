using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Class model for the output of an attack hit
/// Used to structure the message of the output of an attack
/// </summary>

namespace PokePlayer_Library.Models {
	public class AttackHitOutput {
		public bool Attack { get; }
		public string Message { get; }
		public bool Before { get; }

		public AttackHitOutput(bool attack, string message, bool before) {
			this.Attack = attack;
			this.Message = message;
			this.Before = before;
		}
	}
}
