using PokePlayer_Library.Models.Pokemon;
using System;
using System.Collections.Generic;
using System.Text;

namespace PokePlayer.Converters {
	public class MoveConverter {
		public string MoveName { get; }
		public string Power { get; }
		public int MaxPp { get; }
		public int Pp { get; }
		public string TypeName { get; }
		public string Text { get; }
		public int PokemonMoveId { get; }

		public MoveConverter(Move move, int pokemonMoveId=-1) {
			this.MoveName = move.MoveName;
			this.PokemonMoveId = pokemonMoveId;
			if (move.Power == -1) {
				this.Power = "/";
			} else {
				this.Power = "" + move.Power;
			}

			this.MaxPp = move.MaxPp;
			this.Pp = move.MaxPp;
			this.TypeName = move.Type.TypeName;
			this.Text = move.FlavourText;
		}
	}
}
