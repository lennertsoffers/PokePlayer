﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.ServiceModel.Security;
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
using PokePlayer_Library.Models.Pokemon;

namespace PokePlayer.Pages {
	/// <summary>
	/// Interaction logic for ChooseStarter.xaml
	/// </summary>
	public partial class ChooseStarter : Page {

		private string Username;
		private string Password;

		public ChooseStarter(string username, string password) {
			InitializeComponent();
			this.Username = username;
			this.Password = password;
			bulbasaur.Content = new PokemonConverter(new Pokemon(1, 5));
			charmander.Content = new PokemonConverter(new Pokemon(4, 5));
			squirtle.Content = new PokemonConverter(new Pokemon(7, 5));
		}

		public void SelectStarter(object sender, RoutedEventArgs e) {
			Pokemon pokemon = Pokemon.GetPokemon(((PokemonConverter) ((Button) sender).Tag).Id);
			Trainer trainer = new Trainer(this.Username, this.Password, pokemon);

			PokePlayerApplication.MainApplication.navBar.Content = new Navbar();
			PokePlayerApplication.MainApplication.mainContent.Content = new View_Party(trainer);
			PokePlayerApplication.MainApplication.fullScreen.Content = new ClearContent();
			PokePlayerApplication.MainApplication.Trainer = trainer;
		}
	}
}
