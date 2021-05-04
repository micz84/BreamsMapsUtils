using System;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface IInfluence<out TInfluenceTypes> where TInfluenceTypes:Enum
    {
        TInfluenceTypes InfluenceType { get; }
    }
}