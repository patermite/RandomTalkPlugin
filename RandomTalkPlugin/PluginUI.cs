using RandomTalkPlugin.Windows;
using System;

namespace RandomTalkPlugin
{
    // It is good to have this be disposable in general, in case you ever need it
    // to do any cleanup
    public class PluginUI : IDisposable
    {
        private RandomTalkPlugin plugin;

        public LotteryWindows LotteryWindows { get; set; }

        public RandomTalkWindow RandomTalkWindow { get; set; }

        public PluginUI(RandomTalkPlugin plugin)
        {
            this.plugin = plugin;

            LotteryWindows = new LotteryWindows(plugin, this);
            RandomTalkWindow = new RandomTalkWindow(plugin, this);
        }

        public void Dispose()
        {
        }

        public void Draw()
        {
            // This is our only draw handler attached to UIBuilder, so it needs to be
            // able to draw any windows we might have open.
            // Each method checks its own visibility/state to ensure it only draws when
            // it actually makes sense.
            // There are other ways to do this, but it is generally best to keep the number of
            // draw delegates as low as possible.

            LotteryWindows.Draw();
            RandomTalkWindow.Draw();
        }
    }
}
