using System;
using System.Collections.Generic;
using System.Text;

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
