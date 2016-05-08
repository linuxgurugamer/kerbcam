using KerbCam.Core;
using KSP.IO;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KerbCam {
    public delegate void KeyEvent();
    public delegate void AnyKeyEvent(Event ev);
    public delegate void KeyBindingChangedEvent();

    public class KeyBind {
        private ExtEvent binding;
        private ExtEvent defaultBind;
        private string humanBinding;
        private bool requiredBound;
        public string description;
        public event KeyEvent keyUp;
        public event KeyEvent keyDown;
        public event KeyBindingChangedEvent changed;

        public KeyBind(string description, bool requiredBound, KeyCode defaultKeyCode) {
            this.description = description;
            this.defaultBind = new ExtEvent(defaultKeyCode, false);
            this.requiredBound = requiredBound;
            SetBinding(defaultBind);
        }

        public KeyBind(string description) {
            this.description = description;
            this.defaultBind = new ExtEvent();
            this.requiredBound = false;
            SetBinding(defaultBind);
        }

        public bool IsBound() {
            return binding.IsBound();
        }

        public bool IsRequiredBound() {
            return requiredBound;
        }

        public void SetBinding(ExtEvent ev) {
            binding = ev;
            humanBinding = ev.ToHumanString();
            if (changed != null) {
                changed();
            }
        }

        public void SetBinding(Event ev) {
            SetBinding(new ExtEvent(ev));
        }

        public string HumanBinding {
            get { return humanBinding; }
        }

        public bool MatchAndFireEvent(Event ev) {
            if (!binding.Matches(ev)) {
                return false;
            }

            KeyEvent destEvent;
            if (ev.type == EventType.KeyUp) {
                destEvent = this.keyUp;
            } else if (ev.type == EventType.KeyDown) {
                destEvent = this.keyDown;
            } else {
                return true;
            }
            if (destEvent != null) {
                destEvent();
            }
            ev.Use();
            return true;
        }

        public void SetFromConfig(string evStr) {
            SetBinding(ExtEvent.Parse(evStr));
        }

        public string GetForConfig() {
            return binding.ToString();
        }
    }

    public class KeyBindings<KeyT> : IConfigNode {

        // TODO: Maybe optimize this with a hash of the binding, but be
        // careful about hashes changing when the binding changes.
        private List<KeyBind> bindings = new List<KeyBind>();
        private Dictionary<KeyT, KeyBind> keyToBinding = new Dictionary<KeyT, KeyBind>();

        /// <summary>
        /// Captures *all* key events. Will block other key events while at
        /// least one delegate is set.
        /// </summary>
        public event AnyKeyEvent captureAnyKey;

        /// <summary>
        /// Any key binding was changed.
        /// </summary>
        public event KeyBindingChangedEvent anyChanged;

        public void AddBinding(KeyT key, KeyBind kb) {
            this.bindings.Add(kb);
            keyToBinding[key] = kb;
            kb.changed += HandleAnyChanged;
        }

        public void ListenKeyUp(KeyT key, KeyEvent del) {
            keyToBinding[key].keyUp += del;
        }

        public void UnlistenKeyUp(KeyT key, KeyEvent del) {
            keyToBinding[key].keyUp -= del;
        }

        public void ListenKeyDown(KeyT key, KeyEvent del) {
            keyToBinding[key].keyDown += del;
        }

        public void UnlistenKeyDown(KeyT key, KeyEvent del) {
            keyToBinding[key].keyDown -= del;
        }

        public void HandleEvent(Event ev) {
            if (ev.isKey && (ev.type == EventType.KeyUp || ev.type == EventType.KeyDown)) {
                if (captureAnyKey != null) {
                    if (ev.type == EventType.KeyUp) {
                        captureAnyKey(ev);
                        ev.Use();
                    }
                } else {
                    foreach (var kb in bindings) {
                        if (kb.MatchAndFireEvent(ev)) {
                            return;
                        }
                    }
                }
            }
        }

        public IEnumerable<KeyBind> Bindings() {
            return bindings;
        }

        public void Load(ConfigNode node) {
            foreach (var key in keyToBinding.Keys) {
                var kb = keyToBinding[key];
                kb.SetFromConfig(node == null ? null : node.GetValue(key.ToString()));
            }
        }

        public void Save(ConfigNode node) {
            foreach (var key in keyToBinding.Keys) {
                var kb = keyToBinding[key];
                node.AddValue(key.ToString(), kb.GetForConfig());
            }
        }

        private void HandleAnyChanged() {
            if (anyChanged != null) {
                anyChanged();
            }
        }
    }
}
