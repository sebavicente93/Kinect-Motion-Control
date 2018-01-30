using Microsoft.Kinect;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States
{
    class State4 : IState
    {
        public int Id { get; }

        public State4()
        {
            this.Id = 4;
        }

        public int Update(Skeleton skeleton)
        {
            //check that the body is straight. Same Z and X
            bool cond1= (skeleton.Joints[JointType.HipCenter].Position.Z - skeleton.Joints[JointType.ShoulderCenter].Position.Z) < 0.05;
            bool cond2 = (skeleton.Joints[JointType.KneeLeft].Position.Z - skeleton.Joints[JointType.ShoulderCenter].Position.Z) < 0.1;
            bool cond3 = (skeleton.Joints[JointType.HipCenter].Position.X - skeleton.Joints[JointType.ShoulderCenter].Position.X) < 0.05;


            if (cond1 && cond2 && cond3)
            {
                return 1;
            }
            else
            {
                //Check that the column is always straight


                return -1;
            }
        }
    }
}
