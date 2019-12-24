using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LogicGates
{
    static class Input
    {
        private static readonly Hashtable kb_prev = new Hashtable();
        private static readonly Hashtable kb_now = new Hashtable();
        private static bool ScrollUp;
        private static bool ScrollDown;
        private static Vector mouse;
        public static void LinkReferences(Form form, PictureBox canvas)
        {
            mouse = new Vector();
            form.KeyPreview = true;
            form.KeyDown += EventKeyDown;
            form.KeyUp += EventKeyUp;
            canvas.MouseDown += EventMouseDown;
            canvas.MouseUp += EventMouseUp;
            canvas.MouseMove += EvenMouseMove;
            canvas.MouseWheel += EventMouseWheel;
        }
        private static void EventMouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                ScrollUp = true;
                ScrollDown = false;
            }
            else if (e.Delta < 0)
            {
                ScrollUp = false;
                ScrollDown = true;
            }
        }
        private static void EvenMouseMove(object sender, MouseEventArgs e)
        {
            mouse.x = e.X;
            mouse.y = e.Y;
        }
        private static void EventMouseDown(object sender, MouseEventArgs e)
        {
            UpdateState(e.Button, true);
        }
        private static void EventMouseUp(object sender, MouseEventArgs e)
        {
            UpdateState(e.Button, false);
        }
        private static void EventKeyDown(object sender, KeyEventArgs e)
        {
            UpdateState(e.KeyCode, true);
        }
        private static void EventKeyUp(object sender, KeyEventArgs e)
        {
            UpdateState(e.KeyCode, false);
        }
        public static bool WheelScrollUp()
        {
            bool state = ScrollUp;
            ScrollUp = false;
            return state;
        }
        public static bool WheelScrollDown()
        {
            bool state = ScrollDown;
            ScrollDown = false;
            return state;
        }
        public static bool KeyPressed(MouseButtons key)
        {
            if (kb_now[key] != null)
            {
                if (!(bool)kb_prev[key])
                    return (bool)kb_now[key];
                else
                    return false;
            }
            else return false;
        }
        public static bool KeyReleased(MouseButtons key)
        {
            if (kb_now[key] != null)
            {
                if ((bool)kb_prev[key]) 
                    return !(bool)kb_now[key];
                else 
                    return false;
            }
            else return false;
        }
        public static bool KeyHeld(MouseButtons key)
        {
            if (kb_now[key] == null) return false;
            return (bool)kb_now[key];
        }
        public static bool KeyPressed(Keys key)
        {
            if (kb_now[key] != null)
            {
                if (!(bool)kb_prev[key]) 
                    return (bool)kb_now[key];
                else 
                    return false;
            }
            else return false;
        }
        public static bool KeyReleased(Keys key)
        {
            if (kb_now[key] != null)
            {
                if ((bool)kb_prev[key]) 
                    return !(bool)kb_now[key];
                else 
                    return false;
            }
            else return false;
        }
        public static void UpdateKeys()
        {
            foreach (DictionaryEntry key in kb_now)
                kb_prev[key.Key] = (bool)kb_now[key.Key];
        }
        public static bool KeyHeld(Keys key)
        {
            if (kb_now[key] == null) return false;
            return (bool)kb_now[key];
        }
        public static Vector MousePos()
        {
            return mouse;
        }
        public static void UpdateState(Keys key, bool state)
        {
            kb_now[key] = state;
            if (kb_prev[key] == null) kb_prev[key] = false;
        }
        public static void UpdateState(MouseButtons key, bool state)
        {
            kb_now[key] = state;
            if (kb_prev[key] == null) kb_prev[key] = false;
        }
    }
}
