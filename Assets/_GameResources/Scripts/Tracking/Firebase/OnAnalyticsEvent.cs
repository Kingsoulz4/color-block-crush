using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yoolax.Framework;

namespace Analytics
{
    public class OnAnalyticsEvent : BaseEvent
    {

    }

    public class OnUserPropertiesLog : BaseEvent<UserPropertiesStruct>
    {

    }

    #region Level Analytics

    public class OnLevelStartEventLog : BaseEvent<LevelAnalyticStruct>
    {

    }

    public class OnLevelCompleteEventLog : BaseEvent<LevelAnalyticStruct>
    {

    }

    public class OnLevelFailEventLog : BaseEvent<LevelAnalyticStruct> 
    {

    }

    #endregion
    
}