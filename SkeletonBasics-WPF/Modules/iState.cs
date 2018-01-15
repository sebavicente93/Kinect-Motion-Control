using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Samples.Kinect.SkeletonBasics.Modules
{
    public interface IState
    {
        int Id { get; }

        int Update(Skeleton skeleton);
    }
}
