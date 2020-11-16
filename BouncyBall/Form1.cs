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
        private int m_MaxBallRadius = 50;
        private int m_MaxVelocity = 5;
        private const int m_Frequency = 1000;
        private const decimal m_UpdateTime = 1000 / m_Frequency;
        private const int m_MaxNumberOfBalls = 8;
        private int counter = 0;
        bool[,] m_CollisionTracker = new bool[m_MaxNumberOfBalls, m_MaxNumberOfBalls];


        public Form1()
        {
            InitializeComponent();
            InitArray();
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


        private void InitArray()
        {
            for (int i = 0; i < m_CollisionTracker.GetLength(0); i++)
            {
                for (int j = 0; j < m_CollisionTracker.GetLength(1); j++)
                {
                    m_CollisionTracker[i,j] = false;
                }
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
                                new PointD(rnd.Next(m_MaxBallRadius, ClientSize.Width - m_MaxBallRadius), rnd.Next(m_MaxBallRadius, ClientSize.Height - m_MaxBallRadius)), 
                                Color.FromArgb(rnd.Next(0, 256), rnd.Next(0, 256), rnd.Next(0, 256)))); //random color.
                balls[i].PrintBallInfo();
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
            //ie, if there are 4 balls we should check 1,2 - 1,3 - 1, 4, then 2,3, 2,4, then 3 , 4
            //the total number of pairs are 
            for (int i = 0; i < m_MaxNumberOfBalls; i++)
            {
                for (int j = i + 1; j < m_MaxNumberOfBalls; j++)
                {
                    //if a collision has occured, and it hasn't been handled yet
                    if ((CheckCollisionBetweenBalls(balls[i], balls[j])))// && !m_CollisionTracker[i,j])
                    {
                        //set the flag as handle. keep the flag true until there is not a collision anymore
                        m_CollisionTracker[i, j] = true;
                        Console.WriteLine("Col between ball {0} and ball {1}", i, j);
                        DealWithColliction2D(balls[i], balls[j]);
                    }

                    //if ((!CheckCollisionBetweenBalls(balls[i], balls[j])) && m_CollisionTracker[i, j])
                    //{
                    //    m_CollisionTracker[i, j] = false;
                    //}

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


        private void DealWithColliction2D(Ball ball1, Ball ball2)
        {
            /*
             * How we can deal with colisions?. There is a term called momentum (P)
             * Momentum is defined to be the mass of an object multiplied by the velocity of the object
             * This can be written as vecP = m*vecV, were P and V are vector quantities
             * For 2 objects,  in a collision, the momentum of a system is conserved 
             * (i.e. the total momentum of the system is the same after the collision as 
             * it was before the collision - or as I like to say, what goes in must come out).
             * A "system" consists of the two particles that are colliding. We can write this as
             *  Pinit = Pfinal
             *  
             *  Also if something is moving it has kinetic energy, and if it is not moving it has no kinetic energy.
             *  Κenergy is defined as KE= 1/2 * mass * vel^2. Kinetic energy is not a vector.
             *   In an elastic collision (ie perfect) the kinetic energy is conserved: the initial kinetic energy 
             *   of the system is equal to the final kinetic energy. We now have two conservation equations 
             *   that will let us determine the final velocities of two colliding particles if we know their 
             *   initial velocities and masses
             *   so, mass1_init * vel1_init + mass2_init * vel2_init = mass1_final * vel1_final + mass2_final * vel2_final
             *   and similarly for the kinetic energy
             *   Ke1_init + Ke2_init = Ke1_final + Ke2_final
             *   Using these 2 equations the Vfinal for both 1 & 2 can be found.
             */
            
            DistanceBetweenTwoBallsSquared(ball1, ball2);
            //find the angle of the collision. Imagine a line connecting the centers of the 2 balls.
            //imagine also the X axis. The angle between these 2 straight sections is the collision angle.

            double dx = ball2.GetCenterPosition().X - ball1.GetCenterPosition().X;
            double dy = ball2.GetCenterPosition().Y - ball1.GetCenterPosition().Y;
            double collisionAngleRad;
            if (dx == 0)
            {
                collisionAngleRad = Math.PI / 2;
            }
            else
            {
                collisionAngleRad = Math.Atan2(dy, dx);
            }

            CorrectOverlapping(ball1, ball2, collisionAngleRad);

            //at this point we have the collision angle. We also have the velocity components of each ball in x and y
            //but we also need the actuall angle and magnitude of the velocity vector. 
            //this is very easily done using pythagorean theorem. Since this is a quantity that will be needed all the time
            //it makes more sense to implement the calculation internally on the ball class.

            double vel1 = ball1.GetVel();
            double vel2 = ball2.GetVel();

            double angle1Rad = ball1.GetDirectionInDegrees() * (Math.PI / 180);
            double angle2Rad = ball2.GetDirectionInDegrees() * (Math.PI / 180);

            //now what to do with all these angles. Well in 2d,  conservation of momentum applies to the components 
            //of velocity resolved along the common normal surfaces of the colliding bodies at the point of contact. 
            //In the case of the two spheres the velocity components involved are the components resolved along the 
            //line of centers during the contact. Consequently, the components of velocity perpendicular to the line 
            //of centers will be unchanged during the impact

            //this means the following Here is my translation: If you view the collision along the line between the two spheres,
            //the velocities along that line will undergo momentum conservation (the same way we calculated the x-components in Part 1),
            //and the velocities perpendicular to that line won't change.

            //We first want to change our mind set from the standard x-y reference frame, to the new reference frame where the x-axis lies along the collision line,
            //and the y-axis is perpendicular to that. The figure shows the new vector components

            //so lets find the velocity in the new coordinate system. Lets call the new coordinate system R.
            double vel1xr = vel1 * Math.Cos((angle1Rad - collisionAngleRad));
            double vel1yr = vel1 * Math.Sin((angle1Rad - collisionAngleRad));
            double vel2xr = vel2 * Math.Cos((angle2Rad - collisionAngleRad));
            double vel2yr = vel2 * Math.Sin((angle1Rad - collisionAngleRad));

            //We now use the conservation of momentum to determine the new x velocities in our new reference frame, 
            //and the y-components do not change. 

            //find the final velocities in the normal reference frame
            //the x velocities will obey the rules for a 1 - D collision
            double m1 = ball1.GetMass();
            double m2 = ball2.GetMass();
            double massTotal = m1 + m2;
            double vel1fxr = ((m1 - m2) * vel1xr + (m2 + m2) * vel2xr) / massTotal;
            double vel2fxr = (2 * m1 * vel1xr + (m2 - m1) * vel2xr) / massTotal;
            //the y velocities will not be changed
            double vel1fyr = vel1yr;
            double vel2fyr = vel2yr;

            //We now have the 'after collision' velocities, but we have to transform the components back to the standard x-y reference frame.
            //convert back to the standard x,y coordinates
            double newVelX1 = Math.Cos(collisionAngleRad) * vel1fxr + Math.Cos(collisionAngleRad + Math.PI / 2) * vel1fyr;
            double newVelY1 = Math.Sin(collisionAngleRad) * vel1fxr + Math.Sin(collisionAngleRad + Math.PI / 2) * vel1fyr;
            double newVelX2 = Math.Cos(collisionAngleRad) * vel2fxr + Math.Cos(collisionAngleRad + Math.PI / 2) * vel2fyr;
            double newVelY2 = Math.Sin(collisionAngleRad) * vel2fxr + Math.Sin(collisionAngleRad + Math.PI / 2) * vel2fyr;

            ball1.UpdateVelocityAfterCollision(newVelX1, newVelY1);
            ball2.UpdateVelocityAfterCollision(newVelX2, newVelY2);
        }

        private void CorrectOverlapping(Ball ball1, Ball ball2, double collisionAngleRad)
        {
            //we have the collision angle between the balls. If the ball was going too fast or too slow, the balls may have been going to fast and some overlap has been 
            //occurred. Lets correct that.
            //calculate the intersection distance
            double distanceBetweenCircles = Math.Sqrt(DistanceBetweenTwoBallsSquared(ball1, ball2));
            //since we know that the current distance between the circles is less than the sum of their radii, we can find the overlap distance
            double overlapDistance = (ball1.GetRadius() + ball2.GetRadius()) - distanceBetweenCircles;
            //but, since they dont have the same radii, they have to be moved a different amount.
            //if the circles were equal then they would have to be moved overlapDistance / 2. But since they are not
            //multiply this by a factor determined by their radius quotient
            double ball1CorrectiveDistance = (overlapDistance / 2) * (ball1.GetRadius() / ball2.GetRadius()) * 1.1;
            double ball2CorrectiveDistance = (overlapDistance / 2) * (ball2.GetRadius() / ball1.GetRadius()) * 1.1;

            PointD ball1NewLocation = ball1.GetCenterPosition();
            PointD ball2NewLocation = ball2.GetCenterPosition();
            //now lets do the correction
            //without any coordinate system transformation
            //4 cases
            if (ball1.GetCenterPosition().X > ball2.GetCenterPosition().X)
            {
                ball1NewLocation.X += ball1CorrectiveDistance;// * Math.Cos(collisionAngleRad);
                ball2NewLocation.X -= ball2CorrectiveDistance;//  * Math.Cos(collisionAngleRad);
            }
            else
            {
                ball1NewLocation.X -= ball1CorrectiveDistance;//  * Math.Cos(collisionAngleRad);
                ball2NewLocation.X += ball2CorrectiveDistance;//  * Math.Cos(collisionAngleRad);
            }

            if (ball1.GetCenterPosition().Y > ball2.GetCenterPosition().Y)
            {
                ball1NewLocation.Y += ball1CorrectiveDistance;//  * Math.Sin(collisionAngleRad);
                ball2NewLocation.Y -= ball2CorrectiveDistance;//  * Math.Sin(collisionAngleRad);
            }
            else
            {
                ball1NewLocation.Y -= ball1CorrectiveDistance;//  * Math.Sin(collisionAngleRad);
                ball2NewLocation.Y += ball2CorrectiveDistance;//  * Math.Sin(collisionAngleRad);
            }

            ball1.SetNewCenterPosition(ball1NewLocation);
            ball2.SetNewCenterPosition(ball2NewLocation);
        }


        private void DealWithColliction1D(Ball ball1, Ball ball2)
        {
            /*
            * How we can deal with colisions?. There is a term called momentum (P)
            * Momentum is defined to be the mass of an object multiplied by the velocity of the object
            * This can be written as vecP = m*vecV, were P and V are vector quantities
            * For 2 objects,  in a collision, the momentum of a system is conserved 
            * (i.e. the total momentum of the system is the same after the collision as 
            * it was before the collision - or as I like to say, what goes in must come out).
            * A "system" consists of the two particles that are colliding. We can write this as
            *  Pinit = Pfinal
            *  
            *  Also if something is moving it has kinetic energy, and if it is not moving it has no kinetic energy.
            *  Κenergy is defined as KE= 1/2 * mass * vel^2. Kinetic energy is not a vector.
            *   In an elastic collision (ie perfect) the kinetic energy is conserved: the initial kinetic energy 
            *   of the system is equal to the final kinetic energy. We now have two conservation equations 
            *   that will let us determine the final velocities of two colliding particles if we know their 
            *   initial velocities and masses
            *   so, mass1_init * vel1_init + mass2_init * vel2_init = mass1_final * vel1_final + mass2_final * vel2_final
            *   and similarly for the kinetic energy
            *   Ke1_init + Ke2_init = Ke1_final + Ke2_final
            *   Using these 2 equations the Vfinal for both 1 & 2 can be found.
            */
            double m1 = ball1.GetMass();
            double m2 = ball2.GetMass();
            double massTotal = m1 + m2;
            double newVelX1 = ((m1 - m2) * ball1.GetVelX() + 2 * m2 * ball2.GetVelX()) / massTotal;
            double newVelY1 = ((m1 - m2) * ball1.GetVelY() + 2 * m2 * ball2.GetVelY()) / massTotal;
            double newVelX2 = (2 * m1 * ball1.GetVelX() + (m2 - m1) * ball2.GetVelX()) / massTotal;
            double newVelY2 = (2 * m1 * ball1.GetVelY() + (m2 - m1) * ball2.GetVelY()) / massTotal;

            ball1.UpdateVelocityAfterCollision(newVelX1, newVelY1);
            ball2.UpdateVelocityAfterCollision(newVelX2, newVelY2);
        }
    }
}

