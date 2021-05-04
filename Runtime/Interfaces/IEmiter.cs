using System;
using Unity.Mathematics;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface IEmitter<out TInfluenceTypes, out TFallOfTypes>: IInfluence<TInfluenceTypes> where TInfluenceTypes:Enum where TFallOfTypes:Enum
    {
        int2 Position { get; }
        int Radius { get; }
        TFallOfTypes FallOfType { get; }
    }
}