using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xamarin.Forms;

namespace Puzzle
{
    public partial class MainPage : ContentPage
    {
        private readonly ImageSource[] _images = new ImageSource[9];
        private bool Initialized { get; set; }
        private Image Selected { get; set; }

        public MainPage()
        {
            InitializeComponent();

            for (var i = 0; i < _images.Length; i++)
            {
                _images[i] = ImageSource.FromFile("tile_" + (9 - i) + ".jpg");
            }
        }

        private void Initialize()
        {
            var width = Application.Current.MainPage.Width;
            var height = Application.Current.MainPage.Height;
            var pieceLength = (width <= height ? width : height) / 3;

            foreach (var row in Grid.RowDefinitions)
            {
                row.Height = pieceLength;
            }

            foreach (var column in Grid.ColumnDefinitions)
            {
                column.Width = pieceLength;
            }

            var singleTapRecognizer = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 1
            };

            var doubleTapRecognizer = new TapGestureRecognizer
            {
                NumberOfTapsRequired = 2
            };

            singleTapRecognizer.Tapped += OnSingleTap;
            doubleTapRecognizer.Tapped += OnDoubleTap;

            foreach (var child in Grid.Children)
            {
                var frame = (Frame) child;
                frame.GestureRecognizers.Add(singleTapRecognizer);
                frame.GestureRecognizers.Add(doubleTapRecognizer);
            }
        }

        private void Reset()
        {
            if (!Initialized)
            {
                Initialize();
                Initialized = true;
            }

            var random = new Random();
            var reserved = new List<int>(_images.Length);

            foreach (var child in Grid.Children)
            {
                var index = random.Next(9);

                while (reserved.Contains(index))
                {
                    index = random.Next(9);
                }

                reserved.Add(index);

                var frame = (Frame) child;
                var image = frame.Children[0] as Image;
                image.Source = _images[index];
            }

            foreach (var child in Grid.Children)
            {
                var frame = (Frame) child;
                frame.Rotation = random.Next(4) * 90;
                Debug.WriteLine("Rotation: " + frame.Rotation);
            }
        }

        private void Reset_OnClicked(object sender, EventArgs e)
        {
            Reset();
        }

        private void CheckWin()
        {
            var win = false;

            for (var i = 0; i < Grid.Children.Count; i++)
            {
                if (!(Grid.Children[i] is Frame frame)) return;
                if (!(frame.Children[0] is Image image)) return;

                var isUpright = frame.Rotation % 360 == 0;
                var isCorrectLocation = image.Source == _images[i];

                Debug.WriteLine(i + ": " + isUpright);

                win = isUpright && isCorrectLocation;
            }

            if (win) DisplayAlert("Result", "You win!", "OK");
        }

        private void OnSingleTap(object sender, EventArgs args)
        {
            if (!(sender is Frame frame)) return;
            if (!(frame.Children[0] is Image image)) return;

            if (Selected == null)
            {
                frame.BorderColor = Color.Red;
                Selected = image;
            } else if (Selected == image)
            {
                frame.BorderColor = Color.Transparent;
                Selected = null;
            }
            else
            {
                ((Frame) Selected.Parent).BorderColor = Color.Transparent;
                (((Frame) Selected.Parent).Rotation, frame.Rotation) = (frame.Rotation, ((Frame) Selected.Parent).Rotation);
                (Selected.Source, image.Source) = (image.Source, Selected.Source);
                Selected = null;

                CheckWin();
            }
        }

        private void OnDoubleTap(object sender, EventArgs args)
        {
            if (Selected != null)
            {
                ((Frame) Selected.Parent).BorderColor = Color.Transparent;
                Selected = null;
            }

            if (sender is Frame frame) frame.Rotation += 90;

            Debug.WriteLine("Modulo: " + ((Frame)sender).Rotation % 360);
            Debug.WriteLine("Rotation: " + ((Frame)sender).Rotation);

            CheckWin();
        }
    }
}