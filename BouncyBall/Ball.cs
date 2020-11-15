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
        private int m_BallDiameter;
        private float m_BallRadius;
        private float m_BallWidth;
        private float m_BallHeight;
        private Point m_CenterPoint;
        private int m_BallVelX, m_BallVelY;   // Velocity.
        private double m_Volume;
        private double m_Mass;
        private const int m_Density = 1;
        private double m_Momentum;
        private Color m_Color;

        public Ball(float ballRadius, int ballVelX, int ballVelY, Point centerPoint, Color color)
        {
            m_BallRadius = ballRadius;
            m_BallWidth = m_BallHeight = ballRadius * 2;
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

        public void DrawBall(Graphics graphics)
        {
            Point end = new Point(m_BallVelX + GetCenterPosition().X, m_BallVelY + GetCenterPosition().Y);
            //There is not a way to dray a circle directly, merely an eclipse, and the way that it is drown is using
            //the bounding rectangle. Since now we have the centre point of the circle, and the radius we need to
            //create a corresponding rectange. Use a helper function to abstract this away.
            graphics.FillEllipse(new SolidBrush(m_Color), Circle2RectangleF(m_CenterPoint , m_BallRadius));
            graphics.DrawEllipse(Pens.Black, Circle2RectangleF(m_CenterPoint, m_BallRadius));
            graphics.DrawLine(Pens.Black, GetCenterPosition(), end); //the vector of the velocity
        }


        RectangleF Circle2RectangleF(Point centerPoint, float radius)
        {
            //a square with a circle incribed has edge length =  2 * radius  and starts
            //on the apper corner
            return new RectangleF(centerPoint.X - radius,
                                  centerPoint.Y - radius,
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

        public float GetRadius()
        {
            return m_BallRadius;
        }

        public float GetDiameter()
        {
            return m_BallRadius * 2;
        }

        public Point GetCenterPosition()
        {
            return m_CenterPoint;
        }
    }
}
