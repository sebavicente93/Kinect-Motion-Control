using Microsoft.Kinect;
using Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Microsoft.Samples.Kinect.SkeletonBasics.Modules.Utilities;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad
{
    class SquadFSM: FSM
    {
        public SquadFSM()
        {
            this.initFSM();
        }

        private void initFSM()
        {
            this.states = new IState[]{
                new State0(),
                new State1(),
                new State2(),
                new State3(),
                new State4()
            };

            this.activeState = this.states[0];
        }

        public override void Update(Skeleton skeleton)
        {
            base.Update(skeleton);
            var ShoulderCenter = new PointF(skeleton.Joints[JointType.ShoulderCenter].Position.Y, skeleton.Joints[JointType.ShoulderCenter].Position.Z);
            var Hip = new PointF(skeleton.Joints[JointType.HipCenter].Position.Y, skeleton.Joints[JointType.HipCenter].Position.Z);
            var Spine = new PointF(skeleton.Joints[JointType.Spine].Position.Y, skeleton.Joints[JointType.Spine].Position.Z);

            var distance = MathUtility.DistanceFromPointToLine(Spine, Hip, ShoulderCenter);

            if (distance > 0.1)
            {
                Console.WriteLine($"Ponete derecho bobo {distance} ShoulderCenter: {ShoulderCenter.X} {ShoulderCenter.Y}");
            }else
            {
                Console.WriteLine($"Todo piola {distance}");
            }

        }

    }
}
