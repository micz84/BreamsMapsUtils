using System;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface ITypedInfluenceMap<out TInfluenceTypes>:IInfluenceMap, IInfluence<TInfluenceTypes> where TInfluenceTypes:Enum
    {

    }
}