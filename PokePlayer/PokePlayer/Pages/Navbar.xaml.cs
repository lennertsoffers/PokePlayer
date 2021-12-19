using System;
using System.Collections.Generic;
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

/// <summary>
/// Navigation bar for the application
/// </summary> 

namespace PokePlayer.Pages {
	public partial class Navbar : Page {
		public Navbar() {
			InitializeComponent();
		}

		// Loads the viewparty content
		public void viewParty(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.mainContent.Content =
				new View_Party(PokePlayerApplication.MainApplication.Trainer);
		}

		// Loads the switchpokemon content
		public void switchPokemon(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.mainContent.Content =
				new Switch_Pokemon(PokePlayerApplication.MainApplication.Trainer);
		}

		// Loads the wildbattle content
		public void wildBattle(object sender, RoutedEventArgs e) {
			Battle battle = new Battle(PokePlayerApplication.MainApplication.Trainer);
			PokePlayerApplication.MainApplication.fullScreen.Content = battle;
			battle.StartBattle();
		}

		// Loads pre login screen
		public void Logout(object sender, RoutedEventArgs e) {
			PokePlayerApplication.MainApplication.fullScreen.Content = new PreLogin();
		}
	}
}
