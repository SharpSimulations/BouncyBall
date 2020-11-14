using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BouncyBall
{
    public partial class Form1 : Form
    {
        // Some drawing parameters.
        private const int m_BallWidth = 50;
        private const int m_BallHeight = 50;
        private int m_BallposX, m_BallposY;   // Position.
        private int m_BallVelX, m_BallVelY; // Velocity.
        private int m_InitVelocity = 8;
        private const int m_Frequency = 60;
        private const decimal m_UpdateTime = 1000 / m_Frequency;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //for smooth update
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);
            e.Graphics.FillEllipse(Brushes.Red, m_BallposX, m_BallposY, m_BallWidth, m_BallHeight);
            e.Graphics.DrawEllipse(Pens.Black, m_BallposX, m_BallposY, m_BallWidth, m_BallHeight);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // On load Pick a random start position and velocity.
            Random rnd = new Random();
            m_BallVelX = rnd.Next(1, m_InitVelocity);
            m_BallVelY = rnd.Next(1, m_InitVelocity);
            m_BallposX = rnd.Next(0, ClientSize.Width - m_BallWidth);
            m_BallposY = rnd.Next(0, ClientSize.Height - m_BallHeight);

            // Use double buffering to reduce flicker.
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);
            this.UpdateStyles();

            //timer for updating
            Timer MyTimer = new Timer();
            MyTimer.Interval = ((int)m_UpdateTime); 
            MyTimer.Tick += new EventHandler(MyTimer_Tick);
            MyTimer.Start();
        }

         // Update the ball's position, bouncing if necessary.
        private void MyTimer_Tick(object sender, EventArgs e)
        {
            m_BallposX += m_BallVelX; //update the position
            if (m_BallposX < 0)
            {
                m_BallVelX = -m_BallVelX;
            }
            else if (m_BallposX + m_BallWidth > ClientSize.Width)
            {
                m_BallVelX = -m_BallVelX;
            }

            m_BallposY += m_BallVelY;
            if (m_BallposY < 0)
            {
                m_BallVelY = -m_BallVelY;
            }
            else if (m_BallposY + m_BallHeight > ClientSize.Height)
            {
                m_BallVelY = -m_BallVelY;
            }

            Refresh();
        }
    }
}
