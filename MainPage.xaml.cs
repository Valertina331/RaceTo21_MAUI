using Microsoft.Maui.Layouts;
using RaceTo21;

namespace RaceTo21_MAUI;

public partial class MainPage : ContentPage
{
    private StackLayout PlayersNameLayout = new StackLayout();
    private List<Player> players = new List<Player>();
    private Deck deck;
    private int currentTurn = 1;

    public MainPage()
	{
		InitializeComponent();
    }

    private void StartButton_Clicked(System.Object sender, System.EventArgs e)
    {
        GameTitle.IsVisible = false;
        StartButton.IsVisible = false;

        AskForPlayerNum.IsVisible = true;
        PlayerNum.IsVisible = true;
        Enter1.IsVisible = true;
    }

    private void Enter1_Clicked(System.Object sender, System.EventArgs e)
    {
        int playerNum;
        if (int.TryParse(PlayerNum.Text, out playerNum) && 1 < playerNum && playerNum < 7)
        {
            AskForPlayerNum.IsVisible = false;
            PlayerNum.IsVisible = false;
            Enter1.IsVisible = false;
		    if (int.TryParse(PlayerNum.Text, out playerNum))
		    {
                Main.Children.Add(PlayersNameLayout);
                for (int i = 1; i <= playerNum; i++)
			    {
				    var PlayerNameLabel = new Label { FontFamily = "BitMono", Text = "Player #" + i + ", what's your name?", FontSize = 20};
                    PlayersNameLayout.Children.Add(PlayerNameLabel);
                    var PlayerNameEntry = new Entry { FontFamily = "BitMono", FontSize = 20};
                    PlayersNameLayout.Children.Add(PlayerNameEntry);
                }
                var Enter2 = new Button { FontFamily = "BitMono", Text = "Enter", FontSize = 20};
                Enter2.Clicked += Enter2_Clicked;
                PlayersNameLayout.Children.Add(Enter2);
            }
        }

    }

    private void Enter2_Clicked(object sender, EventArgs e)
    {
        TurnLabel.IsVisible = true;
        var playerNames = new List<string>();
        players.Clear();
        foreach (var enterPlayers in PlayersNameLayout.Children)
        {
            if (enterPlayers is Entry entry && !string.IsNullOrEmpty(entry.Text))
            {
                playerNames.Add(entry.Text);
                players.Add(new Player(entry.Text));
            }
        }
        PlayersNameLayout.IsVisible = false;
        deck = new Deck();
        deck.Shuffle();

        InitializePlayersAreas();
    }

    private void InitializePlayersAreas()
    {
        foreach (var player in players)
        {
            // Create player area
            var playerArea = new VerticalStackLayout
            {
                Spacing = 5,
                Margin = 10,
                WidthRequest = 400,
                HeightRequest = 400,
                BackgroundColor = Color.FromArgb("#252525")
            };
            playerArea.Children.Add(new Label { Text = player.name, FontSize = 20, FontFamily = "BitMono" });
            var playerStatus = new Label { FontSize = 14, FontFamily = "BitMono" };
            player.StatusLabel = playerStatus;
            playerArea.Children.Add(playerStatus);
            // Create hand score area
            var totalValueLabel = new Label { FontSize = 14, FontFamily = "BitMono" };
            player.TotalValueLabel = totalValueLabel;
            playerArea.Children.Add(totalValueLabel);

            // Create buttons
            var hitButton = new Button { Text = "Hit", FontSize = 14, FontFamily = "BitMono", WidthRequest = 70 };
            hitButton.Clicked += (sender, e) => DealCardToPlayer(player);
            player.HitButton = hitButton;
            playerArea.Children.Add(hitButton);

            var stayButton = new Button { Text = "Stay", FontSize = 14, FontFamily = "BitMono", WidthRequest = 70 };
            stayButton.Clicked += (sender, e) => PlayerStay(player);
            player.StayButton = stayButton;
            playerArea.Children.Add(stayButton);

            // Create hand area
            player.HandLayout = new FlexLayout
            {
                Direction = FlexDirection.Row,
                Wrap = FlexWrap.Wrap, 
                AlignItems = FlexAlignItems.Center,
                JustifyContent = FlexJustify.Center
            };
            playerArea.Children.Add(player.HandLayout);

            PlayersInfo.Children.Add(playerArea);
        }
    }

    private void PlayerStay(Player player)
    {
        if (!player.HasActedThisRound)
        {
            player.status = PlayerStatus.stay;
            player.StatusLabel.Text = "(Stay)";
            player.HitButton.IsEnabled = false; 
            player.StayButton.IsEnabled = false; 
        }

        player.HasActedThisRound = true;
        CheckTurn();
    }

    private void CheckTurn()
    {
        if (!CheckActivePlayers())
        {
            Player winner = DoFinalScoring();
            if (winner != null)
            {
                WinnerLabel.Text = winner.name + " wins!";
                WinnerLabel.IsVisible = true;
            }
            else
            {
                WinnerLabel.Text = "Everyone busted!";
                WinnerLabel.IsVisible = true;
            }

            foreach (var player in players)
            {
                player.HitButton.IsEnabled = false;
                player.StayButton.IsEnabled = false;
            }
            ContinueButton.IsVisible = true;
        }
        else
        {
            if (players.All(p => p.HasActedThisRound))
            {
                currentTurn++;
                TurnLabel.Text = "Round: " + currentTurn;
                foreach (var player in players)
                {
                    if (player.status == PlayerStatus.active)
                    {
                        player.HasActedThisRound = false; 
                        player.HitButton.IsEnabled = true;
                        player.StayButton.IsEnabled = true;
                    }
                }
            }
        }
    }

    private void DealCardToPlayer(Player player)
    {
        if (!player.HasActedThisRound)
        {
            //if (deck == null || deck.IsEmpty())
            //{
            //    deck = new Deck();
            //    deck.Shuffle();
            //}

            var card = deck.DealTopCard();
            player.cards.Add(card);

            // Card img
            var cardImage = new Image
            {
                Source = ImageSource.FromFile(GetCardImageFilePath(card.ID)),
                WidthRequest = 80, 
            };
            //var cardID = new Label { Text = card.ID };
            player.HandLayout.Children.Add(cardImage);
            //player.HandLayout.Children.Add(cardID);

            player.TotalScore = player.CardsTotalValue();
            player.TotalValueLabel.Text = "Total: " + player.TotalScore;

            if (player.TotalScore > 21)
            {
                player.status = PlayerStatus.bust;
                player.StatusLabel.Text = "(Bust)";
                player.HitButton.IsEnabled = false;
                player.StayButton.IsEnabled = false;
            }
            else if (player.TotalScore == 21)
            {
                player.status = PlayerStatus.win;
                player.StatusLabel.Text = "(Win)";
                CheckTurn();
            }

            player.HasActedThisRound = true;
            CheckTurn();
        }
    }

    private string GetCardImageFilePath(string cardId)
    {
        return $"images/{cardId}.png";
    }

    public bool CheckActivePlayers()
    {
        foreach (var player in players)
        {
            if (player.status == PlayerStatus.win)
            {
                return false;
            }
            if (player.status == PlayerStatus.active)
            {
                return true; // at least one player is still going!
            }
        }
        return false; // everyone has stayed or busted, or someone won!
    }

    public Player DoFinalScoring()
    {
        int highScore = 0;
        foreach (var player in players)
        {
            if (player.status == PlayerStatus.win) // someone hit 21
            {
                return player;
            }
            if (player.status == PlayerStatus.stay) // still could win...
            {
                if (player.TotalScore > highScore)
                {
                    highScore = player.TotalScore;
                }
            }
            // if busted don't bother checking!
        }
        if (highScore > 0) // someone scored, anyway!
        {
            // find the FIRST player in list who meets win condition
            return players.Find(player => player.TotalScore == highScore);
        }
        return null; // everyone must have busted because nobody won!
    }

    private async void ContinueButton_Clicked(System.Object sender, System.EventArgs e)
    {
        List<Player> playersQuit = new List<Player>();

        foreach (var player in players)
        {
            var answer = await DisplayAlert("Continue?", player.name + ", do you want to play another round?", "Yes", "No");
            if (!answer)
            {
                playersQuit.Add(player);
            }
        }

        foreach (var playerQuit in playersQuit)
        {
            players.Remove(playerQuit);
        }

        if (players.Count > 1)
        {
            StartNewRound();
        }
        else
        {
            await DisplayAlert("Game Over", "There're not enough players. The game will now exit.", "OK");
            // Exit the game
        }
    }

    private void StartNewRound()
    {
        //deck = new Deck();
        //deck.Shuffle();
        PlayersInfo.Children.Clear();
        foreach (var player in players)
        {
            player.cards.Clear();
            player.status = PlayerStatus.active;
            player.HasActedThisRound = false;
            player.TotalScore = 0;
        }
        ContinueButton.IsVisible = false;
        //playerArea.Children.Clear();
        TurnLabel.Text = "Round: 1";
        WinnerLabel.IsVisible = false;

        InitializePlayersAreas();
    }

}

