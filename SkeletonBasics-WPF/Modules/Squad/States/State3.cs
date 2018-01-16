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
            if (Math.Abs(skeleton.Joints[JointType.HipLeft].Position.Z - skeleton.Joints[JointType.HipRight].Position.Z) < 0.03)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
