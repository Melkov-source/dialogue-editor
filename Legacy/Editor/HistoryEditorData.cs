using System.Runtime.Serialization;

namespace History.Editor.HistoryEditor
{
    [DataContract]
    public class HistoryEditorData
    {
        [DataMember] public bool IsAutoSave;
        [DataMember] public bool IsShowCharacterNameInDialog;
        [DataMember] public bool IsShowSideInDialog;
    }
}