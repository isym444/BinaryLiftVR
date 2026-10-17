using System;
using System.IO;
using UnityEngine;

namespace BinaryLift
{
    /// <summary>Local event log for teammate QA; never records account or tracking data.</summary>
    public sealed class QuestJournal
    {
        [Serializable]
        sealed class Entry
        {
            public string utc, eventName, phase;
            public int node, cones, moves, levels;
        }
        readonly string path;
        bool disabled;

        public QuestJournal()
        {
            try
            {
                string dir = Path.Combine(Application.persistentDataPath, "QuestLogs");
                Directory.CreateDirectory(dir);
                path = Path.Combine(dir, "session-" + DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff") + ".jsonl");
                Debug.Log("Campus quest journal: " + path);
            }
            catch (Exception e) { disabled = true; Debug.LogWarning("Quest journal unavailable: " + e.Message); }
        }

        public void Write(string eventName, CampusQuestState state, int levels = 0)
        {
            Debug.Log("CampusQuest | " + eventName + " | node=" + state.Node + " | cones=" + state.Cones);
            if (disabled) return;
            try
            {
                File.AppendAllText(path, JsonUtility.ToJson(new Entry {
                    utc = DateTime.UtcNow.ToString("O"), eventName = eventName,
                    phase = state.Phase.ToString(), node = state.Node, cones = state.Cones,
                    moves = state.Moves, levels = levels
                }) + Environment.NewLine);
            }
            catch (Exception e) { disabled = true; Debug.LogWarning("Quest journal stopped: " + e.Message); }
        }
    }
}
