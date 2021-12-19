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
using PokePlayer_Library.Models;

/// <summary>
/// Page to create a new player
/// </summary>

namespace PokePlayer.Pages {
	public partial class NewPlayer : Page {
		public NewPlayer() {
			InitializeComponent();
		}

		// Checks if the username doen't exist already
		public void ValidateUsername(object sender, RoutedEventArgs e) {
			// If the username doesn't exist, the choose starter screen is loaded
			if (!Trainer.TrainerExists(username.Text)) {
				PokePlayerApplication.MainApplication.fullScreen.Content = new ChooseStarter(username.Text, password.Password);
			} else {
				// Otherwise the error message becomes visible
				userExists.Visibility = Visibility.Visible;
				username.Text = "";
				password.Password = "";
			}
		}
	}
}
