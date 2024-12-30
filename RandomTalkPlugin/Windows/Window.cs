
namespace RandomTalkPlugin.Windows
{
    public abstract class Windows
    {
        public Windows(RandomTalkPlugin plugin, PluginUI pluginUI)
        {
            this.plugin = plugin;
            this.pluginUI = pluginUI;
        }


        internal readonly RandomTalkPlugin plugin;
        internal readonly PluginUI pluginUI;

        // this extra bool exists for ImGui, since you can't ref a property
        internal bool visible = false;

        public bool Visible
        {
            get { return this.visible; }
            set { this.visible = value; }
        }

        public abstract void Draw();
    }
}
