using PokePlayer.Converters;
using PokePlayer_Library.Models;
using PokePlayer_Library.Models.Pokemon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Type = PokePlayer_Library.Models.Pokemon.Type;

namespace PokePlayer.Pages {
	public partial class Battle : Page {

		public Trainer Trainer { get; set; }
		public Pokemon FirstPokemon { get; set; }
		public Pokemon WildPokemon { get; set; }
		private Pokemon Attacker { get; set; }
		private Pokemon Target { get; set; }
		private int MoveId { get; set; }
		private bool TrainerWin;
		private readonly Random random = new Random();
		private int premovehits = 0;
		private bool BattleEnds = false;
		private bool ObligatedSwitch = false;
		private TaskCompletionSource<int> tcs;


		public Battle(Trainer trainer) {
			InitializeComponent();
			this.Trainer = trainer;
		}



		// ---- START BATTLE ---- //
		async public void StartBattle() {
			PokePlayerApplication.MainApplication.navBar.Visibility = Visibility.Hidden;
			this.FirstPokemon = Trainer.CarryPokemonList[0];
			
			this.WildPokemon = new Pokemon(random.Next(1, 700), random.Next(FirstPokemon.Level - 5, FirstPokemon.Level + 6));
			// this.WildPokemon = new Pokemon(1, 1);

			wildPokemon.Content = new PokemonConverter(this.WildPokemon);
			trainerPokemon.Content = new PokemonConverter(this.FirstPokemon);

			await SetOutputAsync("A wild " + WildPokemon.Specie.SpecieName + " appeared!");

			ShowBattleMenu();
		}



		// ---- BATTLE MENU ---- //
		// Show menu
		private void ShowBattleMenu() {
			if (!this.BattleEnds) {
				BattleMenu.Visibility = Visibility.Visible;
				SetOutputSync("What will you do?");
			} else {
				BattleMenu.Visibility = Visibility.Hidden;
				this.BattleEnds = false;
			}
		}

		// Fight action
		private void ActionFight(object sender, RoutedEventArgs e) {
			ShowMoveMenu();
		}

		// Throw pokeball action
		private void ActionPokeball(object sender, RoutedEventArgs e) {
			BattleMenu.Visibility = Visibility.Hidden;
			CaptureAttempt();
		}

		// Switch action
		private void ActionSwitch(object sender=null, RoutedEventArgs e=null) {
			List<PokemonConverter> trainerPokemonData = new List<PokemonConverter>();
			foreach (var key in Trainer.CarryPokemonList.Keys) {
				PokemonConverter pokemonConverter = new PokemonConverter(Trainer.CarryPokemonList[key], key);
				if (key == 0) {
					pokemonConverter.IsEnabled = "false";
				}
				trainerPokemonData.Add(pokemonConverter);
			}

			BattleMenu.Visibility = Visibility.Hidden;
			switchPokemons.ItemsSource = trainerPokemonData;
			switchPokemon.Visibility = Visibility.Visible;
			if (sender == null && e == null) {
				this.ObligatedSwitch = true;
			}
		}

		// Run action
		async private void ActionRun(object sender, RoutedEventArgs e) {
			if (FirstPokemon.GetStat("speed").StatValue >= WildPokemon.GetStat("speed").StatValue) {
				await SetOutputAsync("Got away safely");
				PokePlayerApplication.MainApplication.navBar.Visibility = Visibility.Visible;
				PokePlayerApplication.MainApplication.mainContent.Content = new View_Party(Trainer);
			} else {
				await SetOutputAsync("Can't escape");
			}
		}

		// Click on pokemon to switch with
		async private void SwitchPokemon(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			int switchPokemonId = ((PokemonConverter) button.Tag).TrainerPokemonId;
			switchPokemon.Visibility = Visibility.Hidden;

			await SetOutputAsync("Come back " + this.FirstPokemon.NickName + "!");
			this.FirstPokemon = Trainer.CarryPokemonList[switchPokemonId];
			Trainer.SwitchPokemon(0, switchPokemonId);
			await SetOutputAsync(Trainer.Name + " switched to " + this.FirstPokemon.NickName);
			trainerPokemon.Content = new PokemonConverter(this.FirstPokemon);
			if (!this.ObligatedSwitch) {
				await Turn(random.Next(1, WildPokemon.Moves.Count), WildPokemon, FirstPokemon);
			}

			this.ObligatedSwitch = false;
			ShowBattleMenu();
		}



		// ---- MOVE MENU ---- //
		private void ShowMoveMenu() {
			BattleMenu.Visibility = Visibility.Hidden;
			MoveMenu.Visibility = Visibility.Visible;
			SetOutputSync("Choose a move");
			if (FirstPokemon.Moves.ContainsKey(1)) {
				Move1Button.Visibility = Visibility.Visible;
				MoveConverter moveData = new MoveConverter(FirstPokemon.Moves[1]);
				Move1ButtonName.Text = moveData.MoveName;
				Move1ButtonPp.Text = FirstPokemon.MovePpMapping[1] + " / " + moveData.MaxPp;
				Move1Button.IsEnabled = FirstPokemon.MovePpMapping[1] > 0;
			} else {
				Move1Button.Visibility = Visibility.Hidden;
			}
			if (FirstPokemon.Moves.ContainsKey(2)) {
				Move2Button.Visibility = Visibility.Visible;
				MoveConverter moveData = new MoveConverter(FirstPokemon.Moves[2]);
				Move2ButtonName.Text = moveData.MoveName;
				Move2ButtonPp.Text = FirstPokemon.MovePpMapping[2] + " / " + moveData.MaxPp;
				Move2Button.IsEnabled = FirstPokemon.MovePpMapping[2] > 0;
			} else {
				Move2Button.Visibility = Visibility.Hidden;
			}
			if (FirstPokemon.Moves.ContainsKey(3)) {
				Move3Button.Visibility = Visibility.Visible;
				MoveConverter moveData = new MoveConverter(FirstPokemon.Moves[3]);
				Move3ButtonName.Text = moveData.MoveName;
				Move3ButtonPp.Text = FirstPokemon.MovePpMapping[3] + " / " + moveData.MaxPp;
				Move3Button.IsEnabled = FirstPokemon.MovePpMapping[3] > 0;
			} else {
				Move3Button.Visibility = Visibility.Hidden;
			}
			if (FirstPokemon.Moves.ContainsKey(4)) {
				Move4Button.Visibility = Visibility.Visible;
				MoveConverter moveData = new MoveConverter(FirstPokemon.Moves[4]);
				Move4ButtonName.Text = moveData.MoveName;
				Move4ButtonPp.Text = FirstPokemon.MovePpMapping[4] + " / " + moveData.MaxPp;
				Move4Button.IsEnabled = FirstPokemon.MovePpMapping[4] > 0;
			} else {
				Move4Button.Visibility = Visibility.Hidden;
			}
		}

		async private void ActionMove(object sender, RoutedEventArgs e) {
			Button button = (Button) sender;
			MoveMenu.Visibility = Visibility.Hidden;
			if (FirstPokemon.GetStat("speed").StatValue >= WildPokemon.GetStat("speed").StatValue) {
				await Turn(int.Parse((string) button.Tag), FirstPokemon, WildPokemon);
				if (!this.BattleEnds) {
					await Turn(random.Next(1, WildPokemon.Moves.Count), WildPokemon, FirstPokemon);
				}
			} else {
				await Turn(random.Next(1, 5), WildPokemon, FirstPokemon);
				if (!this.BattleEnds) {
					await Turn(int.Parse((string) button.Tag), FirstPokemon, WildPokemon);
				}
			}
			ShowBattleMenu();
		}



		// ---- LEARN MOVE MENU ---- //
		private void TeachMove(Pokemon pokemon, Move move) {
			teachMoveContent.Content = new MoveConverter(move);
			teachMove.Visibility = Visibility.Visible;
			teachMoveMenu.Visibility = Visibility.Visible;
		}

		private void TeachMoveContinueButton(object sender, RoutedEventArgs e) {
			HideTeachMoveButtons();
			this.tcs.SetResult(this.FirstPokemon.Moves.Count);
		}

		private void TeachMoveForgetButton(object sender, RoutedEventArgs e) {
			HideTeachMoveButtons();
			List<MoveConverter> pokemonMoveData = new List<MoveConverter>();
			foreach (var id in FirstPokemon.Moves.Keys) {
				pokemonMoveData.Add(new MoveConverter(FirstPokemon.Moves[id], id));
			}
			selectMoveToForgetData.ItemsSource = pokemonMoveData;
			selectMoveToForget.Visibility = Visibility.Visible;
		}

		private void TeachMoveDontTeachButton(object sender, RoutedEventArgs e) {
			HideTeachMoveButtons();
			this.tcs.SetResult(-1);
		}

		private void ForgetThisMoveButton(object sender, RoutedEventArgs e) {
			selectMoveToForget.Visibility = Visibility.Hidden;
			Button button = (Button) sender;
			int moveToForgetId = (int) button.Tag;
			this.tcs.SetResult(moveToForgetId);
		}

		private void HideTeachMoveButtons() {
			teachMoveContinue.Visibility = Visibility.Hidden;
			teachMove.Visibility = Visibility.Hidden;
			teachMoveMenu.Visibility = Visibility.Hidden;
		}



		// ---- TURN (of player and wildpokemon) ---- //
		async private Task Turn(int id, Pokemon a, Pokemon t) {
			this.Attacker = a;
			this.Target = t;
			this.MoveId = id;
			await PreMoveHit();
			await CheckPokemonFainted();
		}



		// ---- ANIMATION OF HP BAR ---- //
		async public Task SetHpAnimated(Pokemon pokemon, int change) {
			if (change != 0) {
				int currentHp = pokemon.Hp;
				int maxAddHp = pokemon.GetStat("hp").StatValue - currentHp;

				if (change > 0) {
					// add hp
					if (change > maxAddHp) {
						change = maxAddHp;
					}
				} else {
					// subtract hp
					if (Math.Abs(change) > currentHp) {
						change = -currentHp;
					}
				}

				for (int i = 0; i < Math.Abs(change); i++) {
					if (change > 0) {
						pokemon.AddHp(1);
					} else {
						pokemon.LowerHp(1);
					}

					wildPokemon.Content = new PokemonConverter(WildPokemon);
					trainerPokemon.Content = new PokemonConverter(FirstPokemon);
					await Task.Delay(40);
				}
			}
		}



		// ---- EXECUTION OF MOVE ---- //
		async private Task PreMoveHit() {
			this.premovehits++;
			if (Attacker.VolatileStatus["confusion"] != -1) {
				if (Attacker.VolatileStatus["confusion"] == 0) {
					await SetOutputAsync(Attacker.NickName + " snapped out of confusion");
					await MoveHitLoop();
				} else {
					await SetOutputAsync(Attacker.NickName + " is confused");
					if (random.Next(1, 100) <= 50) {
						await SetOutputAsync(Attacker.NickName + " hurt itself in its confusion");
						await SetHpAnimated(Attacker, -int.Parse(CalculateDamage(true)["damage"]));
						wildPokemon.Content = new PokemonConverter(WildPokemon);
						trainerPokemon.Content = new PokemonConverter(FirstPokemon);
					} else {
						await MoveHitLoop();
					}
				}

				Attacker.VolatileStatus["confusion"]--;
			} else {
				await MoveHitLoop();
			}

			if (Attacker.Hp > 0) {
				await NonVolatileStatusDamage(Attacker);
				wildPokemon.Content = new PokemonConverter(WildPokemon);
				trainerPokemon.Content = new PokemonConverter(FirstPokemon);
			}
		}

		async private Task MoveHitLoop() {
			Move move = Attacker.Moves[MoveId];
			int hitCount = CalculateAmountOfHits();
			bool stopHit = false;
			int effectiveHits = 1;
			AttackHitOutput attackHitOutput;

			do {
				attackHitOutput = AttackHit();
				if (attackHitOutput.Attack) {
					if (attackHitOutput.Before && attackHitOutput.Message != "") {
						await SetOutputAsync(attackHitOutput.Message);
					}

					if (effectiveHits == 1) {
						await SetOutputAsync(Attacker.NickName + " uses " + move.MoveName);
					}

					if (!attackHitOutput.Before && attackHitOutput.Message != "") {
						await SetOutputAsync(attackHitOutput.Message);
					}

					Dictionary<string, string> damageOutput = CalculateDamage(false);
					int damage = int.Parse(damageOutput["damage"]);
					if (damageOutput.ContainsKey("crit")) {
						await SetOutputAsync(damageOutput["crit"]);
					}

					if (damageOutput.ContainsKey("message")) {
						await SetOutputAsync(damageOutput["message"]);
					}

					await SetHpAnimated(Target ,-damage);

					await UpdateStats(damage);
					wildPokemon.Content = new PokemonConverter(WildPokemon);
					trainerPokemon.Content = new PokemonConverter(FirstPokemon);
					effectiveHits++;
				} else {
					stopHit = true;
					if (attackHitOutput.Before && attackHitOutput.Message != "") {
						await SetOutputAsync(attackHitOutput.Message);
					}

					if (effectiveHits == 1 && !attackHitOutput.Before) {
						await SetOutputAsync(Attacker.NickName + " uses " + move.MoveName);
					}

					if (!attackHitOutput.Before && attackHitOutput.Message != "") {
						await SetOutputAsync(attackHitOutput.Message);
					}
				}
			} while (effectiveHits <= hitCount && !stopHit);

			if (move.MultiHitType != "single") {
				await SetOutputAsync("It hit " + effectiveHits + " time(s)");
			}
		}

		private AttackHitOutput AttackHit() {
			Move move = Attacker.Moves[this.MoveId];
			int mAccuracy = move.Accuracy;
			int pAccuracy = Attacker.InBattleStats["accuracy"];
			int tEvasion = Target.InBattleStats["evasion"];

			AttackHitOutput attackHitOutput = new AttackHitOutput(true, "", false);

			foreach (var type in Target.TypeList) {
				if (move.Type.NoDamageTo.Contains(type.TypeName)) {
					attackHitOutput = new AttackHitOutput(false, "This move doesn't effect the target", false);
				} else {
					attackHitOutput = new AttackHitOutput(true, "", false);
					break;
				}
			}

			double t;
			if (pAccuracy >= 0) {
				t = mAccuracy * ((3d + pAccuracy) / 3d);
			} else {
				t = mAccuracy * (3d / (3d + Math.Abs(pAccuracy)));
			}

			if (tEvasion >= 0) {
				t *= (3d / (3d + tEvasion));
			} else {
				t *= ((3d + Math.Abs(tEvasion)) / 3d);
			}

			if (random.Next(1, 100) > Math.Round(t)) {
				attackHitOutput = new AttackHitOutput(false, Attacker.NickName + " missed", false);
			} else if (Attacker.NonVolatileStatus["PAR"] == 1 && random.Next(1, 100) <= 25) {
				attackHitOutput = new AttackHitOutput(false, Attacker.NickName + " is paralyzed and can't move", true);
			} else if (Attacker.NonVolatileStatus["SLP"] != -1) {
				if (Attacker.NonVolatileStatus["SLP"] > 0) {
					attackHitOutput = new AttackHitOutput(false, Attacker.NickName + " is fast asleep", true);
				} else {
					attackHitOutput = new AttackHitOutput(false, Attacker.NickName + " woke up!", true);
				}
				Attacker.NonVolatileStatus["SLP"]--;
			} else if (Attacker.NonVolatileStatus["FRZ"] == 1) {
				if (random.Next(1, 100) > 10) {
					attackHitOutput = new AttackHitOutput(false, Attacker.NickName + " is frozen solid", true);
				} else {
					attackHitOutput = new AttackHitOutput(true, Attacker.NickName + " broke free!", true);
					Attacker.NonVolatileStatus["FRZ"] = 0;
				}
			} else if (Attacker.VolatileStatus["flinch"] != 0) {
				attackHitOutput = new AttackHitOutput(false, Attacker.NickName + " flinched", false);
				Attacker.VolatileStatus["flinch"] = 0;
			}

			return attackHitOutput;
		}



		// ---- CHECKING IF POKEMONS ARE FAINTED OR NOT ---- //
		async private Task CheckPokemonFainted() {
			if (this.FirstPokemon.Hp == 0) {
				BattleEnds = true;
				await SetOutputAsync(this.FirstPokemon.NickName + " fainted");
				bool hasPokemonWithHp = false;
				foreach (var pokemon in Trainer.CarryPokemonList.Values) {
					if (pokemon.Hp > 0) {
						hasPokemonWithHp = true;
					}
				}

				if (hasPokemonWithHp) {
					ActionSwitch();
				} else {
					this.TrainerWin = false;
					await EndBattle();
				}
			} else {
				if (WildPokemon.Hp == 0) {
					await SetOutputAsync(this.WildPokemon.NickName + " fainted");
					this.TrainerWin = true;
					this.BattleEnds = true;
					int experience = CalculateExperience(FirstPokemon, WildPokemon);
					await SetOutputAsync(FirstPokemon.NickName + " got " + experience + "xp");
					await UpdateXp(this.FirstPokemon, experience);

					await EndBattle();
				} else {
					this.BattleEnds = false;
				}
			}
		}

		async private Task EndBattle() {
			if (TrainerWin) {
				await SetOutputAsync("You win");
			} else {
				await SetOutputAsync("You lose");
			}

			PokePlayerApplication.MainApplication.navBar.Visibility = Visibility.Visible;
			PokePlayerApplication.MainApplication.mainContent.Content = new View_Party(Trainer);
		}



		// ---- UPDATING OF XP ---- //
		async private Task UpdateXp(Pokemon pokemon, int xp) {
			foreach (var move in pokemon.AddExperience(xp)) {
				if (pokemon.Moves.Count < 4) {
					await SetOutputAsync(pokemon.NickName + " learned the move " + move.MoveName);
					pokemon.Moves.Add(pokemon.Moves.Count + 1, move);
				} else {
					await SetOutputAsync(
						pokemon.NickName + " wants to learn the move " + move.MoveName + "\nDo you want to learn it?",
						true);
					TeachMove(pokemon, move);
					tcs = new TaskCompletionSource<int>();
					int id = await tcs.Task;
					if (id != -1) {
						await SetOutputAsync("1, 2, 3 and...");
						await SetOutputAsync("Poooof");
						if (pokemon.Moves.ContainsKey(id)) {
							await SetOutputAsync(pokemon.NickName + " forgot how to " + pokemon.Moves[id].MoveName);
						}

						pokemon.Moves[id] = move;
						pokemon.UpdateMovePp();
						await SetOutputAsync(pokemon.NickName + " learned the move " + move.MoveName);

					} else {
						await SetOutputAsync(pokemon.NickName + " did not learn the move " + move.MoveName);
					}
				}
			}
		}



		// ---- UPDATING OF STATS ---- //
		async private Task UpdateStats(int damage) {
			Move move = Attacker.Moves[MoveId];

			// Volatile and non volatile stats
			bool hasNonVolatileStatus = false;
			foreach (var value in Target.NonVolatileStatus.Values) {
				if (value > 0) {
					hasNonVolatileStatus = true;
					break;
				}
			}

			if (Target.NonVolatileStatus["SLP"] != -1 || Target.VolatileStatus["confusion"] != -1) {
				hasNonVolatileStatus = true;
			}

			if (move.Ailment["name"] != "-1" && !hasNonVolatileStatus) {
				int chance = int.Parse(move.Ailment["chance"]);
				if (move.Ailment["name"] == "PAR") {
					if (chance == 0 || random.Next(1, 100) <= chance) {
						Target.NonVolatileStatus["PAR"] = 1;
						await SetOutputAsync(Target.NickName + " is paralyzed and may be unable to move");
					}
				} else if (move.Ailment["name"] == "BRN") {
					if (chance == 0 || random.Next(1, 100) <= chance) {
						Target.NonVolatileStatus["BRN"] = CalculateNonVolatileStatusDamage(Target, 0);
						await SetOutputAsync(Target.NickName + " was burned");
					}
				} else if (move.Ailment["name"] == "FRZ") {
					if (chance == 0 || random.Next(1, 100) <= chance) {
						Target.NonVolatileStatus["FRZ"] = 1;
						await SetOutputAsync(Target.NickName + " was frozen");
					}
				} else if (move.Ailment["name"] == "PSN") {
					if (chance == 0 || random.Next(1, 100) <= chance) {
						Target.NonVolatileStatus["PSN"] = CalculateNonVolatileStatusDamage(Target, 0);
						await SetOutputAsync(Target.NickName + " was badly poisoned");
					}
				} else if (move.Ailment["name"] == "SLP") {
					if (chance == 0 || random.Next(1, 100) <= chance) {
						Target.NonVolatileStatus["SLP"] = random.Next(2, 6);
						await SetOutputAsync(Target.NickName + " fell asleep");
					}
				} else if (move.Ailment["name"] == "confusion") {
					if (chance == 0 || random.Next(1, 100) <= chance) {
						Target.VolatileStatus["confusion"] = random.Next(2, 6);
						await SetOutputAsync(Target.NickName + " is confused");
					}
				}
			}


			// In battle stats
			if (move.CritRate > 0 && Attacker.InBattleStats["critRate"] < 6) {
				Attacker.InBattleStats["critRate"]++;
				await SetOutputAsync(Attacker.NickName + " critical hit ratio rose!");
			}

			if (move.Healing > 0) {
				await SetHpAnimated(Attacker, move.Healing);
			}

			if (move.Drain > 0) {
				await SetHpAnimated(Attacker, (int) Math.Round(damage * (move.Drain / 100.0)));
			}

			foreach (var statChange in move.StatChanges) {
				if (statChange["name"] != "hp") {
					int change = int.Parse(statChange["amount"]);
					string statName = statChange["name"];
					bool failed = true;
					if (change > 0 &&
						Attacker.InBattleStats[statName] > -6 &&
					    Attacker.InBattleStats[statName] < 6) {
						failed = false;
						Attacker.InBattleStats[statName] += change;
						switch (change) {
							case 1:
								await SetOutputAsync(Attacker.NickName + "'s " + statName + " rose!");
								break;
							case 2:
								await SetOutputAsync(Attacker.NickName + "'s " + statName + " sharply rose!");
								break;
							case 3:
								await SetOutputAsync(Attacker.NickName + "'s " + statName + " rose drastically!");
								break;
						}
					}
					if (change < 0 &&
						Target.InBattleStats[statName] > -6 &&
					    Target.InBattleStats[statName] < 6) {
						failed = false;
						Target.InBattleStats[statName] += change;
						switch (change) {
							case -1:
								await SetOutputAsync(Target.NickName + "'s " + statName + " fell!");
								break;
							case -2:
								await SetOutputAsync(Target.NickName + "'s " + statName + " harshly fell!");
								break;
							case -3:
								await SetOutputAsync(Target.NickName + "'s " + statName + " severely fell!");
								break;
						}
					}

					if (failed) {
						await SetOutputAsync("But it failed!");
					}
				}
			}
		}



		// ---- NON VOLATILE STATUS DAMAGE ---- //
		// If pokemon has nonVolatile status, it gets damaged
		async private Task NonVolatileStatusDamage(Pokemon pokemon) {
			if (pokemon.NonVolatileStatus["PSN"] > 0) {
				await LowerNonVolatileStatus(pokemon, "PSN", "poison");
			} else if (pokemon.NonVolatileStatus["BRN"] > 0) {
				await LowerNonVolatileStatus(pokemon, "BRN", "burn");
			}
		}

		// Damage pokemon with certain nonVolatile status
		async private Task LowerNonVolatileStatus(Pokemon pokemon, string status, string statusLong) {
			await SetOutputAsync(pokemon.NickName + " got hurt by " + statusLong);
			if (pokemon.NonVolatileStatus[status] == 0) {
				pokemon.NonVolatileStatus[status] = CalculateNonVolatileStatusDamage(pokemon, pokemon.NonVolatileStatus[status]);
			}

			await SetHpAnimated(pokemon, -CalculateNonVolatileStatusDamage(pokemon, pokemon.NonVolatileStatus[status]));
		}



		// ---- CAPTURE POKEMON ---- //
		async private void CaptureAttempt() {
			await SetOutputAsync(Trainer.Name + " throws a pokeball");
			int maxHp = WildPokemon.GetStat("hp").StatValue;
			double a = (((3 * maxHp - 2 * WildPokemon.Hp) * WildPokemon.Specie.CaptureRate * 8) / (3 * maxHp));
			if (WildPokemon.NonVolatileStatus["BRN"] != 0 ||
			    WildPokemon.NonVolatileStatus["PAR"] != 0 ||
			    WildPokemon.NonVolatileStatus["PSN"] != 0) {
				a *= 1.5;
			} else if (WildPokemon.NonVolatileStatus["FRZ"] != 0 ||
			           WildPokemon.NonVolatileStatus["SLP"] != -1) {
				a *= 2;
			}

			double shakeProbability = (1048560 / (Math.Sqrt(Math.Sqrt(16711680 / a))));

			int shakes = 0;
			while (shakes < 4 && random.Next(0, 65536) < shakeProbability) {
				shakes++;
				if (shakes == 4) {
					await SetOutputAsync("You captured " + WildPokemon.NickName);
				} else {
					await SetOutputAsync("Shake " + shakes);
				}
			}

			if (shakes < 4) {
				await SetOutputAsync(WildPokemon.NickName + " got free");
				await Turn(random.Next(1, WildPokemon.Moves.Count), WildPokemon, FirstPokemon);
				ShowBattleMenu();
			} else {
				Trainer.AddPokemon(WildPokemon);
				PokePlayerApplication.MainApplication.navBar.Visibility = Visibility.Visible;
				PokePlayerApplication.MainApplication.mainContent.Content = new View_Party(Trainer);
			}
		}


		// ---- CALCULATION METHODS ---- //
		// Calculates burn or poison damage
		private int CalculateNonVolatileStatusDamage(Pokemon pokemon, int previousValue) {
			double newValue = previousValue + (pokemon.GetStat("hp").StatValue / 8);
			if (newValue > 15 * Math.Floor(pokemon.GetStat("hp").StatValue / 16.0))
				newValue = 15 * Math.Floor(pokemon.GetStat("hp").StatValue / 16.0);
			if (newValue < 1) {
				newValue = 1;
			}
			return (int) Math.Round(newValue, 0);
		}

		// Returns: - 1 for single hit moves
		//		    - 2 for double hit moves
		//		    - 2-5 for multi hit moves
		private int CalculateAmountOfHits() {
			Move move = Attacker.Moves[this.MoveId];
			if (!(move.MultiHitType == "single")) {
				if (move.MultiHitType == "double") {
					return 2;
				} else {
					int randHit = random.Next(1, 100);
					return randHit <= 35 ? 2 : randHit <= 70 ? 3 : randHit <= 85 ? 4 : 5;
				}
			}

			return 1;
		}

		// Returns calculated damage of move or does a basic damage calculation
		private Dictionary<string, string> CalculateDamage(bool basicDamageCalculation) {
			int damage;
			Move move = Attacker.Moves[MoveId];
			double a;
			double d;
			Attacker.MovePpMapping[MoveId]--;
			Dictionary<string, string> outputDictionary = new Dictionary<string, string>();
			if (move.Power != -1) {
				if (move.DamageClass == "physical") {
					a = CalculateStat(Attacker.GetStat("attack").StatValue, Attacker.InBattleStats["attack"]);
					d = CalculateStat(Target.GetStat("defense").StatValue, Target.InBattleStats["defense"]);
				} else {
					a = CalculateStat(Attacker.GetStat("special-attack").StatValue, Attacker.InBattleStats["special-attack"]);
					d = CalculateStat(Target.GetStat("special-defense").StatValue, Target.InBattleStats["special-defense"]);
				}

				if (basicDamageCalculation) {
					damage = (int) Math.Round(((((2.0 * Attacker.Level) / 5.0) + 2.0) * 40.0 * (a / d)) / 50.0);
				} else {
					MultiplierOutput multiplierOutput = CalculateMultiplier();
					damage = (int) Math.Round(((((2.0 * Attacker.Level) / 5.0) + 2.0) * move.Power * (a / d) / 50.0) *
					                          multiplierOutput.Multiplier);
					if (multiplierOutput.Crit) {
						outputDictionary.Add("crit", "A critical hit!");
					}

					if (multiplierOutput.Message != "") {
						outputDictionary.Add("message", multiplierOutput.Message);

					}
				}
			} else {
				damage = 0;
			}

			outputDictionary.Add("damage", "" + damage);

			return outputDictionary;
		}

		// Calculates the stats used for the moves
		// This is a combination of the in battle stats and the effective stats
		private double CalculateStat(int effectiveStat, int inBattleStat) {
			double calcVal = effectiveStat;
			if (inBattleStat >= 0) {
				calcVal *= ((2d + ((double)inBattleStat)) / 2d);
			} else {
				double multiplier = 2d / (2d + ((double) Math.Abs(inBattleStat)));
				calcVal *= multiplier;
			}

			return calcVal;
		}

		// Calculates the value the basic damage is multiplied with
		private MultiplierOutput CalculateMultiplier() {
			// Calculates the type multiplier
			Type moveType = Attacker.Moves[MoveId].Type;
			List<Type> targetTypes = Target.TypeList;
			double multiplier = 1.0;

			foreach (var targetType in targetTypes) {
				if (moveType.HalfDamageTo.Contains(targetType.TypeName)) {
					multiplier *= 0.5;
				}

				if (moveType.DoubleDamageTo.Contains(targetType.TypeName)) {
					multiplier *= 2;
				}
			}

			string multiplierMessage = "";
			if (multiplier < 1) {
				multiplierMessage = "It was not very effective";
			} else if (multiplier > 1) {
				multiplierMessage = "It was very effective";
			}


			// Randomizes critical hit based on inBattleStat value of criticalHitRate
			bool crit = Attacker.InBattleStats["critRate"] switch {
				0 => random.Next(0, 100) <= 6,
				1 => random.Next(0, 100) <= 13,
				2 => random.Next(0, 100) <= 25,
				3 => random.Next(0, 100) <= 33,
				_ => random.Next(0, 100) <= 50,
			};

			if (crit) {
				multiplier *= 2;
			}

			return new MultiplierOutput(multiplier, multiplierMessage, crit);
		}

		// Calculates experience
		private int CalculateExperience(Pokemon winner, Pokemon loser) {
			return (int) Math.Round((loser.BaseExperience * loser.Level / 5 * Math.Pow(((2 * loser.Level) + 10) / (loser.Level + winner.Level + 10), 2)) + 1);
		}



		// ---- SET OUTPUT TEXT ---- //
		async private Task SetOutputAsync(string output, bool keepTextOnScreen=false) {
			outputText.Text = "";
			foreach (var letter in output) {
				string currentText = outputText.Text;
				currentText += letter;
				outputText.Text = currentText;
				await Task.Delay(3);
			}
			await Task.Delay(1500);
			if (!keepTextOnScreen) {
				outputText.Text = "";
			}
		}

		private void SetOutputSync(string output) {
			outputText.Text = output;
		}
	}
}