using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using BlackJackApp.Models;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml;

namespace BlackJackApp.Presentation
{
   public enum Alignment
    {
        Horizontal,
        Vertical,
        Left,
        Right,
        Center
    }

    public enum Action
    {
        Take,
        Give,
        None
    }

    public abstract class CardPage:Page
    {
        public abstract Grid MainGrid { get; }

        public CardPage()
        {
           SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawCardsOnScreen();
        }

        public void OnLoaded(object sender, RoutedEventArgs e)
        {
            DrawCardsOnScreen();
        }

        protected abstract void DrawCardsOnScreen();
        
        /// <summary>
        /// Function to draw cards on the screen
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="cards"></param>
        /// <param name="alignment"></param>
        protected void DrawCards(Canvas canvas, List<Card> cards, Alignment alignment)
        {
            canvas.Children.Clear();
            double center = canvas.ActualWidth / 2;
            double width = MainGrid.RenderSize.Width / 12;

            //loop through the list of cards
            for (int imageIndex = 0; imageIndex < cards.Count(); imageIndex++)
            {
                Image image = new Image();
                image.Width = width;
                image.Height = canvas.ActualHeight;
                        
                if (cards[imageIndex].FaceUp)
                {
                    image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/{cards[imageIndex].GetFileName()}"));
                }
                else
                {
                    image.Source = new BitmapImage(new Uri($"ms-appx:///Assets/Cards/cardBack_red1.png"));
                }
                double offset = imageIndex * width;
                canvas.Children.Add(image);
                Canvas.SetTop(image, 0);

                // center
                if (alignment == Alignment.Center)
                {

                    offset = Math.Ceiling(Convert.ToDouble(imageIndex) / 2) * width;
                    if (imageIndex % 2 == 1)
                        offset *= -1;
                    Canvas.SetLeft(image, center + offset);

                }
                // left
                else if (alignment == Alignment.Left)
                {
                    Canvas.SetLeft(image, offset);
                }
                // right
                else if (alignment == Alignment.Right)
                {
                    Canvas.SetLeft(image, canvas.ActualWidth - offset - width);
                }

            }
        }

    }
}
