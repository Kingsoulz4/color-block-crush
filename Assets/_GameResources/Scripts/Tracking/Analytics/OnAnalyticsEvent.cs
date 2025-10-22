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

    public class OnLevelEndEventLog : BaseEvent<LevelAnalyticStruct>
    {

    }

    public class OnLevelExitEventLog : BaseEvent<LevelAnalyticStruct> 
    {

    }

    public class OnLevelReopenEventLog : BaseEvent<LevelAnalyticStruct>
    {

    }

    #endregion

    #region Resource Analytics

    public class OnResourceEarnEventLog : BaseEvent<ResourceAnalyticStruct>
    {

    }

    public class OnResourceSpendEventLog : BaseEvent<ResourceAnalyticStruct>
    {

    }

    #endregion

    #region In App Purchase Analytics

    public class OnIAPShowEventLog : BaseEvent<InAppPurchaseAnalyticStruct>
    {

    }

    public class OnIAPClickEventLog : BaseEvent<InAppPurchaseAnalyticStruct>
    {

    }

    public class OnIAPPurchaseEventLog : BaseEvent<InAppPurchaseAnalyticStruct>
    {

    }

    public class OnIAPCloseEventLog : BaseEvent<InAppPurchaseAnalyticStruct>
    {

    }

    #endregion

    #region Ads Analytics

    public class OnAdRequestEventLog : BaseEvent<AdsAnalyticStruct>
    {

    }

    public class OnAdImpressionEventLog : BaseEvent<AdsAnalyticStruct>
    {

    }

    public class OnAdClickEventLog : BaseEvent<AdsAnalyticStruct>
    {

    }

    public class OnAdCompleteEventLog : BaseEvent<AdsAnalyticStruct>
    {

    }

    #endregion

    #region Event Analytics

    public class OnEventFeatureFirstShow : BaseEvent<FeatureAnalyticStruct>
    {

    }

    public class OnEventFeatureOpen : BaseEvent<FeatureAnalyticStruct>
    {

    }

    public class OnEventFeatureClose : BaseEvent<FeatureAnalyticStruct>
    {

    }

    #endregion

    #region Other Metrics

    public class OnButtonClickEventLog : BaseEvent<ButtonAnalyticStruct>
    {

    }

    public class OnLoadingSceneStartEventLog : BaseEvent<LoadingScneneAnalyticStruct>
    {

    }

    public class OnLoadingSceneEndEventLog : BaseEvent<LoadingScneneAnalyticStruct>
    {

    }

    #endregion
}