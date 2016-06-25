using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;

internal enum CellState
{
    Closed,
    Opened,
    Flag
}

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private const int MaxCellsAmount = 50;
        public int CellsAmount = 9;
        public int MinesAmount = 10;
        public int CellSize = 50;
        private Point[] mineIndex = new Point[MaxCellsAmount];
        private Point clickedCell;
        private int timePlayed = 0;
        private int clickedButtons = 0;

        private Brush[] brushArray = new Brush[9]
        {
            Brushes.Black,
            Brushes.Blue,
            Brushes.Green,
            Brushes.Red,
            Brushes.DarkBlue,
            Brushes.DarkRed,
            Brushes.DarkRed,
            Brushes.DarkRed,
            Brushes.DarkRed
        };

        private Point lastCellHovered;
        private Random random = new Random();
        private CellState[][] board = new CellState[MaxCellsAmount][];
        private int[][] Cells = new int[MaxCellsAmount][];
        private bool[][] used = new bool[MaxCellsAmount][];
        private Image mineImage = Properties.Resources.mine;
        private Image flagImage = Properties.Resources.flag;

        public Form1()
        {
            InitializeComponent();
            ClientSize = new Size(CellsAmount*CellSize + 2 + 20,
                CellsAmount*CellSize + 2 + menuStrip2.Height + minesLeftLabel.Height + RestartButton.Height + 10);
            pictureBox1.Size = new Size(CellsAmount*CellSize + 2, CellsAmount*CellSize + 2);
            pictureBox1.Location = new Point(10, menuStrip2.Height + minesLeftLabel.Height);
            minesLeftLabel.Location = new Point(10, menuStrip2.Height);
            timeLabel.Location = new Point(CellsAmount*CellSize + 2 - timeLabel.Width, menuStrip2.Height);
            RestartButton.Location = new Point(ClientSize.Width / 2 - RestartButton.Width / 2, CellsAmount * CellSize + 2 + menuStrip2.Height + minesLeftLabel.Height + 10);

            for (int i = 0; i < CellsAmount; i++)
            {
                board[i] = new CellState[MaxCellsAmount];
                Cells[i] = new int[MaxCellsAmount];
                used[i] = new bool[MaxCellsAmount];
            }
            Restart();
        }

        private void Restart()
        {
            timer1.Enabled = false;
            timeLabel.Text = "00:00:00";
            timePlayed = 0;
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
            pictureBox1.Refresh();
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
                    if (Cells[i][j] != -1)
                    {
                        Cells[i][j] = CountMinesAround(i, j);
                    }
                }
            }
        }

        private int CountMinesAround(int indexY, int indexX)
        {
            int k = 0;
            for (int p = -1; p <= 1; p++)
            {
                for (int q = -1; q <= 1; q++)
                {
                    if (IsThere(indexY + p, indexX + q, -1))
                        k++;
                }
            }
            return k;
        }

        private bool IsThere(int indexY, int indexX, int checkNumber)
        {
            return InBoard(indexY, indexX) &&
                   Cells[indexY][indexX] == checkNumber;
        }

        private void CheckOpened(int indexY, int indexX)
        {
            if (!used[indexY][indexX] &&
                Cells[indexY][indexX] == 0)
            {
                used[indexY][indexX] = true;
                if(board[indexY][indexX] != CellState.Flag) board[indexY][indexX] = CellState.Opened;
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (IsThere(indexY + i, indexX + j, 0))
                            CheckOpened(indexY + i, indexX + j);
                        else if (InBoard(indexY + i, indexX + j))
                            board[indexY + i][indexX + j] = CellState.Opened;
                    }
                }
            }
        }

        private bool InBoard(int indexY, int indexX)
        {
            return indexY >= 0 &&
                   indexX >= 0 &&
                   indexY < CellsAmount &&
                   indexX < CellsAmount;
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
                {CellState.Closed, Brushes.Aqua},
                {CellState.Opened, Brushes.AliceBlue},
                {CellState.Flag, Brushes.DarkCyan}
            };
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (board[i][j] == CellState.Opened)
                    {
                        if (Cells[i][j] > 0)
                        {
                            Brush textBrush = brushArray[Cells[i][j]];
                            e.Graphics.DrawString(Cells[i][j].ToString(), numberFont, textBrush,
                                new RectangleF(CellSize*j, CellSize*i, CellSize, CellSize), stringFormat);
                        }
                        if (Cells[i][j] == -1)
                        {
                            e.Graphics.DrawImage(mineImage, j*CellSize, i*CellSize, CellSize, CellSize);
                        }
                    }
                    else
                    {
                        DrawCell(e.Graphics, brushes[board[i][j]], i, j);
                        if (board[i][j] == CellState.Flag)
                            e.Graphics.DrawImage(flagImage, j*CellSize, i*CellSize, CellSize, CellSize);
                    }

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
            MinesLeft();
        }

        private void MinesLeft()
        {
            int minesLeft = MinesAmount;
            for (int i = 0; i < CellsAmount; i++)
            {
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (board[i][j] == CellState.Flag && minesLeft > 0)
                        minesLeft--;
                }
            }
            minesLeftLabel.Text = "Mines left: " + minesLeft;
        }

        private bool CheckVictory()
        {
            int cellsLeft = 0;
            for (int i = 0; i < CellsAmount; i++)
                for (int j = 0; j < CellsAmount; j++)
                {
                    if (board[i][j] == CellState.Closed ||
                        board[i][j] == CellState.Flag)
                        cellsLeft++;
                }
            return (MinesAmount == cellsLeft);
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            /*if (mouseClicks == 2)
            {
                MessageBox.Show("Test");
                mouseClicks--;
            }*/
            timer1.Enabled = true;
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
                            board[mineIndex[i].Y][mineIndex[i].X] = CellState.Opened;
                        }
                        pictureBox1.Refresh();
                        timer1.Stop();
                        MessageBox.Show("You lost =(");
                        Restart();
                    }
                    else
                    {
                        board[clickedCell.Y][clickedCell.X] = CellState.Opened;
                        CheckOpened(clickedCell.Y, clickedCell.X);
                    }
                }
                if (CheckVictory())
                {
                    lastCellHovered.X = -1;
                    lastCellHovered.Y = -1;
                    pictureBox1.Refresh();
                    MessageBox.Show("You won! =)");
                    timer1.Stop();
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
            if ((e.Button & MouseButtons.Middle) != 0)
            {
                SmartClick();
            }
            pictureBox1.Refresh();
        }

        private void SmartClick()
        {
            int k = 0;
            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (InBoard(clickedCell.Y + i, clickedCell.X + j))
                        if (board[clickedCell.Y + i][clickedCell.X + j] == CellState.Flag)
                        {
                            k++;
                        }
                }
            }
            if (Cells[clickedCell.Y][clickedCell.X] == k &&
                board[clickedCell.Y][clickedCell.X] != CellState.Closed &&
                board[clickedCell.Y][clickedCell.X] != CellState.Flag)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (InBoard(clickedCell.Y + i, clickedCell.X + j))
                            if (board[clickedCell.Y + i][clickedCell.X + j] == CellState.Closed)
                            {
                                if (Cells[clickedCell.Y + i][clickedCell.X + j] == -1)
                                {
                                    for (int m = 0; m < MinesAmount; m++)
                                    {
                                        board[mineIndex[m].Y][mineIndex[m].X] = CellState.Opened;
                                    }
                                    pictureBox1.Refresh();
                                    timer1.Stop();
                                    MessageBox.Show("You lost! =(");
                                    Restart();
                                    return;
                                }
                                board[clickedCell.Y + i][clickedCell.X + j] = CellState.Opened;
                                CheckOpened(clickedCell.Y + i, clickedCell.X + j);
                            }
                    }
                }
                if (CheckVictory())
                {
                    lastCellHovered.X = -1;
                    lastCellHovered.Y = -1;
                    pictureBox1.Refresh();
                    timer1.Stop();
                    MessageBox.Show("You won! =)");
                    Restart();
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            int cellX = e.X / CellSize;
            int cellY = e.Y / CellSize;
            if (cellX < CellsAmount &&
                cellY < CellsAmount &&
                cellX >= 0 &&
                cellY >= 0 && 
                board[cellY][cellX] == CellState.Closed)
            {
                lastCellHovered.X = cellX;
                lastCellHovered.Y = cellY;
            }
            else
            {
                lastCellHovered.X = -1;
                lastCellHovered.Y = -1;
            }
            pictureBox1.Refresh();
        }

        private void settingsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var settingsForm = new SettingsForm();
            settingsForm.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timePlayed++;
            int hours = timePlayed / 3600 % 24,
                minutes = timePlayed / 60 % 60, 
                seconds = timePlayed % 60;
            string timeSeconds, timeMinutes, timeHours;
            if (seconds < 10) timeSeconds = 0.ToString() + seconds;
            else timeSeconds = seconds.ToString();
            if (minutes < 10) timeMinutes = 0.ToString() + minutes;
            else timeMinutes = minutes.ToString();
            if (hours < 10) timeHours = 0.ToString() + hours;
            else timeHours = hours.ToString();
            string time = timeHours + ":" + timeMinutes + ":" + timeSeconds;
            timeLabel.Text = time;
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            clickedButtons++;
            clickedCell.X = e.X / CellSize;
            clickedCell.Y = e.Y / CellSize;
            Text = clickedButtons.ToString();
            if (clickedButtons == 2)
            {
                SmartClick();
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            clickedButtons--;
            Text = clickedButtons.ToString();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            lastCellHovered.X = -1;
            lastCellHovered.Y = -1;
            pictureBox1.Refresh();
        }
    }
}

//Дата - Время

//DateTime elapsedTime = new DateTime();

//private void timer1_Tick(object sender, EventArgs e)
//{
//    elapsedTime = elapsedTime.AddSeconds(1);
//    label2.Text = String.Format("{0:H:mm:ss}", elapsedTime);
//}