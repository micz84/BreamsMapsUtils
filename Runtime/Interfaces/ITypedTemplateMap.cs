using System;

namespace pl.breams.dotsinfluancemaps.interfaces
{
    public interface ITypedTemplateMap<out TInfluenceTypes>:ITemplateMap,ITypedInfluenceMap<TInfluenceTypes> where TInfluenceTypes:Enum
    {

    }
}