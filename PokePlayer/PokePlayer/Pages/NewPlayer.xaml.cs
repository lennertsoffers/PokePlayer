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

namespace PokePlayer.Pages {
	/// <summary>
	/// Interaction logic for NewPlayer.xaml
	/// </summary>
	public partial class NewPlayer : Page {
		public NewPlayer() {
			InitializeComponent();
		}

		public void ValidateUsername(object sender, RoutedEventArgs e) {
			if (!Trainer.TrainerExists(username.Text)) {
				PokePlayerApplication.MainApplication.fullScreen.Content = new ChooseStarter(username.Text, password.Password);
			} else {
				userExists.Visibility = Visibility.Visible;
				username.Text = "";
				password.Password = "";
			}
		}
	}
}
