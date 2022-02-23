using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Puzzle
{
    public partial class MainPage : ContentPage
    {
        private readonly ImageSource[] _tiles = new ImageSource[9];
        private Frame _selected;

        public MainPage()
        {
            InitializeComponent();

            var recognizer = new TapGestureRecognizer();
            recognizer.Tapped += Start;

            Welcome.GestureRecognizers.Add(recognizer);
        }

        private async Task CheckWin()
        {
            for (var i = 0; i < BoardGrid.Children.Count; i++)
            {
                var frame = BoardGrid.Children[i] as Frame;
                var tile = frame.Children[0] as Image;

                if (frame.Rotation % 360 != 0) return;
                if (tile.Source != _tiles[i]) return;
            }

            await DisplayAlert("Result", "You win!", "OK");

            Reset(this, EventArgs.Empty);
        }

        private void Start(object sender, EventArgs args)
        {
            Welcome.IsVisible = false;
            Board.IsVisible = true;
            WelcomeRow.Height = 0;
            BoardRow.Height = GridLength.Star;

            var singleTapRecognizer = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };

            var doubleTapRecognizer = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2
            };

            singleTapRecognizer.Tapped += Select;
            doubleTapRecognizer.Tapped += Rotate;

            foreach (var view in BoardGrid.Children)
            {
                var frame = view as Frame;

                frame.GestureRecognizers.Add(singleTapRecognizer);
                frame.GestureRecognizers.Add(doubleTapRecognizer);
            }

            Reset(this, EventArgs.Empty);
        }

        private async void Select(object sender, EventArgs e)
        {
            var frame = sender as Frame;
            var tile = frame.Children[0] as Image;

            if (_selected == null)
            {
                frame.BorderColor = Color.White;
                _selected = tile.Parent as Frame;
            } else if (_selected == frame)
            {
                frame.BorderColor = Color.SaddleBrown;
                _selected = null;
            }
            else
            {
                _selected.BorderColor = Color.SaddleBrown;
                (_selected.Rotation, frame.Rotation) = (frame.Rotation, _selected.Rotation);
                ((_selected.Children[0] as Image).Source, tile.Source) = (tile.Source, (_selected.Children[0] as Image).Source);
                _selected = null;

                await CheckWin();
            }
        }

        private async void Rotate(object sender, EventArgs e)
        {
            var frame = sender as Frame;

            frame.Rotation += 90;

            await CheckWin();
        }

        private void Reset(object sender, EventArgs args)
        {
            var random = new Random();
            var image = char.Parse("a") + random.Next(3);

            for (var i = 0; i < _tiles.Length; i++)
            {
                _tiles[i] = ImageSource.FromFile(new string(new[]{(char) image}) + "_" + (9 - i) + ".jpg");
            }

            var reserved = new List<int>(_tiles.Length);

            foreach (var child in BoardGrid.Children)
            {
                var index = random.Next(9);

                while (reserved.Contains(index))
                {
                    index = random.Next(9);
                }

                reserved.Add(index);

                var frame = (Frame) child;
                var view = frame.Children[0] as Image;

                frame.Rotation = random.Next(4) * 90;
                view.Source = _tiles[index];
            }
        }
    }
}