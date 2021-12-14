using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pokeplayer_Library.DAL;

namespace PokePlayer_Library.Models {
	public class Trainer {
		public int TrainerId { get; }
		public string Name { get; }
		public string Password { get; }
		public List<Pokemon.Pokemon> PokemonList { get; set; }
		public Dictionary<int, Pokemon.Pokemon> CarryPokemonList { get; set; }

		private static TrainerRepository trainerRepository = new TrainerRepository();

		public Trainer() {}

		public Trainer(string name, string password, Pokemon.Pokemon starter) {
			this.TrainerId = trainerRepository.GetAmountOfTrainers();
			this.Name = name;
			this.Password = BCrypt.Net.BCrypt.HashPassword(password);
			this.PokemonList = new List<Pokemon.Pokemon>();
			this.CarryPokemonList = new Dictionary<int, Pokemon.Pokemon>();
			AddPokemon(starter);
			
			trainerRepository.InsertTrainer(this);
		}

		public void AddPokemon(Pokemon.Pokemon pokemon) {
			this.PokemonList.Add(pokemon);
			if (this.CarryPokemonList.Count < 6) {
				this.CarryPokemonList.Add(this.CarryPokemonList.Count, pokemon);
			}

			trainerRepository.UpdateTrainer(this);
		}

		public void MovePokemon(int carryPokemonIndex, Pokemon.Pokemon pokemon) {
			this.CarryPokemonList[carryPokemonIndex] = pokemon;
			
			trainerRepository.UpdateTrainer(this);
		}

		public bool SwitchPokemon(int p1Index, int p2Index) {
			if (this.CarryPokemonList[p2Index].Hp > 0) {
				Pokemon.Pokemon tempPokemon = this.CarryPokemonList[p1Index];
				this.CarryPokemonList[p1Index] = this.CarryPokemonList[p2Index];
				this.CarryPokemonList[p2Index] = tempPokemon;

				trainerRepository.UpdateTrainer(this);
				return true;
			}

			return false;
		}

		public bool HasPokemonsLeft() {
			foreach (var pokemon in CarryPokemonList.Values) {
				if (pokemon.Hp > 0) {
					return true;
				}
			}
			return false;
		}

		public static Trainer GetTrainer(string name) {
			return trainerRepository.GetTrainer(name);
		}

		public static bool TrainerExists(string name) {
			return trainerRepository.TrainerExists(name);
		}
	}
}
