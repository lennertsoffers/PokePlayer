using Caliburn.Micro;
using PokePlayer.Converters;
using PokePlayer_Library.Models.Pokemon;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
using PokePlayer_Library.Models;
using Color = System.Drawing.Color;

/// <summary>
/// Page to view your party
/// </summary>

namespace PokePlayer.Pages {
	public partial class View_Party : Page {
		public View_Party(Trainer trainer) {
			InitializeComponent();

			// Creates a list of pokemonconverter from the carry pokemons
			List<PokemonConverter> pokemonData = new List<PokemonConverter>();
			foreach (var pokemon in trainer.CarryPokemonList.Values) {
				pokemonData.Add(new PokemonConverter(pokemon));
			}

			// This list is the datasource for the party
			party.ItemsSource = pokemonData;

			// Show the data of the first pokemon in the content of the specific pokemon
			SpecificPokemon.Content = pokemonData[0];
			Moves.ItemsSource = pokemonData[0].Moves;
			Stats.ItemsSource = pokemonData[0].Stats;
		}

		// Show the moves of the selected pokemon
		private void move_Click(object sender, RoutedEventArgs e) {
			var bc = new BrushConverter();
			Move_button.Background = (Brush) bc.ConvertFrom("#6a994e");
			Stat_button.Background = (Brush) bc.ConvertFrom("#386641");
			Moves.Visibility = Visibility.Visible;
			Stats.Visibility = Visibility.Hidden;
		}

		// Show the stats of the selected pokemon
		private void stats_Click(object sender, RoutedEventArgs e) {
			var bc = new BrushConverter();
			Stat_button.Background = (Brush) bc.ConvertFrom("#6a994e");
			Move_button.Background = (Brush) bc.ConvertFrom("#386641");
			Stats.Visibility = Visibility.Visible;
			Moves.Visibility = Visibility.Hidden;
		}

		// Select another pokemon
		private void pokemon_Click(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			PokemonConverter pokemonData = (PokemonConverter) button.Tag;
			SpecificPokemon.Content = pokemonData;
			Moves.ItemsSource = pokemonData.Moves;
			Stats.ItemsSource = pokemonData.Stats;
		}
	}
}
