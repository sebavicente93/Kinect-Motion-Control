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
            this.activeState = new State0(0);
        }

    }
}
