using System;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States
{
    class State0 : IState
    {
        public int Id { get; }

        //Estado que chequea que el usuario este frente a la cámara.

        public State0()
        {
            this.Id = 0;
        }

        public int Update(Skeleton skeleton)
        {
            if (Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Z - skeleton.Joints[JointType.HipRight].Position.Z) < 0.03)
            {
                this.Notify("Listos para empezar");
                return 1;
            }
            else
            {
                return -1;
            }
        }

        private void Notify(String mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}
