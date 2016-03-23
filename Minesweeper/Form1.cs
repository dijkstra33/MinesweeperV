using System;
using System.Drawing;
using System.Windows.Forms;

internal enum CellType
{
    Closed,
    Empty,
    Number,
    Flag,
    Mine,
    Entered,
    Left
}

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private const int CellsAmount = 10;
        private const int MinesAmount = 10;
        private const int CellSize = 50;
        private Point[] mineIndex = new Point[MinesAmount];
        private Point currentCursorPosition;
        private Random random = new Random();
        private Timer timer1 = new Timer();
        private CellType[][] board = new CellType[CellsAmount][];
        private int[][] Cells = new int[CellsAmount][];
        private bool[][] used = new bool[CellsAmount][];

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(CellsAmount * CellSize + 2, CellsAmount * CellSize + 2);
            pictureBox1.Size = new Size(CellsAmount * CellSize + 2, CellsAmount * CellSize + 2);
            pictureBox1.Refresh();
            for (int i = 0; i < CellsAmount; i++)
            {
                board[i] = new CellType[CellsAmount];
                Cells[i] = new int[CellsAmount];
                used[i] = new bool[CellsAmount];
            }
            Restart();
        }

        private void DrawLines(PaintEventArgs e)
        {
            Pen LinePen = Pens.Black;
            for (int i = 0; i < CellsAmount + 1; i++)
            {
                e.Graphics.DrawLine(LinePen, 0, i * CellSize, CellsAmount * CellSize, i * CellSize);
                e.Graphics.DrawLine(LinePen, i * CellSize, 0, i * CellSize, CellsAmount * CellSize);
            }
        }

        private void CheckEmpty(int indexY, int indexX)
        {
            if (used[indexY][indexX] == false &&
                Cells[indexY][indexX] == 0)
            {
                used[indexY][indexX] = true;
                if (indexY > 0)
                {
                    CheckEmpty(indexY - 1, indexX);
                }
                if (indexX > 0)
                {
                    CheckEmpty(indexY, indexX - 1);
                }
                if (indexY < CellsAmount - 1)
                {
                    CheckEmpty(indexY + 1, indexX);
                }
                if (indexX < CellsAmount - 1)
                {
                    CheckEmpty(indexY, indexX + 1);
                }
                board[indexY][indexX] = CellType.Empty;
            }
        }

        private void DrawCells(PaintEventArgs e)
        {
            Brush ClosedCellBrush = Brushes.Aqua;
            Brush EmptyCellBrush = Brushes.AliceBlue;
            Brush EnteredCellBrush = Brushes.DarkCyan;
            Brush MinesBrush = Brushes.DarkRed;
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (board[i][j] == CellType.Empty)
                    {
                        e.Graphics.FillRectangle(EmptyCellBrush, j * CellSize + 1, i * CellSize + 1, CellSize - 1,
                            CellSize - 1);
                    }
                    if (board[i][j] == CellType.Entered)
                    {
                        e.Graphics.FillRectangle(EnteredCellBrush, j * CellSize + 1, i * CellSize + 1, CellSize - 1,
                            CellSize - 1);
                    }
                    if (board[i][j] == CellType.Closed)
                    {
                        e.Graphics.FillRectangle(ClosedCellBrush, j * CellSize + 1, i * CellSize + 1, CellSize - 1,
                            CellSize - 1);
                    }
                    if (board[i][j] == CellType.Mine)
                    {
                        e.Graphics.FillRectangle(ClosedCellBrush, j * CellSize + 1, i * CellSize + 1, CellSize - 1,
                            CellSize - 1);
                    }
                    if (board[i][j] == CellType.Mine)
                    {
                        e.Graphics.FillRectangle(MinesBrush, j * CellSize + 1, i * CellSize + 1, CellSize - 1, CellSize - 1);
                    }
                }
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawLines(e);
            DrawCells(e);
        }

        private void Restart()
        {
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    Cells[i][j] = 0;
                    board[i][j] = CellType.Closed;
                    used[i][j] = false;
                }
            }
            for (int i = 0; i < MinesAmount; i++)
            {
                mineIndex[i].X = random.Next(0, MinesAmount);
                mineIndex[i].Y = random.Next(0, MinesAmount);
                Cells[mineIndex[i].Y][mineIndex[i].X] = -1;
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            currentCursorPosition.X = e.X / CellSize;
            currentCursorPosition.Y = e.Y / CellSize;
            CheckEmpty(currentCursorPosition.Y, currentCursorPosition.X);
            if (Cells[currentCursorPosition.Y][currentCursorPosition.X] == -1)
            {
                for (int i = 0; i < MinesAmount; i++)
                {
                    board[mineIndex[i].Y][mineIndex[i].X] = CellType.Mine;
                }
                pictureBox1.Refresh();
                MessageBox.Show("You lost =(");
                Restart();
            }
            pictureBox1.Refresh();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            Point lastCellHovered = new Point();
            if (e.X / CellSize < CellsAmount && e.Y / CellSize < CellsAmount &&
                board[e.Y / CellSize][e.X / CellSize] == CellType.Closed)
            {
                board[e.Y / CellSize][e.X / CellSize] = CellType.Entered;
                lastCellHovered.X = e.X / CellSize;
                lastCellHovered.Y = e.Y / CellSize;
                pictureBox1.Refresh();
                board[lastCellHovered.Y][lastCellHovered.X] = CellType.Closed;
            }
        }
    }
}