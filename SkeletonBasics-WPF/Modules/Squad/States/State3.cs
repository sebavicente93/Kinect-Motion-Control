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
        // Estado que representa la parte mas baja del descenso
        public State3()
        {
            this.Id = 3;
        }

        public int Update(Skeleton skeleton)
        {
            //Condicion de corte posicion mas baja del ejercicio
            bool cond1 = Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Y - skeleton.Joints[JointType.KneeLeft].Position.Y) < 0.05;
            bool cond2 = Math.Abs(skeleton.Joints[JointType.HipRight].Position.Y - skeleton.Joints[JointType.KneeRight].Position.Y) < 0.05;
            
            // Condicion que se debe cumplir mientras bajo!!
            bool cond3 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderCenter].Position.Z) > 0.1;
            bool cond4 = (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.AnkleLeft].Position.Z) > 0.1;

            if (cond3 && cond4)
            {
                if (cond1 && cond2)
                {
                    return 4;
                }
                else
                {
                    this.Notify("Bajá un poquito más");
                }
            }else
            {
                this.Notify("Tirá la cadera para atras cuando bajas");
            }

            return -1;
        }

        private void Notify(String mensaje)
        {
            Console.WriteLine(mensaje);
        }
    }
}
