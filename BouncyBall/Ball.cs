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
        private int m_BallWidth;
        private int m_BallHeight;
        private int m_BallposX, m_BallposY;   // Position.
        private int m_BallVelX, m_BallVelY; // Velocity.

        public Ball(int ballDiameter, int ballVelX, int ballVelY, int ballPosX, int ballPosY)
        {

            m_BallWidth = m_BallHeight = ballDiameter / 2;
            m_BallposX = ballPosX;
            m_BallposY = ballPosY;
            m_BallVelX = ballVelX;
            m_BallVelY = ballVelY;
        }

        public void DrawBall(Graphics graphics)
        {
            graphics.FillEllipse(Brushes.Red, m_BallposX, m_BallposY, m_BallWidth, m_BallHeight);
            graphics.DrawEllipse(Pens.Black, m_BallposX, m_BallposY, m_BallWidth, m_BallHeight);
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
    }
}
