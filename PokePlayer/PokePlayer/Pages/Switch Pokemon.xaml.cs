using PokePlayer_Library.Models.Pokemon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PokePlayer.Converters;
using PokePlayer_Library.Models;

/// <summary>
/// Page to switch your party pokemon with the rest of your pokemon
/// </summary>

namespace PokePlayer.Pages {
	public partial class Switch_Pokemon : Page {

		private int CarryPokemon;
		private Pokemon ListPokemon;

		public Switch_Pokemon(Trainer trainer) {
			InitializeComponent();
			// Id of the selected pokemon of the party
			this.CarryPokemon = 1;

			// Index of the selected pokemon from all the pokemon
			this.ListPokemon = trainer.PokemonList[0];

			// Creates a list of pokemon converters from all the party pokemon
			List<PokemonConverter> partyPokemonData = new List<PokemonConverter>();
			foreach (var pokemon in trainer.CarryPokemonList.Values) {
				partyPokemonData.Add(new PokemonConverter(pokemon));
			}

			// The selected pokemon from the party is the first of this list
			party_pokemon_select.Content = partyPokemonData[0];

			// Set the itemsource for the party pokemon to this list
			party_pokemon_list.ItemsSource = partyPokemonData;

			// Each pokemon of all pokemon must be converted to a pokemon converter
			// Except from the pokemons that are in the party pokemon because we would have double pokemons then
			List<PokemonConverter> allPokemonData = new List<PokemonConverter>();
			foreach (var pokemon in trainer.PokemonList) {
				bool carriesPokemon = false;
				foreach (var p in trainer.CarryPokemonList.Values) {
					if (p.Id == pokemon.Id) {
						carriesPokemon = true;
					}
				}

				if (!carriesPokemon) {
					allPokemonData.Add(new PokemonConverter(pokemon));
				}
			}

			// If the trainer has no more than 6 pokemon, the all pokemon view doesn't need to be displayed
			if (allPokemonData.Count == 0) {
				all_pokemon_select.Visibility = Visibility.Hidden;
			} else {
				all_pokemon_select.Visibility = Visibility.Visible;
				all_pokemon_select.Content = allPokemonData[0];
			}

			// The itemsource of all pokemons is set to the corresponding list
			all_pokemon_list.ItemsSource = allPokemonData;
		}

		// Sets the content of the selected party pokemon to the clicked party pokemon
		private void SelectPartyPokemon(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			PokemonConverter pokemonConverter = (PokemonConverter) button.Tag;
			Trainer trainer = PokePlayerApplication.MainApplication.Trainer;
			foreach (var key in trainer.CarryPokemonList.Keys) {
				if (trainer.CarryPokemonList[key].Id == pokemonConverter.Id) {
					this.CarryPokemon = key;
				}
			}
			party_pokemon_select.Content = pokemonConverter;
		}

		// Sets the content of the selected pokemon to the clicked pokemon
		private void SelectSwitchPokemon(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			PokemonConverter pokemonConverter = (PokemonConverter) button.Tag;
			this.ListPokemon = Pokemon.GetPokemon(pokemonConverter.Id);
			all_pokemon_select.Content = pokemonConverter;
		}

		// Switches the pokemon from the party and all pokemon list
		private void SwitchPokemon(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.Trainer.SwitchOutPokemon(this.CarryPokemon, this.ListPokemon);
			PokePlayerApplication.MainApplication.mainContent.Content = new Switch_Pokemon(PokePlayerApplication.MainApplication.Trainer);
		}
	}
}
