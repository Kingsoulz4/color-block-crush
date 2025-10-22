using Yoolax.Framework;
using Firebase.Analytics;
using UnityEngine;
using System.Collections.Generic;

namespace Analytics
{
    public static class LevelAnalytics
    {
        public static void InitEvent()
        {
            Server.Get<OnLevelStartEventLog>().AddListener(LogLevelStartEvent);
            Server.Get<OnLevelEndEventLog>().AddListener(LogLevelEndEvent);
            Server.Get<OnLevelExitEventLog>().AddListener(LogLevelExitEvent);
            Server.Get<OnLevelReopenEventLog>().AddListener(LogLevelReopenEvent);
        }

        public static void LogLevelStartEvent(LevelAnalyticStruct levelStruct)
        {            
            var parameters = new[]
            {
            new Parameter("level", levelStruct.level),
            new Parameter("loop_by", levelStruct.loopBy),
            new Parameter("play_type", levelStruct.playType.ToString()),
            new Parameter("play_index", levelStruct.playIndex),
            new Parameter("lose_index", levelStruct.loseIndex),
            new Parameter("total_duration_start", levelStruct.total_duration_start),
            new Parameter("mode", levelStruct.mode.ToString()),
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "level", levelStruct.level },
                { "loop_by", levelStruct.loopBy },
                { "play_type", levelStruct.playType.ToString() },
                { "play_index", levelStruct.playIndex },
                { "lose_index", levelStruct.loseIndex },
                { "total_duration_start", levelStruct.total_duration_start },
                { "mode", levelStruct.mode.ToString() }               
            };
            FirebaseManager.Instance.AddEvent("level_start", paramData, parameters);
        }

        public static void LogLevelEndEvent(LevelAnalyticStruct levelStruct)
        {
            var parameters = new[]
            {
            new Parameter("level", levelStruct.level),
            new Parameter("loop_by", levelStruct.loopBy),
            new Parameter("play_type", levelStruct.playType.ToString()),
            new Parameter("mode", levelStruct.mode.ToString()),
            new Parameter("play_index", levelStruct.playIndex),
            new Parameter("lose_index", levelStruct.loseIndex),
            new Parameter("total_duration_start", levelStruct.total_duration_start),
            new Parameter("total_duration_end", levelStruct.total_duration_end),
            new Parameter("remain_duration", levelStruct.remain_duration),
            new Parameter("play_duration", levelStruct.playDuration),
            new Parameter("level_progress", levelStruct.levelProgress),
            new Parameter("result", levelStruct.result.ToString()),
            new Parameter("lose_by", levelStruct.loseBy.ToString())       
        };

            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "level", levelStruct.level },
                { "loop_by", levelStruct.loopBy },
                { "play_type", levelStruct.playType.ToString() },
                { "play_index", levelStruct.playIndex },
                { "lose_index", levelStruct.loseIndex },
                { "total_duration_start", levelStruct.total_duration_start },
                { "total_duration_end", levelStruct.total_duration_end },
                { "remain_duration", levelStruct.remain_duration },
                { "level_progress", levelStruct.levelProgress },
                { "play_duration", levelStruct.playDuration },
                { "mode", levelStruct.mode.ToString() },
                { "result", levelStruct.result.ToString() },
                { "lose_by", levelStruct.loseBy.ToString() }
            };
            FirebaseManager.Instance.AddEvent("level_end", paramData, parameters);
        }

        public static void LogLevelExitEvent(LevelAnalyticStruct levelStruct)
        {
            var parameters = new[]
            {
            new Parameter("level", levelStruct.level),
            new Parameter("loop_by", levelStruct.loopBy),
            new Parameter("play_type", levelStruct.playType.ToString()),
            new Parameter("mode", levelStruct.mode.ToString()),
            new Parameter("level_progress", levelStruct.levelProgress),
            new Parameter("play_index", levelStruct.playIndex),
            new Parameter("lose_index", levelStruct.loseIndex),
            new Parameter("exit_index", levelStruct.exitIndex),           
            new Parameter("play_duration", levelStruct.playDuration)           
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "level", levelStruct.level },
                { "loop_by", levelStruct.loopBy },
                { "play_type", levelStruct.playType.ToString() },
                { "level_progress", levelStruct.levelProgress },
                { "play_index", levelStruct.playIndex },
                { "lose_index", levelStruct.loseIndex },                                
                { "play_duration", levelStruct.playDuration },
                { "mode", levelStruct.mode.ToString() },                
                { "exit_index", levelStruct.exitIndex }
            };
            FirebaseManager.Instance.AddEvent("level_exit", paramData, parameters);
        }

        public static void LogLevelReopenEvent(LevelAnalyticStruct levelStruct)
        {
#if UNITY_FIREBASE
            var parameters = new[]
            {
            new Parameter("level", levelStruct.level),
            new Parameter("loop_by", levelStruct.loopBy),
            new Parameter("play_index", levelStruct.playIndex),
            new Parameter("lose_index", levelStruct.loseIndex),
            new Parameter("mode", levelStruct.mode.ToString())            
        };
            Dictionary<string, object> paramData = new Dictionary<string, object>
            {
                { "level", levelStruct.level },
                { "loop_by", levelStruct.loopBy },                
                { "play_index", levelStruct.playIndex },
                { "lose_index", levelStruct.loseIndex },                
                { "mode", levelStruct.mode.ToString() },               
            };
            FirebaseManager.Instance.AddEvent("level_reopen", paramData, parameters);
#endif
        }
    }

    public struct LevelAnalyticStruct
    {
        public int level;
        public int loopBy;
        public int playIndex;
        public int loseIndex;
        public int exitIndex;
        public float total_duration_start;
        public float total_duration_end;
        public float remain_duration;
        public int levelProgress;
        public float playDuration;
        public PlayType playType;
        public Mode mode;        
        public LevelResult result;
        public LoseBy loseBy;

        public LevelAnalyticStruct SetBaseLevel(int level, int loopBy, Mode mode, int playIndex, int loseIndex)
        {            
            this.level = level;
            this.loopBy = loopBy;
            this.mode = mode;
            this.playIndex = playIndex;
            this.loseIndex = loseIndex;
            return this;
        }

        public LevelAnalyticStruct SetLevelStartStruct(PlayType playType, float totalDurationStart = 0)
        {            
            this.playType = playType;
            this.total_duration_start = totalDurationStart;
            return this;
        }

        public LevelAnalyticStruct SetLevelEndStruct(PlayType playType, int levelProgress, LevelResult result, 
            LoseBy loseBy, float playDuration, float totalDurationStart = 0, float totalDurationEnd = 0, float remainDuration = 0)
        {
            this.playType = playType;
            this.levelProgress = levelProgress;
            this.result = result;
            this.loseBy = loseBy;
            this.playDuration = playDuration;
            this.total_duration_start = totalDurationStart;
            this.total_duration_end = totalDurationEnd;
            this.remain_duration = remainDuration;
            return this;
        }

        public LevelAnalyticStruct SetLevelExitStruct(PlayType playType, int levelProgress, float playDuration, int exitIndex)
        {
            this.playType = playType;
            this.levelProgress = levelProgress;
            this.exitIndex = exitIndex;
            this.playDuration = playDuration;
            return this;
        }

        public LevelAnalyticStruct SetLevelReopenStruct()
        {
            return this;
        }
    }

    public enum PlayType
    {
        home,
        next,
        restart
    }

    public enum LevelResult
    {
        win,
        lose,
        quit,
        restart
    }

    public enum LoseBy
    {
        full_slot,
        bomb,
        ice,
        time,
        NULL
    }
}
