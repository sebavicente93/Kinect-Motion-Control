using Microsoft.Samples.Kinect.SkeletonBasics.Modules.Squad.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    }
}
