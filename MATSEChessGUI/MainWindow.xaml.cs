using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using MATSEChess;

namespace MATSEChessGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly int TILE_SIZE = 64;
        private readonly Brush BRIGHT_BRUSH = new SolidBrush(Color.FromArgb(255, 240, 217, 181));
        private readonly SolidBrush DARK_BRUSH = new SolidBrush(Color.FromArgb(255, 181, 136, 99));
        private ChessPieceImageManager manager = new ChessPieceImageManager();

        public MainWindow()
        {
            InitializeComponent();
            manager.LoadImages();
            RenderBoard();
        }

        private void OnBoardMouseDown(object sender, MouseButtonEventArgs e)
        {
            var pos = e.GetPosition(boardImage);
            int relativeX = (int)Math.Floor(pos.X/TILE_SIZE);
            int relativeY = (int)Math.Floor(pos.Y/TILE_SIZE);
            MessageBox.Show($"X: {relativeX}, Y: {relativeY}");
        }

        private void RenderBoard()
        {
            ChessBoard board = new ChessBoard();
            board.Reset();

            var bitmap = DrawBasicBoard();
            DrawPieces(bitmap, board);
            boardImage.Source = BitmapToImageSource(bitmap);
        }

        private Bitmap DrawBasicBoard()
        {
            var bitmap = new Bitmap(TILE_SIZE * 8, TILE_SIZE * 8);
            using (Graphics g = Graphics.FromImage(bitmap))
            { 
                for (int x = 0; x < 8; ++x)
                {
                    for (int y = 0; y < 8; ++y)
                    {
                        Brush b = ((x+y) % 2 == 0) ? DARK_BRUSH : BRIGHT_BRUSH;
                        g.FillRectangle(b, new Rectangle(TILE_SIZE * x, TILE_SIZE * y, TILE_SIZE, TILE_SIZE));
                    }
                } 
            }

            return bitmap;
        }

        private void DrawPieces(Bitmap bitmap, ChessBoard board)
        {
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                foreach (var piece in board.Pieces)
                {
                    g.DrawImageUnscaled(manager.GetImageTile(piece.Type, piece.Color), new System.Drawing.Point(TILE_SIZE * piece.Position.X, TILE_SIZE * piece.Position.Y));
                }
            }

        }

        BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }
    }
}
