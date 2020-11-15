using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouncyBall
{
    class Vector2
    {
        private Point m_Origin;
        private double m_Direction;
        private double m_Magnitude;

        public Vector2(Point origin, double directionAngleDegrees, double magnitude)
        {
            m_Origin = origin;
            m_Direction = directionAngleDegrees;
            m_Magnitude = magnitude;
        }


        public Vector2(Point origin, Point end)
        {
            m_Origin = origin;
            //distance between two points = Sqrt((X2-X1)^2 + (Y2-Y1)^2))
            m_Magnitude = Math.Sqrt(Math.Pow((end.X - origin.X), 2) + Math.Pow((end.Y - origin.Y), 2));
            //angle can be found with the inverse tangent of y/y. then convert to degrees
            m_Direction = Math.Atan2(end.Y - origin.Y, end.X - origin.X) * (180 / Math.PI);
        }


        public double GetMagnitudeX()
        {
            return m_Magnitude * Math.Cos(m_Direction);
        }


        public double GetMagnitudeY()
        {
            return m_Magnitude * Math.Sin(m_Direction);
        }


        public double GetMagnitude()
        {
            return m_Magnitude;
        }


        public Point GetOrigin()
        {
            return m_Origin;
        }


        public double GetDirectionDegrees()
        {
            return m_Direction;
        }
    }

    
}
