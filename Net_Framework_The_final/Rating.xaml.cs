using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace English_for_kids
{
    /// <summary>
    /// Interaction logic for Rating.xaml
    /// </summary>
    public partial class Rating : Window
    {
        public Rating(List<Player> players)
        {
            InitializeComponent();

            players.Sort();
            players.Reverse();

            for (int i = 0; i < players.Count; i++)
                players[i].Number = i + 1;

            List<Player> top_players = new List<Player>();
            int kol = 0;

            if (players.Count < 5)
                kol = players.Count;
            else
                kol = 5;

            for (int i = 0; i < kol; i++)
                top_players.Add(players[i]);

            dg_pl.ItemsSource = top_players;
        }
    }
}
