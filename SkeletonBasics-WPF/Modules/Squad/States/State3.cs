using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States
{
    class State3 : IState
    {
        public int Id { get; }

        public State3()
        {
            this.Id = 3;
        }

        public int Update(Skeleton skeleton)
        {
            //Condicion de corte posicion mas baja del ejercicio
            bool cond1 = Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Y - skeleton.Joints[JointType.KneeLeft].Position.Y) < 0.05;
            bool cond2 = Math.Abs(skeleton.Joints[JointType.HipRight].Position.Y - skeleton.Joints[JointType.KneeRight].Position.Y) < 0.05;
            var cond1 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderCenter].Position.Z) > 0.1;
            var cond2 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) > 0.1;

            Console.WriteLine("HipCenter " + skeleton.Joints[JointType.HipCenter].Position.Z + " ShoulderCenter " + skeleton.Joints[JointType.ShoulderCenter].Position.Z);

        }
    }
}
