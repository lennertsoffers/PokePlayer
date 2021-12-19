using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Pokeplayer_Library.DAL;
using PokePlayer_Library.Models.Pokemon;

// Class model for a trainer

namespace PokePlayer_Library.Models {
	public class Trainer {
		public int TrainerId { get; }
		public string Name { get; }
		public string Password { get; }
		public long RegenPokemon { get; set; }
		public List<Pokemon.Pokemon> PokemonList { get; set; }
		public Dictionary<int, Pokemon.Pokemon> CarryPokemonList { get; set; }

		// Corresponding model for database interaction
		private static TrainerRepository trainerRepository = new TrainerRepository();

		// No arguments constructor for use with Dapper
		public Trainer() {}

		// Constructor used for creation of new trainer
		public Trainer(string name, string password, Pokemon.Pokemon starter) {
			this.TrainerId = trainerRepository.GetAmountOfTrainers();
			this.Name = name;

			// Hash the password of the trainer
			this.Password = BCrypt.Net.BCrypt.HashPassword(password);

			// After exactly a day, pokemons must be at full health and stats again
			this.RegenPokemon = DateTimeOffset.Now.ToUnixTimeMilliseconds() + 86400000;
			this.PokemonList = new List<Pokemon.Pokemon>();
			this.CarryPokemonList = new Dictionary<int, Pokemon.Pokemon>();

			// Add the chosen starter pokemon
			AddPokemon(starter);
			Random r = new Random();
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));
			AddPokemon(new Pokemon.Pokemon(r.Next(1, 700), 30));

			// Insert the trainer in the database
			trainerRepository.InsertTrainer(this);
		}

		// Function to add new pokemons to the trainer
		public void AddPokemon(Pokemon.Pokemon pokemon) {
			// Pokemon is always added to the pokmonlist
			this.PokemonList.Add(pokemon);

			// If the trainer is carrying less than 6 pokemon, the pokemon is added to the party too
			if (this.CarryPokemonList.Count < 6) {
				this.CarryPokemonList.Add(this.CarryPokemonList.Count, pokemon);
			}

			// After adding a pokemon, the database must be updated
			trainerRepository.UpdateTrainer(this);
		}

		// Switch a pokemon from the party with pokemon from all trainer its pokemons
		public void SwitchOutPokemon(int carryPokemonId, Pokemon.Pokemon pokemon) {
			this.CarryPokemonList[carryPokemonId] = pokemon;

			// After switching pokemons, the database must be updated
			trainerRepository.UpdateTrainer(this);
		}

		// Switch two party pokemon, only used in battle
		public void SwitchPokemon(int p1Index, int p2Index) {
			if (this.CarryPokemonList[p2Index].Hp > 0) {
				Pokemon.Pokemon tempPokemon = this.CarryPokemonList[p1Index];
				this.CarryPokemonList[p1Index] = this.CarryPokemonList[p2Index];
				this.CarryPokemonList[p2Index] = tempPokemon;

				// After switching pokemons, the database must be updated
				trainerRepository.UpdateTrainer(this);
			}
		}

		// Used to check if trainer has pokemons in its party that aren't fainted
		public bool HasPokemonsLeft() {
			foreach (var pokemon in CarryPokemonList.Values) {
				if (pokemon.Hp > 0) {
					return true;
				}
			}
			return false;
		}

		// Get a trainer by name
		public static Trainer GetTrainer(string name) {
			return trainerRepository.GetTrainer(name);
		}

		// Check if trainer exists
		public static bool TrainerExists(string name) {
			return trainerRepository.TrainerExists(name);
		}
	}
}
