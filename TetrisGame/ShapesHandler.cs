using System;
using System.Drawing;

namespace TetrisGame
{
    internal class ShapesHandler
    {
        private static Shape[] shapesArray; // Tableau contenant toutes les formes disponibles

        // Constructeur qui initialise les formes disponibles
        static ShapesHandler()
        {
            shapesArray = new Shape[]
            {
                new Shape {
                    Width = 2,
                    Height = 2,
                    Dots = new int[,]
                    {
                        { 1, 1 },
                        { 1, 1 }
                    },
                    ShapeColor = Color.Red // Carré 
                },
                new Shape {
                    Width = 1,
                    Height = 4,
                    Dots = new int[,]
                    {
                        { 1 },
                        { 1 },
                        { 1 },
                        { 1 }
                    },
                    ShapeColor = Color.Blue // Barre (bleue)
                },
                new Shape {
                    Width = 3,
                    Height = 2,
                    Dots = new int[,]
                    {
                        { 0, 1, 0 },
                        { 1, 1, 1 }
                    },
                    ShapeColor = Color.Green // Forme T
                },
                new Shape {
                    Width = 3,
                    Height = 2,
                    Dots = new int[,]
                    {
                        { 0, 0, 1 },
                        { 1, 1, 1 }
                    },
                    ShapeColor = Color.Purple // Forme L inversé
                },
                new Shape {
                    Width = 3,
                    Height = 2,
                    Dots = new int[,]
                    {
                        { 1, 0, 0 },
                        { 1, 1, 1 }
                    },
                    ShapeColor = Color.Orange // Forme L (orange)
                },
                new Shape {
                    Width = 3,
                    Height = 2,
                    Dots = new int[,]
                    {
                        { 1, 1, 0 },
                        { 0, 1, 1 }
                    },
                    ShapeColor = Color.Yellow // Forme S 
                },
                new Shape {
                    Width = 3,
                    Height = 2,
                    Dots = new int[,]
                    {
                        { 0, 1, 1 },
                        { 1, 1, 0 }
                    },
                    ShapeColor = Color.Cyan // Forme Z 
                }
            };
        }

        // Retourne une forme aléatoire du tableau
        public static Shape GetRandomShape()
        {
            var shape = shapesArray[new Random().Next(shapesArray.Length)];
            return shape;
        }
    }
}