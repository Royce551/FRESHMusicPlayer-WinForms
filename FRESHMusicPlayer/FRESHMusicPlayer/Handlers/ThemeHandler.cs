using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FRESHMusicPlayer.Handlers
{
    static class ThemeHandler
    {
        private static IEnumerable<Control> GetAllChildren(this Control root) // Not my code. Thanks, "Servy" from Stack Overflow!
        {
            var stack = new Stack<Control>();
            stack.Push(root);

            while (stack.Any())
            {
                var next = stack.Pop();
                foreach (Control child in next.Controls)
                    stack.Push(child);
                yield return next;
            }
        }
        public static void SetColors(this Control root, (int red, int green, int blue) highlightcolor, (int red, int green, int blue) forecolor, Color backcolor, Color textcolor)
        {
            int hR = highlightcolor.red; int hG = highlightcolor.green; int hB = highlightcolor.blue;
            int fR = forecolor.red; int fG = forecolor.green; int fB = forecolor.blue;
            if (root.Name == "UserInterface") // The main form needs some special handling to look right
            {
                root.BackColor = Color.FromArgb(hR, hG, hB);
            }
            else root.BackColor = backcolor;
            //root.volumeBar.BackColor = Color.FromArgb(hR, hG, hB); // Handles highlight colors
            root.ForeColor = textcolor;
            foreach (var button in ThemeHandler.GetAllChildren(root).OfType<Button>())
                button.ForeColor = Color.Black; // The button text should always be black (because buttons are always white)
            foreach (var tab in ThemeHandler.GetAllChildren(root).OfType<TabPage>())
            {
                tab.BackColor = backcolor;
                tab.ForeColor = textcolor;
            }
            foreach (var group in ThemeHandler.GetAllChildren(root).OfType<GroupBox>())
            {
                if (group.Name == "controlsBox") continue; // Avoid theming the controls box (it's already themed)
                group.BackColor = backcolor;
                group.ForeColor = textcolor;
            }
            foreach (var list in ThemeHandler.GetAllChildren(root).OfType<ListBox>())
            {
                list.BackColor = backcolor;
                list.ForeColor = textcolor;
            }
        }
    }
}
