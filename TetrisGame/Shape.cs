using System.Drawing;

namespace TetrisGame
{
    class Shape
    {
        public int Width; // Largeur de la forme
        public int Height; // Hauteur de la forme
        public int[,] Dots; // Représentation de la forme sous forme de tableau 2D
        public Color ShapeColor; // Couleur de la forme

        private int[,] backupDots; // Copie de sauvegarde de la forme pour les annulations

        public void turn()
        {
            // Sauvegarder les valeurs actuelles de la forme dans backupDots
            backupDots = Dots;

            // Créer un nouveau tableau pour la forme pivotée
            Dots = new int[Width, Height];

            // Pivoter la forme de 90 degrés dans le sens horaire
            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Dots[i, j] = backupDots[Height - 1 - j, i];
                }
            }

            // Échanger la largeur et la hauteur après la rotation
            var temp = Width;
            Width = Height;
            Height = temp;
        }

        public void rollback()
        {
            // Restaurer la forme à son état précédent
            Dots = backupDots;

            // Restaurer la largeur et la hauteur d'origine
            var temp = Width;
            Width = Height;
            Height = temp;
        }
    }
}
