using System;
using System.Collections.Generic;
using System.Drawing;
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
        private Point currentCursorPosition;
        private Point lastCellHovered = new Point();
        private Random random = new Random();
        private Timer timer1 = new Timer();
        private CellState[][] board = new CellState[CellsAmount][];
        private int[][] Cells = new int[CellsAmount][];
        private bool[][] used = new bool[CellsAmount][];

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            pictureBox1.Size = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            pictureBox1.Refresh();
            for (int i = 0; i < CellsAmount; i++)
            {
                board[i] = new CellState[CellsAmount];
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
                e.Graphics.DrawLine(LinePen, 0, i*CellSize, CellsAmount*CellSize, i*CellSize);
                e.Graphics.DrawLine(LinePen, i*CellSize, 0, i*CellSize, CellsAmount*CellSize);
            }
        }

        private void CheckMinesAround(int indexY, int indexX)
        {
            int k = 0;
            if (used[indexY][indexX] == false &&
                Cells[indexY][indexX] != -1)
            {
                used[indexY][indexX] = true;
                for(int i = -1; i <= 1; i++)
                    for(int j = -1; j <= 1; j++)
                        if(IsThere(indexY + i, indexX + j, -1)) k++;
                Cells[indexY][indexX] = k;
                //Console.WriteLine(Cells[indexY][indexX]);
                if (indexY > 0)
                {
                    CheckMinesAround(indexY - 1, indexX);
                }
                if (indexX > 0)
                {
                    CheckMinesAround(indexY, indexX - 1);
                }
                if (indexY < CellsAmount - 1)
                {
                    CheckMinesAround(indexY + 1, indexX);
                }
                if (indexX < CellsAmount - 1)
                {
                    CheckMinesAround(indexY, indexX + 1);
                }
            }
        }

        private bool IsThere(int indexY, int indexX, int checkNumber)
        {
            if (indexY >= 0 &&
                indexX >= 0 &&
                indexY < CellsAmount &&
                indexX < CellsAmount &&
                Cells[indexY][indexX] == checkNumber)
                return true;
            else return false;
        }

        private void CheckOpened(int indexY, int indexX)
        {
            if (used[indexY][indexX] == false &&
                Cells[indexY][indexX] == 0)
            {
                used[indexY][indexX] = true;
                board[indexY][indexX] = CellState.Opened;
                for (int i = -1; i <= 1; i++)
                    for (int j = -1; j <= 1; j++)
                        if (IsThere(indexY + i, indexX + j, 0)) CheckOpened(indexY + i, indexX + j);
                        else 
                            if(indexY + i >= 0 &&
                               indexX + j >= 0 &&
                               indexY + i < CellsAmount &&
                               indexX + j < CellsAmount)
                                    board[indexY + i][indexX + j] = CellState.Opened;
            }
            /*else if (used[indexY][indexX] == false && Cells[indexY][indexX] != -1)
                board[indexY][indexX] = CellState.Opened;*/
        }

        /*if (indexY > 0)
                {
                    CheckOpened(indexY - 1, indexX);
                }
                if (indexX > 0)
                {
                    CheckOpened(indexY, indexX - 1);
                }
                if (indexY < CellsAmount - 1)
                {
                    CheckOpened(indexY + 1, indexX);
                }
                if (indexX < CellsAmount - 1)
                {
                    CheckOpened(indexY, indexX + 1);
                }
                if (indexY > 0 &&
                    indexX > 0 &&
                    indexY < CellsAmount - 1 &&
                    indexX < CellsAmount - 1)
                {
                    CheckOpened(indexY - 1, indexX - 1);
                    CheckOpened(indexY - 1, indexX + 1);
                    CheckOpened(indexY + 1, indexX - 1);
                    CheckOpened(indexY + 1, indexX + 1);
                }*/

        private void DrawCells(PaintEventArgs e)
        {
            Dictionary<CellState, Brush> d = new Dictionary<CellState, Brush>()
            {
                { CellState.Closed, Brushes.Aqua}, 
                //{ CellState.Opened, Brushes.AliceBlue}, 
                { CellState.Flag, Brushes.BlueViolet}, 
                { CellState.Mine, Brushes.DarkRed}
            };
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (Cells[i][j] != -1 && board[i][j] == CellState.Opened)
                    {
                        Brush textBrush = Brushes.Black;
                        //Font numberFont = new Font(CellSize);
                        e.Graphics.DrawString(Cells[i][j].ToString(), DefaultFont, textBrush, CellSize*j, CellSize*i);
                    }
                    else
                        DrawCell(e.Graphics, d[board[i][j]], i, j);
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
            while(k < MinesAmount)
            {
                mineIndex[k].X = random.Next(0, CellsAmount);
                mineIndex[k].Y = random.Next(0, CellsAmount);
                if (Cells[mineIndex[k].Y][mineIndex[k].X] != -1)
                {
                    Cells[mineIndex[k].Y][mineIndex[k].X] = -1;
                    k++;
                }
            }
            CheckMinesAround(CellsAmount/2, CellsAmount/2);
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    used[i][j] = false;
                }
            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            currentCursorPosition.X = e.X/CellSize;
            currentCursorPosition.Y = e.Y/CellSize;
            if ((e.Button & MouseButtons.Left) != 0)
            {
                if (board[currentCursorPosition.Y][currentCursorPosition.X] != CellState.Flag)
                {
                    if (Cells[currentCursorPosition.Y][currentCursorPosition.X] == -1)
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
                        board[currentCursorPosition.Y][currentCursorPosition.X] = CellState.Opened;
                        CheckOpened(currentCursorPosition.Y, currentCursorPosition.X);
                    }
                }
                int k = 0;
                for (int i = 0; i < CellsAmount; i++)
                    for (int j = 0; j < CellsAmount; j++)
                    {
                        if (board[i][j] == CellState.Closed ||
                            board[i][j] == CellState.Flag)
                            k++;
                    }
                if (MinesAmount == k)
                {
                    lastCellHovered.X = -1;
                    lastCellHovered.Y = -1;
                    pictureBox1.Refresh();
                    MessageBox.Show("You win! =)");
                    Restart();
                }
                pictureBox1.Refresh();
        }
            if ((e.Button & MouseButtons.Right) != 0)
            {
                if (board[currentCursorPosition.Y][currentCursorPosition.X] == CellState.Closed)
                    board[currentCursorPosition.Y][currentCursorPosition.X] = CellState.Flag;
                else
                {
                    if (board[currentCursorPosition.Y][currentCursorPosition.X] == CellState.Flag)
                        board[currentCursorPosition.Y][currentCursorPosition.X] = CellState.Closed;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            lastCellHovered.X = -1;
            lastCellHovered.Y = -1;
            if (e.X/CellSize < CellsAmount && e.Y/CellSize < CellsAmount && board[e.Y/CellSize][e.X/CellSize] == CellState.Closed)
            {
                lastCellHovered.X = e.X/CellSize;
                lastCellHovered.Y = e.Y/CellSize;
            }
            pictureBox1.Refresh();
        }
    }
}