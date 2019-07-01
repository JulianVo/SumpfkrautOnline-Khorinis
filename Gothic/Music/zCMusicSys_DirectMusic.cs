using System;
using Gothic.Types;
using WinApi;

namespace Gothic.Music
{
    /// <summary>
    /// Class for accessing the gothic 2 music system.
    /// </summary>
    public static class zCMusicSys_DirectMusic
    {
        /// <summary>
        /// Address of the static instance used by the game. Its a zCMusicSystem pointer but zCMusicSys_DirectMusic inherits from that class.
        /// </summary>
        public const int StaticInstance = 0x8D1F14;

        /// <summary>
        /// Gets the current volume of the music system.
        /// </summary>
        /// <returns></returns>
        public static float GetVolume()
        {
            return Process.THISCALL<FloatArg>(StaticInstance, 0x4E8160);
        }

        /// <summary>
        /// Sets the music volume
        /// </summary>
        /// <param name="volume">music volume to be set</param>
        public static void SetVolume(float volume)
        {
            Process.THISCALL<NullReturnCall>(StaticInstance, 0x4E9FD0, (FloatArg)volume);
        }

        /// <summary>
        /// Completely mutes the music system.
        /// </summary>
        public static void Mute()
        {
            Process.THISCALL<NullReturnCall>(StaticInstance, 0x4E9F40);
        }

        /// <summary>
        /// Stops the currently played music.
        /// <remarks>Does not mute the music system. Instead a new music can be started.</remarks>
        /// </summary>
        public static void Stop()
        {
            Process.THISCALL<NullReturnCall>(StaticInstance, 0x4E9F20);
        }

        /// <summary>
        /// Checks whether a given music theme is available(e.g. SYS_Menu).
        /// </summary>
        /// <param name="musicName">The name of the music theme that should be searched.</param>
        /// <returns>True if the music theme is available</returns>
        public static bool IsAvailable(string musicName)
        {
            if (musicName == null)
            {
                throw new ArgumentNullException(nameof(musicName));
            }

            using (zString z = zString.Create(musicName))
            {
                return Process.THISCALL<BoolArg>(StaticInstance, 0x4EA010, z);
            }
        }


        //Does not work... crashes. I think this is caused by us blocking the script init call.
        // Maybe we can start our own themes by using the other more complicated methods such as "PlayTheme" which requires loading a theme first.
        //public static void PlayThemeByScript(string musicName,int argument1=0,int argument2=0)
        //{
        //    if (musicName == null)
        //    {
        //        throw new ArgumentNullException(nameof(musicName));
        //    }

 
        //        Process.THISCALL<NullReturnCall>(StaticInstance, 0x4E8AB0, zString.Create(musicName), (IntArg)argument1, (IntArg)argument2);
          
        //}
    }
}
