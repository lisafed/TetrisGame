using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TetrisGame
{
    public partial class Form1 : Form
    {
        Shape currentShape;
        Shape nextShape;
        Timer timer = new Timer();
        public Form1()
        {
            InitializeComponent();

            loadCanvas();
            currentShape = getRandomShapeWithCenterAligned();
            nextShape = getNextShape();

            timer.Tick += Timer_Tick;
            timer.Interval = 500;
            timer.Start();
            this.KeyDown += Form1_KeyDown;


        }

        Bitmap canvasBitmap;
        Graphics canvasGraphics;
        int canvasWidth = 15;
        int canvasHeight = 20;
        int[,] canvasDotArray;
        int dotSize = 20;

        private void loadCanvas()
        {
            // Redimensionner la zone d'affichage en fonction de la taille des cellules et de la grille
            pictureBox1.Width = canvasWidth * dotSize;
            pictureBox1.Height = canvasHeight * dotSize;

            // Créer une image Bitmap correspondant à la taille de la zone d'affichage
            canvasBitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            canvasGraphics = Graphics.FromImage(canvasBitmap);

            // Remplir le canevas avec une couleur grise claire par défaut
            canvasGraphics.FillRectangle(Brushes.LightGray, 0, 0, canvasBitmap.Width, canvasBitmap.Height);

            // Charger l'image dans la zone d'affichage
            pictureBox1.Image = canvasBitmap;

            // Initialiser le tableau représentant la grille (toutes les cases à zéro par défaut)
            canvasDotArray = new int[canvasWidth, canvasHeight];
        }

        int currentX;
        int currentY;

        private Shape getRandomShapeWithCenterAligned()
        {
            var shape = ShapesHandler.GetRandomShape();

            // Calculer les coordonnées initiales pour centrer la forme horizontalement
            currentX = 7;
            currentY = -shape.Height;

            return shape;
        }

        // Retourne false si la forme atteint le bas ou entre en collision avec d'autres blocs
        private bool moveShapeIfPossible(int moveDown = 0, int moveSide = 0)
        {
            var newX = currentX + moveSide;
            var newY = currentY + moveDown;

            // Vérifier si la forme dépasse les limites de la grille
            if (newX < 0 || newX + currentShape.Width > canvasWidth
                || newY + currentShape.Height > canvasHeight)
                return false;

            // Vérifier si la forme entre en collision avec d'autres blocs
            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (newY + j > 0 && canvasDotArray[newX + i, newY + j] == 1 && currentShape.Dots[j, i] == 1)
                        return false;
                }
            }

            currentX = newX;
            currentY = newY;

            drawShape();

            return true;
        }

        Bitmap workingBitmap;
        Graphics workingGraphics;

        private void drawShape()
        {
            // Créer une copie de l'image du canevas pour dessiner dessus
            workingBitmap = new Bitmap(canvasBitmap);
            workingGraphics = Graphics.FromImage(workingBitmap);

            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1)
                    {
                        // Dessiner les cellules actives de la forme avec leur couleur
                        using (Brush brush = new SolidBrush(currentShape.ShapeColor))
                        {
                            workingGraphics.FillRectangle(brush, (currentX + i) * dotSize, (currentY + j) * dotSize, dotSize, dotSize);
                        }
                    }
                }
            }

            // Charger l'image mise à jour dans la zone d'affichage
            pictureBox1.Image = workingBitmap;
        }

        private void updateCanvasDotArrayWithCurrentShape()
        {
            // Mettre à jour le tableau de la grille avec la position actuelle de la forme
            for (int i = 0; i < currentShape.Width; i++)
            {
                for (int j = 0; j < currentShape.Height; j++)
                {
                    if (currentShape.Dots[j, i] == 1)
                    {
                        checkIfGameOver();

                        canvasDotArray[currentX + i, currentY + j] = 1;
                    }
                }
            }
        }

        private void checkIfGameOver()
        {
            // Vérifier si une forme dépasse le haut de la grille, ce qui signifie la fin de la partie
            if (currentY < 0)
            {
                timer.Stop();
                MessageBox.Show("Game Over");
                Application.Restart();
            }
        }

        private Shape getNextShape()
        {
            var shape = getRandomShapeWithCenterAligned();

            // Code pour afficher la prochaine forme dans le panneau latéral
            nextShapeBitmap = new Bitmap(6 * dotSize, 6 * dotSize);
            nextShapeGraphics = Graphics.FromImage(nextShapeBitmap);

            nextShapeGraphics.FillRectangle(Brushes.LightGray, 0, 0, nextShapeBitmap.Width, nextShapeBitmap.Height);

            // Calculer la position idéale pour centrer la forme dans le panneau latéral
            var startX = (6 - shape.Width) / 2;
            var startY = (6 - shape.Height) / 2;

            for (int i = 0; i < shape.Height; i++)
            {
                for (int j = 0; j < shape.Width; j++)
                {
                    nextShapeGraphics.FillRectangle(
                        shape.Dots[i, j] == 1 ? Brushes.Black : Brushes.LightGray,
                        (startX + j) * dotSize, (startY + i) * dotSize, dotSize, dotSize);
                }
            }

            pictureBox2.Size = nextShapeBitmap.Size;
            pictureBox2.Image = nextShapeBitmap;

            return shape;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var isMoveSuccess = moveShapeIfPossible(moveDown: 1);

            // Si la forme atteint le bas ou touche une autre forme
            if (!isMoveSuccess)
            {
                // Copier l'image de travail dans l'image du canevas
                canvasBitmap = new Bitmap(workingBitmap);

                updateCanvasDotArrayWithCurrentShape();

                // Charger la forme suivante
                currentShape = nextShape;
                nextShape = getNextShape();

                clearFilledRowsAndUpdateScore();
            }


        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            var verticalMove = 0;
            var horizontalMove = 0;

            // Calculer les mouvements verticaux et horizontaux selon la touche pressée
            switch (e.KeyCode)
            {
                // Déplacer la forme vers la gauche
                case Keys.Left:
                    verticalMove--;
                    break;

                // Déplacer la forme vers la droite
                case Keys.Right:
                    verticalMove++;
                    break;

                // Faire descendre la forme plus rapidement
                case Keys.Down:
                    horizontalMove++;
                    break;

                // Faire pivoter la forme dans le sens horaire
                case Keys.Up:
                    currentShape.turn();
                    break;

                default:
                    return;
            }

            var isMoveSuccess = moveShapeIfPossible(horizontalMove, verticalMove);

            // Si la rotation n'est pas possible, revenir à l'état précédent
            if (!isMoveSuccess && e.KeyCode == Keys.Up)
                currentShape.rollback();
        }

        int score;

        public void clearFilledRowsAndUpdateScore()
        {
            // Vérifier chaque ligne de la grille
            for (int i = 0; i < canvasHeight; i++)
            {
                int j;
                for (j = canvasWidth - 1; j >= 0; j--)
                {
                    if (canvasDotArray[j, i] == 0)
                        break;
                }

                if (j == -1)
                {
                    // Mettre à jour le score et le niveau
                    score++;
                    label1.Text = "Score: " + score;
                    label2.Text = "Level: " + score / 10;
                    // Augmenter la vitesse du jeu
                    timer.Interval -= 10;

                    // Déplacer les lignes supérieures vers le bas
                    for (j = 0; j < canvasWidth; j++)
                    {
                        for (int k = i; k > 0; k--)
                        {
                            canvasDotArray[j, k] = canvasDotArray[j, k - 1];
                        }

                        canvasDotArray[j, 0] = 0;
                    }
                }
            }

            // Redessiner le canevas après avoir mis à jour les lignes
            for (int i = 0; i < canvasWidth; i++)
            {
                for (int j = 0; j < canvasHeight; j++)
                {
                    canvasGraphics = Graphics.FromImage(canvasBitmap);
                    canvasGraphics.FillRectangle(
                        canvasDotArray[i, j] == 1 ? Brushes.Black : Brushes.LightGray,
                        i * dotSize, j * dotSize, dotSize, dotSize
                        );
                }
            }
            pictureBox1.Image = canvasBitmap;
        }

        Bitmap nextShapeBitmap;
        Graphics nextShapeGraphics;


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click_1(object sender, EventArgs e)
        {

        }
    }
}