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

namespace PokePlayer.Pages {
	public partial class Switch_Pokemon : Page {
		public Switch_Pokemon(Trainer trainer) {
			InitializeComponent();

			List<PokemonConverter> partyPokemonData = new List<PokemonConverter>();
			foreach (var pokemon in trainer.CarryPokemonList.Values) {
				partyPokemonData.Add(new PokemonConverter(pokemon));
			}

			party_pokemon_select.Content = partyPokemonData[0];
			party_pokemon_list.ItemsSource = partyPokemonData;

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

			if (allPokemonData.Count == 0) {
				all_pokemon_select.Visibility = Visibility.Hidden;
			} else {
				all_pokemon_select.Visibility = Visibility.Visible;
				all_pokemon_select.Content = allPokemonData[0];
			}
			all_pokemon_list.ItemsSource = allPokemonData;
		}

		private void SelectPartyPokemon(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			party_pokemon_select.Content = (PokemonConverter) button.Tag;
		}

		private void SelectSwitchPokemon(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			all_pokemon_select.Content = (PokemonConverter) button.Tag;
		}
	}
}
