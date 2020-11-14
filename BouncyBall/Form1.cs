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
        List<Ball> balls = new List<Ball>();
        private int m_MaxBallSize = 100;
        private int m_MaxVelocity = 30;
        private const int m_Frequency = 60;
        private const decimal m_UpdateTime = 1000 / m_Frequency;
        private int m_MaxNumberOfBalls = 4;

        
        public Form1()
        {
            InitializeComponent();
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            //for smooth update
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.Clear(BackColor);
            foreach (Ball ball in balls)
            { 
                ball.DrawBall(e.Graphics);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Random rnd = new Random();
            // On load Pick a random start position and velocity.
            for (int i = 0; i < m_MaxNumberOfBalls; i++)
            { 
                int ballSize = rnd.Next(1, m_MaxBallSize);
            
                balls.Add(new Ball(ballSize, rnd.Next(1, rnd.Next(1, m_MaxVelocity)), 
                                            rnd.Next(1, rnd.Next(1, m_MaxVelocity)), 
                                            rnd.Next(0, ClientSize.Width - ballSize), 
                                            rnd.Next(0, ClientSize.Height - ballSize)));
            }
            
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
            foreach (Ball ball in balls)
            {
                ball.UpdatePosition(ClientSize.Width, ClientSize.Height);
            }
            Refresh();
        }
    }
}
