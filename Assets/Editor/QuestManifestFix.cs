using System.IO;
using System.Xml;
using UnityEditor.Android;

namespace BinaryLift.EditorTools
{
    /// <summary>
    /// Quest 2 runs Android 12, where an app targeting API 31+ must declare any system native library it loads.
    /// Without this entry the OpenXR loader cannot open /system/lib64/libopenxr_forwardloader.oculus.so,
    /// the runtime never starts, and the app sits on the loading dots forever.
    /// </summary>
    public sealed class QuestManifestFix : IPostGenerateGradleAndroidProject
    {
        const string AndroidNs = "http://schemas.android.com/apk/res/android";
        const string Library = "libopenxr_forwardloader.oculus.so";

        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            string manifestPath = Path.Combine(path, "src", "main", "AndroidManifest.xml");
            if (!File.Exists(manifestPath)) return;

            var doc = new XmlDocument();
            doc.Load(manifestPath);

            var application = doc.SelectSingleNode("/manifest/application");
            if (application == null) return;

            foreach (XmlNode node in application.SelectNodes("uses-native-library"))
                if (node.Attributes?["name", AndroidNs]?.Value == Library) return;

            XmlElement entry = doc.CreateElement("uses-native-library");
            entry.SetAttribute("name", AndroidNs, Library);
            entry.SetAttribute("required", AndroidNs, "false");
            application.AppendChild(entry);
            doc.Save(manifestPath);
        }
    }
}
