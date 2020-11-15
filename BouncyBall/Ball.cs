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
        private int m_BallRadius;
        private int m_BallWidth;
        private int m_BallHeight;
        private int m_BallposX, m_BallposY;   // Position.
        private int m_BallVelX, m_BallVelY; // Velocity.
        private Color m_Color;

        public Ball(int ballDiameter, int ballVelX, int ballVelY, int ballPosX, int ballPosY)
        {
            m_BallRadius = ballDiameter / 2;
            m_BallDiameter = ballDiameter;
            m_BallWidth = m_BallHeight = ballDiameter;
            m_BallposX = ballPosX;
            m_BallposY = ballPosY;
            m_BallVelX = ballVelX;
            m_BallVelY = ballVelY;
            m_Color = Color.FromArgb(r.Next(0, 256), r.Next(0, 256), r.Next(0, 256)); ;

        }

        public void DrawBall(Graphics graphics)
        {
            Point end = new Point(m_BallVelX + GetCenterPosition().X, m_BallVelY + GetCenterPosition().Y);
            graphics.FillEllipse(Brushes.Red, m_BallposX, m_BallposY, m_BallWidth, m_BallHeight);
            graphics.DrawEllipse(Pens.Black, m_BallposX, m_BallposY, m_BallWidth, m_BallHeight);
            graphics.DrawLine(Pens.Black, GetCenterPosition(), end); //draw vector
        }


        public void UpdatePosition(int boundingBoxWidth, int boundingBoxHeight)
        {
            m_BallposX += m_BallVelX; //update the position
            if (m_BallposX < 0)
            {
                m_BallVelX = -m_BallVelX;
            }
            else if (m_BallposX + m_BallWidth > boundingBoxWidth)
            {
                m_BallVelX = -m_BallVelX;
            }

            m_BallposY += m_BallVelY;
            if (m_BallposY < 0)
            {
                m_BallVelY = -m_BallVelY;
            }
            else if (m_BallposY + m_BallHeight > boundingBoxHeight)
            {
                m_BallVelY = -m_BallVelY;
            }
        }

        public int GetRadius()
        {
            return m_BallRadius;
        }

        public Point GetCenterPosition()
        {   
            return new Point(m_BallposX + m_BallRadius, m_BallposY + m_BallRadius);
        }
    }
}
