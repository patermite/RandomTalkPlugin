using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RandomTalkPlugin.Windows
{
    public abstract class Window
    {
        public Window(RandomTalkPlugin plugin, PluginUI pluginUI)
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
