using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace BouncyBall
{
    class Ball
    {
        Random r = new Random();
        private float m_BallRadius;
        private PointD m_CenterPoint;
        private double m_BallVelX, m_BallVelY;   // Velocity.
        private readonly double m_Volume;
        private readonly double m_Mass;
        private const int m_Density = 1;
        private Color m_Color;

        public Ball(float ballRadius, double ballVelX, double ballVelY, PointD centerPoint, Color color)
        {
            m_BallRadius = ballRadius;
            m_CenterPoint = centerPoint;

            m_BallVelX = ballVelX;
            m_BallVelY = ballVelY;
            m_Color = color;

            //calculate the mass from its dimensions
            //first calculate the volume, assume unit thickness
            m_Volume = Math.PI * Math.Pow(m_BallRadius, 2) * 1.0;
            //then calculate the mass
            m_Mass = m_Volume * m_Density;
        }


        public void PrintBallInfo()
        {
            Console.WriteLine("Ball Radius {0}, Ball location {1}x{2}, Ball Mass {3}",
                              m_BallRadius, m_CenterPoint.X, m_CenterPoint.Y, m_Mass);
        }


        public void DrawBall(Graphics graphics)
        {
            //There is not a way to dray a circle directly, merely an eclipse, and the way that it is drown is using
            //the bounding rectangle. Since now we have the centre point of the circle, and the radius we need to
            //create a corresponding rectange. Use a helper function to abstract this away.
            graphics.FillEllipse(new SolidBrush(m_Color), Circle2RectangleF(m_CenterPoint , m_BallRadius));
            graphics.DrawEllipse(Pens.Black, Circle2RectangleF(m_CenterPoint, m_BallRadius));

            //lets also draw the vector of the velocity
            PointD end = new PointD(m_BallVelX + GetCenterPosition().X, m_BallVelY + GetCenterPosition().Y);
            graphics.DrawLine(Pens.Black, GetCenterPosition().ToPoint(), end.ToPoint()); //the vector of the velocity
        }


        RectangleF Circle2RectangleF(PointD centerPoint, float radius)
        {
            //a square with a circle incribed has edge length =  2 * radius  and starts
            //on the apper corner
            return new RectangleF((float)centerPoint.X - radius,
                                  (float)centerPoint.Y - radius,
                                  radius * 2,
                                  radius * 2);
        }

        public void UpdatePosition(int boundingBoxWidth, int boundingBoxHeight)
        {
            m_CenterPoint.X += m_BallVelX; //update the position
            if (m_CenterPoint.X - m_BallRadius < 0) //the perimeter of the circle touched the panel edges
            {
                m_BallVelX = -m_BallVelX; //invert the velocity to simulate the perfect elastic bounce
            }
            else if (m_CenterPoint.X + m_BallRadius > boundingBoxWidth) //the perimeter of the circle touched the panel edges
            {
                m_BallVelX = -m_BallVelX;
            }

            m_CenterPoint.Y += m_BallVelY;
            if (m_CenterPoint.Y - m_BallRadius < 0)
            {
                m_BallVelY = -m_BallVelY;
            }
            else if (m_CenterPoint.Y + m_BallRadius > boundingBoxHeight)
            {
                m_BallVelY = -m_BallVelY;
            }
        }


        public void UpdateVelocityAfterCollision(double newVelX, double newVelY)
        {
            m_BallVelX = newVelX;
            m_BallVelY = newVelY;
        }

        public double GetVelX()
        {
            return m_BallVelX;
        }


        public double GetVelY()
        {
            return m_BallVelY;
        }


        /// <summary>
        /// returns the magnitude of the velocity vector of the ball
        /// </summary>
        /// <returns></returns>
        public double GetVel()
        {
            return Math.Sqrt( Math.Pow(m_BallVelX, 2) + Math.Pow(m_BallVelY, 2));
        }


        /// <summary>
        /// returns the direction of the velocity vector.
        /// </summary>
        /// <returns></returns>
        public double GetDirectionInDegrees()
        {
            //returns value in degrees
            double divisor = Math.PI / 180.0;
            if (m_BallVelX < 0)
            {
                return 180 + Math.Atan2(m_BallVelY, m_BallVelX) / divisor;
            }
            else if ((m_BallVelX > 0) && (m_BallVelY >= 0))
            {
                return Math.Atan2(m_BallVelY, m_BallVelX) / divisor;
            }
            else if ((m_BallVelX < 0) && (m_BallVelY < 0))
            {
                return 360 + Math.Atan2(m_BallVelY, m_BallVelX) / divisor;
            }
            else if ((m_BallVelX == 0) && (m_BallVelY == 0))
            {
                return 0;
            }
            else if ((m_BallVelX == 0) && (m_BallVelY >= 0))
            {
                return 90;
            }
            else
            {
                return 270;
            }
        }


        public float GetRadius()
        {
            return m_BallRadius;
        }

        public float GetDiameter()
        {
            return m_BallRadius * 2;
        }

        public PointD GetCenterPosition()
        {
            return m_CenterPoint;
        }

        public double GetMass()
        {
            return m_Mass;
        }
    }
}
