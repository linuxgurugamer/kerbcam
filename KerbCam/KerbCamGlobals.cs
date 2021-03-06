// 
//     Copyright (C) 2015 CYBUTEK - https://github.com/CYBUTEK
//     Copyright (C) 2016 Cartman09 - https://github.com/cartman09 
//
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 

namespace KerbCam
{
    using System.IO;
    using System.Reflection;

    public static class KerbCamGlobals
    {
        
        /// <summary>
        ///     Current version of the KerbCam assembly.
        /// </summary>
        public const string ASSEMBLY_VERSION = "0.18.0.0";
        public static Assembly assembly = Assembly.GetCallingAssembly();

        private static string assemblyFile;
        private static string assemblyName;
        private static string assemblyPath;
        private static string settingsPath;

        /// <summary>
        ///     Gets the KerbCams assembly's path including the file name.
        /// </summary>
        public static string AssemblyFile
        {
            get
            {
                return assemblyFile ?? (assemblyFile = Assembly.GetExecutingAssembly().Location);
            }
        }

        /// <summary>
        ///     Gets the KerbCams assembly's file name.
        /// </summary>
        public static string AssemblyName
        {
            get
            {
                return assemblyName ?? (assemblyName = new FileInfo(AssemblyFile).Name);
            }
        }

        /// <summary>
        ///     Gets the KerbCams assembly's path excluding the file name.
        /// </summary>
        public static string AssemblyPath
        {
            get
            {
                return assemblyPath ?? (assemblyPath = AssemblyFile.Replace(new FileInfo(AssemblyFile).Name, ""));
            }
        }

        /// <summary>
        ///     Gets the settings directory path.
        /// </summary>
        public static string SettingsPath
        {
            get
            {
                if (string.IsNullOrEmpty(settingsPath))
                {
                    settingsPath = Path.Combine(AssemblyPath, "Settings");
                }
                return settingsPath;
            }
        }
    }
}
