using System;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States
{
    class State1 : IState
    {
        public int Id { get; }
        //Estado que chequea que esté parado derecho
        public State1()
        {
            this.Id = 1;
        }

        public int Update(Skeleton skeleton)
        {
            var cond1 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderCenter].Position.Z) < 0.1;
            var cond2 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.KneeLeft].Position.Z) < 0.1;
            var cond3 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) < 0.1;
            var cond5 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.KneeRight].Position.Z) < 0.1;
            var cond6 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleRight].Position.Z) < 0.1; 

            if (cond1 && cond2 && cond3 && cond5 && cond6)
            {
                 return 2;
            }
            else
            {
                this.Notify("Ponete derecho");
                return -1;
            }
        }

        private void Notify(String mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}
