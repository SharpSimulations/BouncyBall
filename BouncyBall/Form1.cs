﻿using System;
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
        private int m_MaxBallRadius = 50;
        private int m_MaxVelocity = 20;
        private const int m_Frequency = 10;
        private const decimal m_UpdateTime = 1000 / m_Frequency;
        private int m_MaxNumberOfBalls = 15;
        private int counter = 0;

        
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
           
                balls.Add(new Ball(rnd.Next(1, m_MaxBallRadius), 
                                rnd.Next(1, rnd.Next(1, m_MaxVelocity)),  //random velocity x component
                                rnd.Next(1, rnd.Next(1, m_MaxVelocity)),  //random velocity y component
                                 //random center point. make it though inside the rectangle which is offset from the application window
                                 //by the maximum possible ball radius. That way we can guarranty that no ball will start with the perimeter
                                 //outside the bounds of the application window
                                new Point(rnd.Next(m_MaxBallRadius, ClientSize.Width - m_MaxBallRadius), rnd.Next(m_MaxBallRadius, ClientSize.Height - m_MaxBallRadius)), 
                                Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)))); //random color.
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
            //check colisions between balls. for each ball pairs
            //
            for (int i = 0; i < m_MaxNumberOfBalls; i++)
            {
                for (int j = i + 1; j < m_MaxNumberOfBalls; j++)
                {
                    if (CheckCollisionBetweenBalls(balls[i], balls[j]))
                    {
                        Console.WriteLine("Col between ball {0} and ball {1}", i, j);
                    }
                }
            }
            Refresh();
        }

        private bool CheckCollisionBetweenBalls(Ball ball1, Ball ball2)
        {
            counter++;
            // If the distance between two centers is equal to or less than the sum of both radii,
            //the two circles are colliding and need to bounce off each other.
            double x = Math.Pow((ball1.GetRadius() + ball2.GetRadius()),2);
            return (x >= DistanceBetweenTwoBallsSquared(ball1, ball2));
        }

        private double DistanceBetweenTwoBallsSquared(Ball ball1, Ball ball2)
        {
            //distance between 2 circles are derived by the equation
            //d = Squrt((x2-x1)^2 + (y2-y1)^2)
            //but iam not using the sqrt, so d^2 = (x2-x1)^2 + (y2-y1)^2
            return Math.Pow((ball2.GetCenterPosition().X - ball1.GetCenterPosition().X), 2) + Math.Pow((ball2.GetCenterPosition().Y - ball1.GetCenterPosition().Y), 2);
        }
    }
}
