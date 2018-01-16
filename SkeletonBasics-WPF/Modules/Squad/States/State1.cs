using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States
{
    class State1 : IState
    {
        public int Id { get; }
        private float oldHeadY;

        public State1()
        {
            this.Id = 1;
            this.oldHeadY = 0;
        }

        public int Update(Skeleton skeleton)
        {
            var cond1 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderLeft].Position.Z) < 0.1;
            var cond2 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.KneeLeft].Position.Z) < 0.1;
            var cond3 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) < 0.1;
            var cond4 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderRight].Position.Z) < 0.1;
            var cond5 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.KneeRight].Position.Z) < 0.1;
            var cond6 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleRight].Position.Z) < 0.1; 

            if (cond1 && cond2 && cond3 && cond4 && cond5 && cond6)
            {
                Console.WriteLine("Todo perfecto!");
                return 2;
            }
            else
            {
                if (!cond1)
                {
                    Console.WriteLine("ShoulderLeft");
                }
                if (!cond2)
                {
                    Console.WriteLine("KneeLeft");
                }
                if (!cond3)
                {
                    Console.WriteLine("AnkleLeft");
                }
                if (!cond4)
                {
                    Console.WriteLine("ShoulderRight");
                }
                if (!cond5)
                {
                    Console.WriteLine("KneeRight");
                }
                if (!cond6)
                {
                    Console.WriteLine("AnkleRight");
                }

                return -1;
            }
        }
    }
}
