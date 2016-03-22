using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

enum CellType
{
    closed,
    empty,
    one,
    two,
    three,
    four,
    five,
    six,
    seven,
    eight,
    flag,
    mine,
    entered,
    left
}

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        const int CellsAmount = 10;
        const int MinesAmount = 10;
        const int CellSize = 50;
        private Point[] mineIndex = new Point[MinesAmount];
        private Point currentCursorPosition = new Point();
        Random random = new Random();
        Timer timer1 = new Timer();
        CellType[][] board = new CellType[CellsAmount][];

        public Form1()
        {
            InitializeComponent();
            //Timer.Initialize();
            ClientSize = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            pictureBox1.Size = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            pictureBox1.Refresh();
            for (int i = 0; i < CellsAmount; i++)
            {
                board[i] = new CellType[CellsAmount];
            }
        }

        private void DrawLines(PaintEventArgs e)
        {
            int dx = 0;
            int dy = 0;
            Pen LinePen = Pens.Black;
            for (int i = 0; i < CellsAmount + 1; i++)
            {
                e.Graphics.DrawLine(LinePen, 0, dy, CellsAmount*CellSize, dy);
                e.Graphics.DrawLine(LinePen, dx, 0, dx, CellsAmount*CellSize);
                dx += CellSize;
                dy += CellSize;
            }
        }

        private void DrawCells(PaintEventArgs e)
        {
            int dx = 0;
            int dy = 0;
            Brush ClosedCellBrush = Brushes.Aqua;
            Brush EmptyCellBrush = Brushes.AliceBlue;
            Brush EnteredCellBrush = Brushes.DarkCyan;
            for (int i = 0; i < CellsAmount; i++)
            {
                dx = 0;
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (board[i][j] == CellType.empty)
                    {
                        e.Graphics.FillRectangle(EmptyCellBrush, dx + 1, dy + 1, CellSize - 1, CellSize - 1);
                        dx += CellSize;
                    }
                    if (board[i][j] == CellType.entered)
                    {
                        e.Graphics.FillRectangle(EnteredCellBrush, dx + 1, dy + 1, CellSize - 1, CellSize - 1);
                        dx += CellSize;
                    }
                    {
                        e.Graphics.FillRectangle(ClosedCellBrush, dx + 1, dy + 1, CellSize - 1, CellSize - 1);
                        dx += CellSize;
                    }
                }
                dy += CellSize;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawLines(e);
            DrawCells(e);
        }

        private void Restart()
        {
            for (int i = 0; i < MinesAmount; i++)
            {
                mineIndex[i].X = random.Next(0, MinesAmount);
                mineIndex[i].Y = random.Next(0, MinesAmount);
            }
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    board[i][j] = CellType.closed;
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            currentCursorPosition.X = e.X/CellSize;
            currentCursorPosition.Y = e.Y/CellSize;
            board[currentCursorPosition.Y][currentCursorPosition.X] = CellType.empty;
            pictureBox1.Refresh();
        }

        /*private void pictureBox1_Move(object sender, EventArgs e)
        {
            
        }*/

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            currentCursorPosition.X = e.X / CellSize;
            currentCursorPosition.Y = e.Y / CellSize;
            Point lastCellHovered = new Point();
            if (currentCursorPosition.Y < CellsAmount && currentCursorPosition.X < CellsAmount)
            {
                board[currentCursorPosition.Y][currentCursorPosition.X] = CellType.entered;
                lastCellHovered.X = currentCursorPosition.X;
                lastCellHovered.Y = currentCursorPosition.Y;
            }
            pictureBox1.Refresh();
            board[lastCellHovered.Y][lastCellHovered.X] = CellType.closed;
        }
    }
}
