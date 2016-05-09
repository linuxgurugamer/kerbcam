using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KerbCam.Core
{
    public struct ExtEvent
    {
        private Event ev;

        /// <summary>
        /// Acts as the numeric field on ev, as we can't set that
        /// field.
        /// </summary>
        public bool numeric;

        public ExtEvent(KeyCode keyCode, bool numeric)
        {
            this.ev = Event.KeyboardEvent(keyCode.ToString());
            this.numeric = numeric;
        }

        public ExtEvent(Event ev)
        {
            if (ev != null)
            {
                this.ev = new Event(ev);
                this.numeric = ev.numeric;
            }
            else {
                this.ev = null;
                this.numeric = false;
            }
        }

        public ExtEvent(Event ev, bool numeric)
        {
            if (ev != null)
            {
                this.ev = new Event(ev);
            }
            else {
                this.ev = null;
            }
            this.numeric = numeric;
        }

        public bool IsBound()
        {
            return ev != null;
        }

        public void Clear()
        {
            ev = null;
            numeric = false;
        }

        public bool Matches(Event ev)
        {
            return (
                this.ev != null &&
                this.ev.keyCode == ev.keyCode &&
                this.ev.alt == ev.alt &&
                this.ev.control == ev.control &&
                this.ev.shift == ev.shift &&
                this.ev.command == ev.command);
        }

        /// <summary>
        /// Returns the event in a parseable form.
        /// </summary>
        /// <returns>The event as a string.</returns>
        public override string ToString()
        {
            if (ev == null)
            {
                return "";
            }

            StringBuilder s = new StringBuilder(10);

            if (numeric) s.Append("*");
            var mods = ev.modifiers;
            if ((mods & EventModifiers.Alt) != 0) s.Append("&");
            if ((mods & EventModifiers.Control) != 0) s.Append("^");
            if ((mods & EventModifiers.Command) != 0) s.Append("%");
            if ((mods & EventModifiers.Shift) != 0) s.Append("#");
            s.Append(ev.keyCode.ToString());

            return s.ToString();
        }

        public static ExtEvent Parse(string evStr)
        {
            if (evStr == null || evStr == "")
            {
                return new ExtEvent(null, false);
            }

            bool numeric = evStr.StartsWith("*");
            if (numeric)
            {
                evStr = evStr.Substring(1);
            }
            Event ev = Event.KeyboardEvent(evStr);
            return new ExtEvent(ev, numeric);
        }

        /// <summary>
        /// Creates a readable string for the event.
        /// </summary>
        /// <returns>The description string.</returns>
        public string ToHumanString()
        {
            if (ev == null)
            {
                return "<unset>";
            }

            List<string> p = new List<string>(5);
            var mods = ev.modifiers;
            if ((mods & EventModifiers.Alt) != 0) p.Add("Alt");
            if ((mods & EventModifiers.Control) != 0) p.Add("Ctrl");
            if ((mods & EventModifiers.Command) != 0) p.Add("Cmd");
            if ((mods & EventModifiers.Shift) != 0) p.Add("Shift");
            string keyDesc;
            if (numeric)
            {
                keyDesc = "(numpad)" + ev.keyCode.ToString();
            }
            else {
                keyDesc = ev.keyCode.ToString();
            }
            p.Add(keyDesc);

            return string.Join("+", p.ToArray());
        }
    }

}