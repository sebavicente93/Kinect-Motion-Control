using System;
using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States
{
    class State2 : IState
    {
        public int Id { get; }
        private float oldHeadY;
        // Estado que representa que la persona está en la posicion de descenso
        public State2()
        {
            this.Id = 2;
        }

        public int Update(Skeleton skeleton)
        {
            var cond1 = (skeleton.Joints[JointType.ShoulderCenter].Position.Z - skeleton.Joints[JointType.KneeLeft].Position.Z) < 0.1;
            var cond2 = (skeleton.Joints[JointType.ShoulderCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) < 0.1;
            var cond3 = (skeleton.Joints[JointType.ShoulderCenter].Position.Z - skeleton.Joints[JointType.KneeRight].Position.Z) < 0.1;
            var cond4 = (skeleton.Joints[JointType.ShoulderCenter].Position.Z - skeleton.Joints[JointType.AnkleRight].Position.Z) < 0.1;

            if (oldHeadY == 0)
            {
                oldHeadY = skeleton.Joints[JointType.Head].Position.Y;
            }

            bool goDownY = Math.Abs(oldHeadY - skeleton.Joints[JointType.Head].Position.Y) > 0.1;

            if (cond1 && cond2 && cond3 && cond4 && goDownY)
            {
                  return 3;             
            }
            else
            {
                if (goDownY)
                {
                    // Cambia de estado pero con una advertencia
                   this.Notify("Mantene el eje vertical mientras desciendas");
                    return 3;
                }
                
            }

            return -1;
        }

        private void Notify(String mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}
