using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules
{
    abstract class FSM
    {
        protected IState[] states;
        protected IState activeState;

        public void Update(Skeleton skeleton)
        {
            Console.WriteLine($"Active State {this.activeState.Id}");
            int newState= this.activeState.Update(skeleton);
            
            if (newState != -1)
            {
                this.activeState = this.states[newState];
            }
        }
    }
}
