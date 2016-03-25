using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

internal enum CellState
{
    Closed,
    Opened,
    Flag,
    Mine
}

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private const int CellsAmount = 9;
        private const int MinesAmount = 10;
        private const int CellSize = 50;
        private Point[] mineIndex = new Point[MinesAmount];
        private Point clickedCell;

        private Brush[] brushArray = new Brush[9]
        {
            Brushes.Black,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Red,
            Brushes.DarkRed,
            Brushes.DarkRed,
            Brushes.DarkRed,
            Brushes.DarkRed,
            Brushes.DarkRed
        };
        private Point lastCellHovered = new Point();
        private Random random = new Random();
        private CellState[][] board = new CellState[CellsAmount][];
        private int[][] Cells = new int[CellsAmount][];
        private bool[][] used = new bool[CellsAmount][];

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            pictureBox1.Size = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            for (int i = 0; i < CellsAmount; i++)
            {
                board[i] = new CellState[CellsAmount];
                Cells[i] = new int[CellsAmount];
                used[i] = new bool[CellsAmount];
            }
            Restart();
        }

        private void Restart()
        {
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    Cells[i][j] = 0;
                    board[i][j] = CellState.Closed;
                    used[i][j] = false;
                }
            }
            int k = 0;
            while (k < MinesAmount)
            {
                mineIndex[k].X = random.Next(0, CellsAmount);
                mineIndex[k].Y = random.Next(0, CellsAmount);
                if (Cells[mineIndex[k].Y][mineIndex[k].X] != -1)
                {
                    Cells[mineIndex[k].Y][mineIndex[k].X] = -1;
                    k++;
                }
            }
            CheckMinesAround();
        }

        private void DrawLines(PaintEventArgs e)
        {
            Pen linePen = Pens.Black;
            for (int i = 0; i < CellsAmount + 1; i++)
            {
                e.Graphics.DrawLine(linePen, 0, i*CellSize, CellsAmount*CellSize, i*CellSize);
                e.Graphics.DrawLine(linePen, i*CellSize, 0, i*CellSize, CellsAmount*CellSize);
            }
        }

        private void CheckMinesAround()
        {
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    if(Cells[i][j] != -1)
                    {
                        int k = 0;
                        for (int p = -1; p <= 1; p++)  //Todo: 2 for's into method
                        {
                            for (int q = -1; q <= 1; q++)
                            {
                                if (IsThere(i + p, j + q, -1))
                                    k++;
                            }
                            Cells[i][j] = k;
                        }
                    }
                }
            }
        }

        private bool IsThere(int indexY, int indexX, int checkNumber)
        {
            return indexY >= 0 &&
                   indexX >= 0 &&
                   indexY < CellsAmount &&
                   indexX < CellsAmount &&
                   Cells[indexY][indexX] == checkNumber;
        }

        private void CheckOpened(int indexY, int indexX)
        {
            if (!used[indexY][indexX] &&
                Cells[indexY][indexX] == 0)
            {
                used[indexY][indexX] = true;
                board[indexY][indexX] = CellState.Opened;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (IsThere(indexY + i, indexX + j, 0))
                            CheckOpened(indexY + i, indexX + j);
                        else 
                            if (indexY + i >= 0 &&            //Todo: add a method if possible
                                indexX + j >= 0 &&
                                indexY + i < CellsAmount &&
                                indexX + j < CellsAmount)
                                    board[indexY + i][indexX + j] = CellState.Opened;
                    }
                }
            }
        }

        private void DrawCells(PaintEventArgs e)
        {
            Font numberFont = new Font(new FontFamily(GenericFontFamilies.SansSerif), 14);
            StringFormat stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            Dictionary<CellState, Brush> brushes = new Dictionary<CellState, Brush>()
            {
                { CellState.Closed, Brushes.Aqua},
                { CellState.Opened, Brushes.AliceBlue},
                { CellState.Flag, Brushes.BlueViolet}, 
                { CellState.Mine, Brushes.DarkRed}
            };
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (Cells[i][j] > 0 && board[i][j] == CellState.Opened)
                    {
                        Brush textBrush = brushArray[Cells[i][j]];
                        e.Graphics.DrawString(Cells[i][j].ToString(), numberFont, textBrush,
                            new RectangleF(CellSize*j, CellSize*i, CellSize, CellSize), stringFormat);
                    }
                    else
                        DrawCell(e.Graphics, brushes[board[i][j]], i, j);
                }
            }
            if (lastCellHovered.X != -1 && lastCellHovered.Y != -1)
                DrawCell(e.Graphics, Brushes.DarkCyan, lastCellHovered.Y, lastCellHovered.X);
        }

        private void DrawCell(Graphics graphics, Brush brush, int i, int j)
        {
            graphics.FillRectangle(brush, j*CellSize + 1, i*CellSize + 1, CellSize - 1, CellSize - 1);
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            DrawLines(e);
            DrawCells(e);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            clickedCell.X = e.X/CellSize;
            clickedCell.Y = e.Y/CellSize;
            if ((e.Button & MouseButtons.Left) != 0)
            {
                if (board[clickedCell.Y][clickedCell.X] != CellState.Flag)
                {
                    if (Cells[clickedCell.Y][clickedCell.X] == -1)
                    {
                        lastCellHovered.Y = -1;
                        lastCellHovered.X = -1;
                        for (int i = 0; i < MinesAmount; i++)
                        {
                            board[mineIndex[i].Y][mineIndex[i].X] = CellState.Mine;
                        }
                        pictureBox1.Refresh();
                        MessageBox.Show("You lost =(");
                        Restart();
                    }
                    else
                    {
                        board[clickedCell.Y][clickedCell.X] = CellState.Opened;
                        CheckOpened(clickedCell.Y, clickedCell.X);
                    }
                }
                int cellsLeft = 0;
                for (int i = 0; i < CellsAmount; i++)
                    for (int j = 0; j < CellsAmount; j++)
                    {
                        if (board[i][j] == CellState.Closed ||
                            board[i][j] == CellState.Flag)
                            cellsLeft++;
                    }
                if (MinesAmount == cellsLeft)
                {
                    lastCellHovered.X = -1;
                    lastCellHovered.Y = -1;
                    pictureBox1.Refresh();
                    MessageBox.Show("You win! =)");
                    Restart();
                }
            }
            if ((e.Button & MouseButtons.Right) != 0)
            {
                switch (board[clickedCell.Y][clickedCell.X])
                {
                    case CellState.Closed:
                        board[clickedCell.Y][clickedCell.X] = CellState.Flag;
                        break;
                    case CellState.Flag:
                        board[clickedCell.Y][clickedCell.X] = CellState.Closed;
                        break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            lastCellHovered.X = -1;
            lastCellHovered.Y = -1;
            int cellX = e.X / CellSize;
            int cellY = e.Y / CellSize;
            if (cellX < CellsAmount && cellY < CellsAmount && board[cellY][cellX] == CellState.Closed)
            {
                lastCellHovered.X = cellX;
                lastCellHovered.Y = cellY;
            }
            pictureBox1.Refresh();
        }
    }
}