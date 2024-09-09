using History.Editor.HistoryEditor.Reflections;

namespace History.Editor.HistoryEditor
{
    public enum FileType
    {
        [FileExtension(".asset")] SCRIPTABLE_OBJECT,
        [FileExtension(".json")] JSON
    }
}