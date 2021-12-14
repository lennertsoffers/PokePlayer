using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using PokePlayer_Library.Models.Pokemon;

namespace PokePlayer.Converters {
	public class PokemonConverter {
		public string NickName { get; }
		public string Sprite { get; }
		public string SpriteBack { get; }
		public string HpPercentage { get; }
		public string LostHpPercentage { get; }
		public string NonVolatileStatus { get; set; }
		public string IsEnabled { get; set; }
		public string ChooseText { get; set; }
		public string FlavourText { get; set; }
		public int Id { get; set; }
		public int Hp { get; }
		public int MaxHp { get; }
		public int Level { get; }
		public int ExperienceDifference { get; }
		public int TrainerPokemonId;
		public List<MoveConverter> Moves = new List<MoveConverter>();
		public List<Stat> Stats = new List<Stat>();

		public PokemonConverter(Pokemon pokemon, int trainerPokemonId=-1) {
			this.Id = pokemon.Id;
			this.NickName = pokemon.NickName;
			this.NickName = char.ToString(char.ToUpper(NickName[0])) + this.NickName[1..];
			this.Sprite = pokemon.SpriteFront;
			this.SpriteBack = pokemon.SpriteBack;
			this.TrainerPokemonId = trainerPokemonId;
			this.ChooseText = "Choose " + this.NickName;
			this.FlavourText = pokemon.Specie.FlavorText;
			double maxVal = (double) pokemon.GetStat("hp").StatValue;
			double curVal = (double) pokemon.Hp;
			double result = Math.Round(curVal / maxVal, 2);
			this.HpPercentage = result + "*";
			this.LostHpPercentage = 1 - result + "*";
			this.Hp = pokemon.Hp;
			this.MaxHp = pokemon.Stats["hp"].StatValue;
			this.Level = pokemon.Level;
			this.IsEnabled = this.Hp != 0 ? "True" : "False";

			foreach (var move in pokemon.Moves.Values) {
				this.Moves.Add(new MoveConverter(move));
			}

			foreach (var stat in pokemon.Stats.Values) {
				this.Stats.Add(stat);
			}

			this.NonVolatileStatus = "";
			foreach (var key in pokemon.NonVolatileStatus.Keys) {
				if (pokemon.NonVolatileStatus[key] > 0) {
					this.NonVolatileStatus = key;
				} else if (pokemon.NonVolatileStatus["SLP"] != -1) {
					this.NonVolatileStatus = "SLP";
				}
			}

			this.ExperienceDifference = pokemon.GetExperienceDifference();
		}
	}
}
