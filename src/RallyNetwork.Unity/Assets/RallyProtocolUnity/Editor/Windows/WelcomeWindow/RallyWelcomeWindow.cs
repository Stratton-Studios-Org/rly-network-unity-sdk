using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace RallyProtocol.Editor
{

    public class RallyWelcomeWindow : EditorWindow
    {

        public const string DocumentationUrl = "https://docs.rallyprotocol.com/";

        [SerializeField]
        private VisualTreeAsset m_VisualTreeAsset = default;

        protected Button documentationButton;
        protected Button setupButton;

        [MenuItem("Window/Rally Protocol/Welcome")]
        public static void Open()
        {
            RallyWelcomeWindow wnd = GetWindow<RallyWelcomeWindow>();
            wnd.titleContent = new GUIContent("Welcome");
            wnd.minSize = wnd.maxSize = new Vector2(440, 200);
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;
            VisualElement uxml = m_VisualTreeAsset.Instantiate();
            uxml.style.flexGrow = 1f;
            root.Add(uxml);

            this.documentationButton = root.Query<Button>("documentationButton");
            this.setupButton = root.Query<Button>("setupButton");

            this.documentationButton.clicked += OpenDocumentation;
            this.setupButton.clicked += OpenSetup;
        }

        public void OpenDocumentation()
        {
            Application.OpenURL(DocumentationUrl);
        }

        public void OpenSetup()
        {
            RallySetupWindow.Open();
        }

    }

}