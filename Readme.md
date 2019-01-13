This is a tool for those video ﻿makers who want just a bit more camera movement and orientation﻿ control﻿﻿

KerbCam on Curse

Note that Camera Tools will likely cover many of the features of KerbCam and more, and will probably see more love and time than I can spend on KerbCam.

Kerbcam2 WIP/RFC thread

Source code

License (BSD license)

Issues/feature requests

KerbCam guidebook on Google Docs

Google Drive for archived releases

This is a tool for those video ﻿makers who want just a bit more camera movement and orientation﻿ control﻿﻿.

Excellent tutorial video provided by Rureglathon:

﻿
Installation:

Unzip the ZIP file inside the GameData directory. This should create a "KerbCam" directory there (with a "Plugins" subdirectory).

Features:

Keyed pathing and playback. Create and edit keys, each of which controls the orientation and position of the camera relative to the spacecraft at a given time on the path. Path playback attempts to smoothly interpolate from one point to the next.
6-degrees-of-freedom camera controller. No longer be limited to camera positions that are simple rotations around the craft.
Path recording (and playback) and camera movement while paused. This is actually an accidental feature, but one that is very handy for preparatory recording of a path or taking a screenshot while a lot of action is going on. (suggest creating and selecting the new path before pausing to workaround a glitch for now). This should also allow for cool "bullet-time" like effects (albeit with the game paused rather than slowed down).
Keyframed linear﻿ly interpolated time slowdown/pausing. (bullet time effect)
Make more videos, with greater freedom than you ever had before.

Known to work with KSP 0.23.5.

Known issues:

When installing KerbCam, including the source files apparently incurs a heavy framerate loss. It's unknown why this happens. Workaround: remove/don't install the Source directory (it's not required for KerbCam to work - future versions will be distributed without this).
When switching from map view to flight view while KerbCam is in control of the camera, the camera will jerk around a lot. Workaround: turn off camera control and then back on (unfortunately there isn't a game event for view changes, so this might have to be fixed by polling the camera manager).
Some laptops may have trouble pressing F8 to toggle the window, and are therefore unable to change the keybinding to anything else. Workaround: Make a text file inside your top-level KSP directory called "kerbcam.cfg" (use Notepad, or similar), copy/paste the following into it: (change F8 to a value from the Unity3d KeyCode reference.)
KEY_BINDINGS
{
KEY_TOGGLE_WINDOW = F8
}